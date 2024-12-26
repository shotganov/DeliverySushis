using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Diagnostics;
using DeliverySushi.Table;
using DeliverySushi.Model;
using System.Windows;
using System.Windows.Input;
using DeliverySushi;

namespace DeliverySushi.ViewModel
{
    public class SetViewModel : INotifyPropertyChanged
    {
        public event EventHandler CartUpdated; // Событие для обновления корзины

        private ObservableCollection<Set> sets;
        private ModelSet setModel;
        private int userId;

        public ObservableCollection<Set> Sets
        {
            get => sets;
            set
            {
                sets = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddToCartCommand { get; }
        public SetViewModel()
        {
            
            AddToCartCommand = new RelayCommand(param =>
            {
                if (param is Set selectedSet)
                {
                    AddToCart(selectedSet);
                }
            });

            setModel = new ModelSet();
            LoadSets();
        }

        public async void SetId(int id)
        {
            userId = id;
            setModel.SetId(userId);
           
        }

        public async void AddToCart(Set selectedSet)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Ошибка: пользователь не авторизован.");
                return;
            }

            if (selectedSet == null)
            {
                MessageBox.Show("Ошибка: сет не выбран.");
                return;
            }

            await setModel.AddSetToCart(selectedSet);
          
            CartUpdated?.Invoke(this, EventArgs.Empty);

            MessageBox.Show($"Сет {selectedSet.name} добавлен в корзину.");
        }
      
        private async void LoadSets()
        {
          
           var setList = await setModel.GetSets();
           Sets = new ObservableCollection<Set>(setList);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
