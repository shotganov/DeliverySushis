using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeliverySushi.ViewModel;
using System.Threading.Tasks;
using DeliverySushi.Table;

namespace DeliverySushi.View
{
    public partial class RegPage : Page
    {
        private readonly UserViewModel userViewModel;

        public RegPage()
        {
            InitializeComponent();
            userViewModel = new UserViewModel();
        }

        private async void Button_Reg_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = passBox.Password.Trim();
            string confirmPassword = passBox_2.Password.Trim();
            string email = textBoxEmail.Text.Trim().ToLower();
            string phoneNum = textBoxNumber.Text.Trim();

            if (login.Length < 5)
            {
                ShowError(textBoxLogin, "Логин должен содержать не менее 5 символов.");
                return;
            }

            if (password.Length < 5)
            {
                ShowError(passBox, "Пароль должен содержать не менее 5 символов.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError(passBox_2, "Пароли не совпадают.");
                return;
            }

            if (email.Length < 5 || !email.Contains("@") || !email.Contains("."))
            {
                ShowError(textBoxEmail, "Введите корректный email.");
                return;
            }

            if (phoneNum.Length != 11 || !long.TryParse(phoneNum, out long phone))
            {
                ShowError(textBoxNumber, "Введите корректный номер телефона.");
                return;
            }

            ResetFieldStyle(textBoxLogin);
            ResetFieldStyle(passBox);
            ResetFieldStyle(passBox_2);
            ResetFieldStyle(textBoxEmail);
            ResetFieldStyle(textBoxNumber);


            int registered = await userViewModel.RegisterAsync(login, password, email, phone);
   
            if (registered > 0)
            {
                var mainViewModel = (MainViewModel)Application.Current.MainWindow.DataContext;
                mainViewModel.UserViewModel = userViewModel;

                mainViewModel.SushiViewModel.SetId(registered);
                mainViewModel.SetViewModel.SetId(registered);
                mainViewModel.AddonViewModel.SetId(registered);
                mainViewModel.CartViewModel.SetId(registered);
                mainViewModel.UserViewModel.SetId(registered);
                mainViewModel.OrderViewModel.SetId(registered);

                var customer = mainViewModel.UserViewModel.GetCustomerByIdAsync(registered);

                NavigationService.Navigate(new UserPage());
            }
          
        }

        private void Button_Window_Auth_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new AuthPage());
        }

        private void ShowError(Control control, string message)
        {
            control.ToolTip = message;
            control.Background = Brushes.DarkRed;
        }

        private void ResetFieldStyle(Control control)
        {
            control.ToolTip = null;
            control.Background = Brushes.Transparent;
        }
    }
}
