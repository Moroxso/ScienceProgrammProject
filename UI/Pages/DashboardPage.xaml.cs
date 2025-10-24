using ScienceProgrammProject.Data;
using ScienceProgrammProject.UI.Windows;
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
    /// Логика взаимодействия для DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;

        public DashboardPage(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // Приветствие
            txtWelcome.Text = $"Добро пожаловать, {_currentUser.firstname} {_currentUser.lastname}!";

            // Статистика заказов
            var orders = _context.order.ToList();
            txtTotalOrders.Text = orders.Count.ToString();
            txtActiveOrders.Text = orders.Count(o => o.orderstatus == "принят" || o.orderstatus == "готовится").ToString();
            txtReadyOrders.Text = orders.Count(o => o.orderstatus == "готов").ToString();

            // Настройка быстрых действий по ролям
            switch (_currentUser.userroleid)
            {
                case 1: // Заведующий
                    borderQuickUsers.Visibility = Visibility.Visible;
                    borderQuickCreate.Visibility = Visibility.Visible;
                    break;
                case 2: // Организатор
                    borderQuickCreate.Visibility = Visibility.Visible;
                    break;
                case 3: // Техник
                    // Только просмотр заказов
                    break;
            }
        }

        private void btnQuickOrders_Click(object sender, RoutedEventArgs e)
        {
            // Навигация к заказам
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.btnOrders_Click(sender, e);
        }

        private void btnQuickCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            var createOrderWindow = new CreateOrderWindow(_currentUser, _context);
            createOrderWindow.Owner = Application.Current.MainWindow;
            createOrderWindow.ShowDialog();
        }

        private void btnQuickUsers_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.btnUsers_Click(sender, e);
        }
    }
}
