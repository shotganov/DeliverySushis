using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.Entity;
using System.Threading.Tasks;
using DeliverySushi.Table;
using System.Windows.Media.Imaging;
using System;
using System.Windows;
using System.Windows.Input;
using DeliverySushi.Model;
using System.Runtime.Remoting.Contexts;

namespace DeliverySushi.ViewModel
{
    public class AddonViewModel : INotifyPropertyChanged
    {
        public event EventHandler CartUpdated; // Событие для обновления корзины

        private int userId;
        private ModelAddon addonModel;
        private ObservableCollection<Addon> addons;

        public ICommand AddToCartCommand { get; }

        public ObservableCollection<Addon> Addons
        {
            get => addons;
            set
            {
                addons = value;
                OnPropertyChanged();
            }
        }

        public AddonViewModel()
        {
            AddToCartCommand = new RelayCommand(param =>
            {
                if (param is Addon selectedAddon)
                {
                    AddToCart(selectedAddon);
                }
            });

            addonModel = new ModelAddon();
            LoadAddons();

        }

        public async void SetId(int id)
        {
            userId = id;
            addonModel.SetId(id);
        }

        public async void AddToCart(Addon selectedAddon)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Ошибка: пользователь не авторизован.");
                return;
            }

            if (selectedAddon == null)
            {
                MessageBox.Show("Ошибка: дополнение не выбрано.");
                return;
            }

            await addonModel.AddAddonToCart(selectedAddon);
            CartUpdated?.Invoke(this, EventArgs.Empty);

            MessageBox.Show($"Дополнение {selectedAddon.name} добавлено в корзину.");
        
        }

      
        private async void LoadAddons()
        {
            var addonList = await addonModel.GetAddons();
            Addons = new ObservableCollection<Addon>(addonList);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
