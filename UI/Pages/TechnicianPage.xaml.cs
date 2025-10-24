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
    /// Логика взаимодействия для TechnicianPage.xaml
    /// </summary>
    public partial class TechnicianPage : Page
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;

        public TechnicianPage(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
            LoadTechnicianOrders();
        }

        private void LoadTechnicianOrders()
        {
            var orders = _context.order
                .Where(o => o.orderstatus == "принят" || o.orderstatus == "готовится")
                .ToList();

            TechnicianOrdersItemsControl.ItemsSource = orders;
            txtNoTechnicianOrders.Visibility = orders.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateStatusColors()
        {
            // Эта функция будет вызываться при изменении статусов
            // Пока оставим статические цвета
        }

        private void MarkAsInProgress(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int orderId = (int)button.Tag;

            var order = _context.order.Find(orderId);
            if (order != null && order.orderstatus == "принят")
            {
                order.orderstatus = "готовится";
                _context.SaveChanges();
                LoadTechnicianOrders();
                MessageBox.Show("Статус заказа изменен на 'В работе'", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MarkAsReady(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int orderId = (int)button.Tag;

            var order = _context.order.Find(orderId);
            if (order != null && order.orderstatus == "готовится")
            {
                order.orderstatus = "готов";
                _context.SaveChanges();
                LoadTechnicianOrders();
                MessageBox.Show("Статус заказа изменен на 'Готов'", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
