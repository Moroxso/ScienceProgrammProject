using ScienceProgrammProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using ScienceProgrammProject.UI.Pages;
using ScienceProgrammProject.Core.DataScripts;

namespace ScienceProgrammProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScienceProgrammProjectEntities _context;
        public user CurrentUser { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            _context = DatabaseManager.GetContext();

            // Загружаем страницу авторизации при старте
            NavigateToLoginPage();
        }

        private void NavigateToLoginPage()
        {
            var loginPage = new LoginPage(this);
            MainContentFrame.Navigate(loginPage);

            // Скрываем меню до авторизации
            SetMenuVisibility(false);
        }

        public void SetMenuVisibility(bool isVisible)
        {
            btnDashboard.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            btnOrders.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            btnShifts.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            btnUsers.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            btnLogout.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            btnLogin.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void SetUserInfo(user user)
        {
            CurrentUser = user;
            if (user != null)
            {
                txtUserInfo.Text = $"{user.lastname} {user.firstname} ({user.userrole.namerole})";
                SetMenuVisibility(true);

                // Настраиваем видимость меню в зависимости от роли
                ConfigureMenuByRole(user.userroleid);
            }
        }

        private void ConfigureMenuByRole(int userRoleId)
        {
            // Сбрасываем видимость всех кнопок
            btnDashboard.Visibility = Visibility.Visible;
            btnOrders.Visibility = Visibility.Visible;
            btnShifts.Visibility = Visibility.Visible;
            btnUsers.Visibility = Visibility.Visible;

            // Настраиваем доступ в зависимости от роли
            switch (userRoleId)
            {
                case 1: // Заведующий подразделением - полный доступ
                    break;
                case 2: // Организатор
                    btnUsers.Visibility = Visibility.Collapsed;
                    btnShifts.Visibility = Visibility.Collapsed;
                    break;
                case 3: // Техник
                    btnUsers.Visibility = Visibility.Collapsed;
                    btnShifts.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        // Обработчики навигации
        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            var dashboardPage = new DashboardPage(CurrentUser, _context);
            MainContentFrame.Navigate(dashboardPage);
        }

         public void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            // В зависимости от роли показываем разные страницы
            if (CurrentUser?.userroleid == 2) // Организатор
            {
                var ordersPage = new OrdersPage(CurrentUser, _context);
                MainContentFrame.Navigate(ordersPage);
            }
            else if (CurrentUser?.userroleid == 3) // Техник
            {
                var technicianPage = new TechnicianPage(CurrentUser, _context);
                MainContentFrame.Navigate(technicianPage);
            }
            else // Заведующий видит все
            {
                var ordersPage = new OrdersPage(CurrentUser, _context);
                MainContentFrame.Navigate(ordersPage);
            }
        }

        public void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser?.userroleid == 1) // Только заведующий
            {
                var usersPage = new UsersPage(CurrentUser, _context);
                MainContentFrame.Navigate(usersPage);
            }
            else
            {
                MessageBox.Show("Доступ запрещен. Только для заведующего подразделением.",
                              "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void btnShifts_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser?.userroleid == 1) // Только заведующий
            {
                var shiftsPage = new ShiftsPage(CurrentUser, _context);
                MainContentFrame.Navigate(shiftsPage);
            }
            else
            {
                MessageBox.Show("Доступ запрещен. Только для заведующего подразделением.",
                              "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigateToLoginPage();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser = null;
            txtUserInfo.Text = "Не авторизован";
            NavigateToLoginPage();
        }
    }
}
