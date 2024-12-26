using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DeliverySushi.Table;
using DeliverySushi.Model;
using System.Collections.Generic;
using System.Windows;
using System;

namespace DeliverySushi.ViewModel
{
    public class OrderViewModel : INotifyPropertyChanged
    {
       

        private int userId;
        private ModelOrder modelOrder;
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

        public OrderViewModel()
        {
           
            Orders = new ObservableCollection<Order>();
        }

        public async void RefreshOrderItems()
        {
            await LoadOrders();
        }

        public async void SetId(int id)
        {
            userId = id;
            modelOrder = new ModelOrder(userId);
            RefreshOrderItems();
        }

        private async Task LoadOrders()
        {
            if (userId <= 0)
            {
                Orders.Clear();
                return;
            }

            try
            {
                var userOrders = await modelOrder.GetUserOrders();
                Orders = new ObservableCollection<Order>(userOrders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}