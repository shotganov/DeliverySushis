using DeliverySushi.Table;
using DeliverySushi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;
using System.Data.Entity;
using DeliverySushi.Model;

namespace DeliverySushi.ViewModel
{
    public class SushiViewModel : INotifyPropertyChanged
    {
        public event EventHandler CartUpdated; // Событие для обновления корзины

        private ObservableCollection<Sushi> sushis;
        private int userId;
        private ModelSushi sushiModel;

        public ObservableCollection<Sushi> Sushis
        {
            get => sushis;
            set
            {
                sushis = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddToCartCommand { get; }

        public SushiViewModel()
        {
            AddToCartCommand = new RelayCommand(param =>
            {
                if (param is Sushi selectedSushi)
                {
                    AddToCart(selectedSushi);
                }
            });

            sushiModel = new ModelSushi();
            LoadSushi();

        }
        public async void SetId(int id)
        {
            userId = id;
            sushiModel.SetId(id);
            LoadSushi();
        }

        private async void AddToCart(Sushi selectedSushi)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Ошибка: пользователь не авторизован.");
                return;
            }

            if (selectedSushi == null)
            {
                MessageBox.Show("Ошибка: суши не выбраны.");
                return;
            }

            await sushiModel.AddSushiToCart(selectedSushi);
           
            CartUpdated?.Invoke(this, EventArgs.Empty);

            MessageBox.Show($"Суши {selectedSushi.name} добавлены в корзину.");
            
        }

     
        private async void LoadSushi()
        {
       
            var sushiList = await sushiModel.GetSushis();  
            Sushis = new ObservableCollection<Sushi>(sushiList);
        
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}