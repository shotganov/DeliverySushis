using DeliverySushi.Model;
using DeliverySushi.Table;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DeliverySushi.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private User user;
        private int userId;
        private string tempPassword;
        private string adress;
        private long phone;
        private string login;

        private ModelUser modelUser;

        public ICommand SaveCommand { get; }

        public UserViewModel()
        {
            modelUser = new ModelUser();
            SaveCommand = new RelayCommand(
                param => SaveChanges(),
                param => CanSave()
            );
        }

        public User User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }

        public string Adress
        {
            get => adress;
            set
            {
                adress = value;
                OnPropertyChanged();
            }
        }

        public long Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        public string TempPassword
        {
            get => tempPassword;
            set
            {
                tempPassword = value;
                OnPropertyChanged();
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(User.login) &&
                   !string.IsNullOrWhiteSpace(User.email) &&
                   !string.IsNullOrWhiteSpace(tempPassword) &&
                   User.phone > 0;
        }

        public async void SetId(int id)
        {
            if (id != userId)
            {
                userId = id;
                await LoadUser(userId);
            }
        }

        private async void SaveChanges()
        {
            try
            {
                await modelUser.SaveUserAsync(User, tempPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        public async Task LoadUser(int id)
        {
            if (id <= 0)
            {
                Console.WriteLine("Некорректный ID пользователя.");
                return;
            }

            try
            {
                var fetchedUser = await modelUser.LoadUserAsync(id);
                if (fetchedUser != null)
                {
                    User = fetchedUser;
                    TempPassword = fetchedUser.password;
                    Adress = fetchedUser.adress;
                    Phone = fetchedUser.phone;
                    Login = fetchedUser.login;
                }
                else
                {
                    User = new User();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки пользователя: {ex.Message}");
            }
        }

        public async Task<User> AuthenticateAsync(string login, string password)
        {
            try
            {
                var user = await modelUser.AuthenticateAsync(login, password);
                if (user != null)
                {
                    userId = user.id;
                    TempPassword = user.password;
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
                throw;
            }
        }

        public async Task<int> RegisterAsync(string login, string password, string email, long phone)
        {
            try
            {
                var userId = await modelUser.RegisterAsync(login, password, email, phone);
                if (userId > 0)
                {
                    this.userId = userId;
                    await LoadUser(userId);
                }
                return userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка регистрации: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetCustomerByIdAsync(int id)
        {
            try
            {
                var user = await modelUser.GetCustomerByIdAsync(id);
                if (user != null)
                {
                    User = user;
                    TempPassword = user.password;
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных пользователя: {ex.Message}");
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}