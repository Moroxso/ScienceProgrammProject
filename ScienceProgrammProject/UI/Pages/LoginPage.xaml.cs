using ScienceProgrammProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScienceProgrammProject.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private MainWindow _mainWindow;
        private ScienceProgrammProjectEntities _context;

        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _context = new ScienceProgrammProjectEntities();

            // Устанавливаем фокус на поле логина при загрузке
            Loaded += (s, e) => txtLogin.Focus();

            // Обработчики для Enter
            txtLogin.KeyDown += TextBox_KeyDown;
            txtPassword.KeyDown += PasswordBox_KeyDown;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            // Валидация
            if (string.IsNullOrEmpty(login))
            {
                ShowError("Введите логин");
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль");
                txtPassword.Focus();
                return;
            }

            // Показываем индикатор загрузки
            SetLoadingState(true);

            try
            {
                // Имитация задержки для лучшего UX
                await System.Threading.Tasks.Task.Delay(800);

                // Поиск пользователя в базе данных
                var user = _context.user
                    .Include("userrole")
                    .FirstOrDefault(u => u.login == login && u.password == password);

                if (user != null)
                {
                    // Проверяем статус пользователя
                    if (user.status?.ToLower() == "уволен")
                    {
                        ShowError("Учетная запись заблокирована. Обратитесь к администратору.");
                        return;
                    }

                    // Успешная авторизация
                    ShowError(""); // Очищаем ошибки
                    _mainWindow.SetUserInfo(user);

                    // Переходим на главную страницу
                    var dashboardPage = new DashboardPage(user, _context);
                    _mainWindow.MainContentFrame.Navigate(dashboardPage);
                }
                else
                {
                    ShowError("Неверный логин или пароль. Проверьте введенные данные.");
                    txtPassword.Password = "";
                    txtPassword.Focus();
                }
            }
            catch (System.Exception ex)
            {
                ShowError($"Ошибка подключения к базе данных: {ex.Message}");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetLoadingState(bool isLoading)
        {
            loadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            btnLogin.IsEnabled = !isLoading;
            txtLogin.IsEnabled = !isLoading;
            txtPassword.IsEnabled = !isLoading;
        }
    }
}
