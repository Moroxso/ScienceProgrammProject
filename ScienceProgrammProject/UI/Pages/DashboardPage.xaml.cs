using ScienceProgrammProject.Core.Services;
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

            int activeOrdersCount = orders.Count(o => o.orderstatus == "принят" || o.orderstatus == "готовится");
            txtActiveOrders.Text = activeOrdersCount.ToString();

            int readyOrdersCount = orders.Count(o => o.orderstatus == "готов");
            txtReadyOrders.Text = readyOrdersCount.ToString();

            // Статистика сотрудников
            int activeUsersCount = _context.user.Count(u => string.IsNullOrEmpty(u.status) || u.status != "уволен");
            txtActiveUsers.Text = activeUsersCount.ToString();

            // Настройка быстрых действий по ролям
            ConfigureQuickActions();

            // Загрузка последних активностей
            LoadRecentActivities();
        }

        private void ConfigureQuickActions()
        {
            // Всегда видимые кнопки
            btnQuickOrders.Visibility = Visibility.Visible;
            btnQuickReports.Visibility = Visibility.Visible;

            // Настройка по ролям
            if (_currentUser.userroleid == 1) // Заведующий
            {
                borderQuickUsers.Visibility = Visibility.Visible;
                borderQuickCreate.Visibility = Visibility.Visible;
                borderQuickShifts.Visibility = Visibility.Visible;
            }
            else if (_currentUser.userroleid == 2) // Организатор
            {
                borderQuickCreate.Visibility = Visibility.Visible;
            }
            else if (_currentUser.userroleid == 3) // Техник
            {
                borderQuickTechnician.Visibility = Visibility.Visible;
            }
        }

        private void LoadRecentActivities()
        {
            var activities = new List<string>();
            var recentOrders = _context.order
                .OrderByDescending(o => o.datecreation)
                .Take(5)
                .ToList();

            foreach (var order in recentOrders)
            {
                string statusIcon = GetStatusIcon(order.orderstatus);
                activities.Add($"{statusIcon} {order.datecreation:dd.MM.yyyy} - {order.nameconference} ({order.orderstatus})");
            }

            RecentActivitiesItemsControl.ItemsSource = activities;

            if (activities.Any())
            {
                txtNoActivities.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoActivities.Visibility = Visibility.Visible;
            }
        }

        private string GetStatusIcon(string status)
        {
            if (status == "готов")
                return "✅";
            else if (status == "готовится")
                return "🔄";
            else if (status == "принят")
                return "📥";
            else
                return "📋";
        }

        // Обработчики быстрых действий
        private void btnQuickOrders_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.btnOrders_Click(sender, e);
            }
        }

        private void btnQuickCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            var createOrderWindow = new CreateOrderWindow(_currentUser, _context);
            createOrderWindow.Owner = Application.Current.MainWindow;
            createOrderWindow.ShowDialog();
            LoadDashboardData(); // Обновляем данные после закрытия
        }

        private void btnQuickUsers_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.btnUsers_Click(sender, e);
            }
        }

        private void btnQuickShifts_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.btnShifts_Click(sender, e);
            }
        }

        private void btnQuickTechnician_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.btnOrders_Click(sender, e);
            }
        }

        private void btnQuickReports_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция отчетов будет реализована в следующем обновлении",
                          "В разработке",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }
    }
}
