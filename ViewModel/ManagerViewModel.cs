using DeliverySushi.Table;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DeliverySushi.Model;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Windows.Media.Imaging;
using PdfSharp.UniversalAccessibility.Drawing;

namespace DeliverySushi.ViewModel
{
    public class ManagerViewModel : INotifyPropertyChanged
    {
        private DateTime? _selectedDate;
        private SeriesCollection _seriesCollection;
        private ModelManager managerModel;
        
        public ICommand ShowStatisticsCommand { get; set; }
        public ICommand LoadNewOrderCommand { get; set; }
        public ICommand LoadHistoryOrdersCommand { get; set; }
        public ICommand AddCourierCommand { get; set; }
        public ICommand AddNewProductCommand { get; set; }
        public ICommand ConfirmOrderCommand { get; set; }
        public ICommand SaveToPdfCommand { get; set; }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                UpdateChart();
            }
        }

        private ObservableCollection<DateTime> _availableDates;
        public ObservableCollection<DateTime> AvailableDates
        {
            get => _availableDates;
            set
            {
                _availableDates = value;
                OnPropertyChanged(nameof(AvailableDates));
            }
        }

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        public ManagerViewModel()
        {
            // Инициализация доступных дат
            managerModel = new ModelManager();
            LoadAvailableDates();
            SaveToPdfCommand = new RelayCommand(SaveChartToPdf);
            LoadNewOrderCommand = new RelayCommand(LoadConfrirmListOrders);
            ConfirmOrderCommand = new RelayCommand(ConfirmOrder);
            LoadHistoryOrdersCommand = new RelayCommand(LoadHistoryOrders);
        }

        private bool _ConfirmButtonVisible;
        public bool ConfirmButtonVisible
        {
            get => _ConfirmButtonVisible;
            set
            {
                _ConfirmButtonVisible = value;
                OnPropertyChanged(nameof(ConfirmButtonVisible));
            }
        }


        private ObservableCollection<Order> orders;
        public ObservableCollection<Order> Orders
        {
            get => orders;
            set
            {
                orders = value;
                OnPropertyChanged();
            }
        }

        private async void SaveChartToPdf(object parameter)
        {
            if (SelectedDate == null)
            {
                MessageBox.Show("Дата не выбрана.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем диалог сохранения файла
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf", // Фильтр для выбора только PDF-файлов
                FileName = $"Orders_Summary_{SelectedDate:yyyyMMdd}.pdf", // Имя файла по умолчанию
                DefaultExt = ".pdf", // Расширение по умолчанию
                Title = "Сохранить данные о заказах" // Заголовок диалога
            };

            // Открываем диалог и проверяем, нажал ли пользователь "ОК"
            if (saveFileDialog.ShowDialog() == true)
            {
                // Получаем данные о товарах за выбранный день
                var productData = await managerModel.GetProductData(SelectedDate.Value);

                if (productData == null || !productData.Any())
                {
                    MessageBox.Show("Нет данных для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем PDF-документ
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 12);
                XBrush brush = XBrushes.Black;

                // Начальные координаты для текста
                double x = 50;
                double y = 50;
                double lineHeight = 20;

                // Заголовок
                gfx.DrawString($"Отчет по заказам за {SelectedDate:dd.MM.yyyy}", font, brush, new XPoint(x, y));
                y += lineHeight * 2; // Пропуск строки

                // Заголовки таблицы
                gfx.DrawString("Товар", font, brush, new XPoint(x, y));
                gfx.DrawString("Количество", font, brush, new XPoint(x + 200, y));
                gfx.DrawString("Стоимость", font, brush, new XPoint(x + 350, y));
                y += lineHeight;

                // Итоговая выручка
                double totalRevenue = 0;

                // Данные о товарах
                foreach (var product in productData)
                {
                    double price = managerModel.GetProductPrice(product.Key); // Получаем цену товара
                    double totalPrice = product.Value * price; // Общая стоимость товара

                    gfx.DrawString(product.Key, font, brush, new XPoint(x, y)); // Название товара
                    gfx.DrawString(product.Value.ToString(), font, brush, new XPoint(x + 200, y)); // Количество
                    gfx.DrawString($"{totalPrice:C}", font, brush, new XPoint(x + 350, y)); // Стоимость
                    y += lineHeight;

                    // Суммируем выручку
                    totalRevenue += totalPrice;
                }

                // Итоговая выручка
                y += lineHeight;
                gfx.DrawString($"Итоговая выручка: {totalRevenue:C}", font, brush, new XPoint(x, y));

                // Сохранение PDF по выбранному пользователем пути
                document.Save(saveFileDialog.FileName);
                MessageBox.Show($"Данные сохранены в файл {saveFileDialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private async void ConfirmOrder(object parameter)
        {
            if (parameter is Order order)
            {
                await managerModel.ConfirmOrder(order);
                LoadConfrirmListOrders(order);
            }
        }

        private async void LoadAvailableDates()
        {
            var availableDate = await managerModel.GetAvaliableDates();

            AvailableDates = new ObservableCollection<DateTime>(availableDate);

            if (AvailableDates.Any())
            {
                SelectedDate = AvailableDates.First();
            }
           
        }

        private async void LoadConfrirmListOrders(object parameter)
        {
            var availableOrders = await managerModel.GetConfrirmListOrders();
            ConfirmButtonVisible = true;
            Orders = new ObservableCollection<Order>(availableOrders);
          

        }

        private async void LoadHistoryOrders(object parameter)
        {
            var historyOrders = await managerModel.GetHistoryListOrders();
            ConfirmButtonVisible = false;
            Orders = new ObservableCollection<Order>(historyOrders);

          
        }
        
     

        private async void UpdateChart()
        {
            if (SelectedDate == null)
                return;

            var productData = await managerModel.GetProductData(SelectedDate.Value);
            // Создаем SeriesCollection для диаграммы
            SeriesCollection = new SeriesCollection();

            foreach (var product in productData)
            {
                SeriesCollection.Add(new PieSeries
                {
                    Title = product.Key, // Название продукта
                    Values = new ChartValues<int> { product.Value }, // Количество
                    DataLabels = true
                });
            }             

            OnPropertyChanged(nameof(SeriesCollection));           
            
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
