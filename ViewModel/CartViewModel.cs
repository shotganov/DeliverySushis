using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows.Input;
using DeliverySushi.Table;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using DeliverySushi.Model;
using DeliverySushi.View;
using System.Net;

namespace DeliverySushi.ViewModel
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public event EventHandler OrderUpdated; // Событие для обновления корзины

        private int userId;
        private ModelOrder orderModel;
        private ModelCart cartModel;
        private ModelOrder_Item order_itemModel;

        private ObservableCollection<object> cartItems; // Для хранения и суши, сетов и дополений
        public ObservableCollection<object> CartItems
        {
            get => cartItems;
            set
            {
                cartItems = value;
                OnPropertyChanged();
            }
        }

        private int sum;
        public int Sum
        {
            get => sum;
            set
            {
                sum = value;
                OnPropertyChanged();
            }
        }

        public async Task<int> GetSum()
        {
            return sum;
        }

        public ICommand MakeOrderCommand { get; private set; }
        public ICommand IncreaseQuantityCommand { get; private set; }
        public ICommand DecreaseQuantityCommand { get; private set; }

        public CartViewModel()
        {
            MakeOrderCommand = new RelayCommand(MakeOrderForm);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);

        }

        public async void SetId(int id)
        {
            userId = id;
            orderModel = new ModelOrder(id);
            cartModel = new ModelCart(id);
            order_itemModel = new ModelOrder_Item(id);
            RefreshCartItems();
        }
        private async void MakeOrderForm(object parameter)
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            // Открываем форму заказа через Dispatcher
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var orderForm = new OrderForm(Sum);
                bool? result = orderForm.ShowDialog();

                if (result == true)
                {
                    string recipientName = orderForm.RecipientName;
                    string deliveryAddress = orderForm.DeliveryAddress;
                    string phoneNumber = orderForm.PhoneNumber;
                    if (CheckInputData(recipientName, deliveryAddress, phoneNumber))
                    {
                        await MakeOrder(Sum, recipientName, deliveryAddress, phoneNumber);
                        RefreshCartItems();
                    }

                }
            });
        }

        private bool CheckInputData(string recipientName, string deliveryAddress, string phoneNumber )
        {
            if (string.IsNullOrWhiteSpace(recipientName) || string.IsNullOrWhiteSpace(deliveryAddress) || phoneNumber.Length != 11)
            {
                MessageBox.Show("Данные не введены или введены некорректно");
                return false;
                
            }
            else
            {
                return true;
            }
        }

        public async void RefreshCartItems()
        {
            await LoadCartItems();
        }
        public async Task MakeOrder(int sum, string name, string adress, string phone)
        {
           

            await cartModel.DeleteCartItems();

            var newOrder = await orderModel.CreateNewOrder(sum, name, adress, phone);
              
            await order_itemModel.CreateOrder_Items(CartItems, newOrder);

            OrderUpdated?.Invoke(this, EventArgs.Empty);

            CartItems = new ObservableCollection<object>();

            Sum = 0;
}
        private async Task LoadCartItems()
        {
            if (userId <= 0)
            {
                CartItems = new ObservableCollection<object>();
                Sum = 0; 
                return;
            }


            CartItems = await cartModel.LoadCartItem();
            Sum = await cartModel.GetSum();
        }

        private void IncreaseQuantity(object parameter)
        {
            if (parameter is Sushi sushi)
            {
                UpdateQuantity(sushi, true);
            }
            else if (parameter is Set set)
            {
                UpdateQuantity(set, true);
            }
            else if (parameter is Addon addon)
            {
                UpdateQuantity(addon, true);
            }
        }

        private void DecreaseQuantity(object parameter)
        {
            if (parameter is Sushi sushi)
            {
                UpdateQuantity(sushi, false);
            }
            else if (parameter is Set set)
            {
                UpdateQuantity(set, false);
            }
            else if (parameter is Addon addon)
            {
                UpdateQuantity(addon, false);
            }
        }

        private async void UpdateQuantity(object item, bool increase)
        {
            
            await cartModel.ChangeQuantity(item, increase);
            RefreshCartItems();
        }

     
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}