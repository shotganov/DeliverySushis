using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DeliverySushi.Table; // Убедитесь, что этот namespace содержит ваши модели
using DeliverySushi.Model;

namespace DeliverySushi.ViewModel
{
    public class CourierViewModel : INotifyPropertyChanged
    {
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

        private int courierId; // Идентификатор курьера

        private ModelCourier courierModel;

        private bool _StatusDeliveryButtonVisible;
        public bool StatusDeliveryButtonVisible
        {
            get => _StatusDeliveryButtonVisible;
            set
            {
                _StatusDeliveryButtonVisible = value;
                OnPropertyChanged(nameof(StatusDeliveryButtonVisible));
            }
        }

        private bool _ConfirmDeliveryButtonVisible;
        public bool ConfirmDeliveryButtonVisible
        {
            get => _ConfirmDeliveryButtonVisible;
            set
            {
                _ConfirmDeliveryButtonVisible = value;
                OnPropertyChanged(nameof(ConfirmDeliveryButtonVisible));
            }
        }

        private string _buttonContent;
        public string ButtonContent
        {
            get => _buttonContent;
            set
            {
                _buttonContent = value;
                OnPropertyChanged(nameof(ButtonContent));
            }
        }

        public ObservableCollection<string> Statuses { get; set; }

        public ICommand CancelOrderCommand { get; set; }
        public ICommand AcceptOrderCommand { get; set; }
        public ICommand LoadWaitOrderCommand { get; set; }
        public ICommand LoadInDeliveryOrderCommand { get; set; }
        public ICommand ConfirmDeliverOrderCommand { get; set; }
        public ICommand LoadDeliveredOrdersCommand { get; set; }
        public ICommand LoadCanceledOrdersCommand { get; set; }

    

        public CourierViewModel(int courierId)
        {
            this.courierId = courierId;
            courierModel = new ModelCourier(courierId);


            // Статусы заказа
            Statuses = new ObservableCollection<string>
            {
                "Принят",
                "В пути",
                "Доставлен"
            };

            
            AcceptOrderCommand = new RelayCommand(AcceptOrder);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            LoadWaitOrderCommand = new RelayCommand(LoadWaitOrders);
            LoadInDeliveryOrderCommand = new RelayCommand(LoadInDeliveryOrders);
            ConfirmDeliverOrderCommand = new RelayCommand(ConfirmDeliveryOrder);
            LoadDeliveredOrdersCommand = new RelayCommand(LoadDeliveredOrders);
            LoadCanceledOrdersCommand = new RelayCommand(LoadCancelledOrders);
           
            LoadWaitOrders(Orders);
        }

        private async void LoadWaitOrders(object parameter)
        {

            var availableOrders = await courierModel.GetWaitOrders();

            Orders = new ObservableCollection<Order>(availableOrders);

            ConfirmDeliveryButtonVisible = false;
            StatusDeliveryButtonVisible = true;
            ButtonContent = "Взять заказ";
        }


        private async void LoadInDeliveryOrders(object parameter)
        {
            

            var availableOrders = await courierModel.GetInDeliveryOrders();

            Orders = new ObservableCollection<Order>(availableOrders);

            ConfirmDeliveryButtonVisible = true;
            StatusDeliveryButtonVisible = true;
            ButtonContent = "Отменить заказ";

        }


        private async void LoadDeliveredOrders(object parameter)
        {
           
            var deliveredOrders = await courierModel.GetDeliveredOrders();

            Orders = new ObservableCollection<Order>(deliveredOrders);

            ConfirmDeliveryButtonVisible = false;
            StatusDeliveryButtonVisible = false;
        }

        private async void LoadCancelledOrders(object parameter)
        {
          
            var cancelledOrders = await courierModel.GetCancelledOrders();

            Orders = new ObservableCollection<Order>(cancelledOrders);

            ConfirmDeliveryButtonVisible = false;
            StatusDeliveryButtonVisible = false;
        }
        private async void AcceptOrder(object parameter)
        {
            if (parameter is Order order)
            {

                await courierModel.AcceptDeliveryOrder(order);
                MessageBox.Show("Вы взяли заказ!");
                LoadWaitOrders(Orders);
              
            }
        }

        private async void CancelOrder(object parameter)
        {
            if (parameter is Order order)
            {
                await courierModel.CancelDeliveryOrder(order);
                MessageBox.Show("Вы отменили заказ!");
                LoadInDeliveryOrders(Orders);
                
            }
        }

        private async void ConfirmDeliveryOrder(object parameter)
        {
            if (parameter is Order order)
            {
                await courierModel.ConfirmDeliveryOrder(order);
                MessageBox.Show("Поздравляем с доставкой заказа!");
                LoadInDeliveryOrders(Orders);
               
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}