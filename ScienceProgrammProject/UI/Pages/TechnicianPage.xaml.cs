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
    /// Логика взаимодействия для TechnicianPage.xaml
    /// </summary>
    public partial class TechnicianPage : Page
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;
        private ShiftService _shiftService;

        public TechnicianPage(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
            _shiftService = new ShiftService(_context);
            LoadTechnicianOrders();
        }

        private void LoadTechnicianOrders()
        {
            if (!_shiftService.HasActiveShift(_currentUser.userid))
            {
                txtNoTechnicianOrders.Text = "У вас нет активной смены. Обратитесь к заведующему.";
                txtNoTechnicianOrders.Visibility = Visibility.Visible;
                TechnicianOrdersItemsControl.ItemsSource = null;
                return;
            }

            var shiftOrders = _shiftService.GetOrdersForUserShift(_currentUser.userid)
                .Where(o => o.orderstatus == "принят" || o.orderstatus == "готовится")
                .ToList();

            TechnicianOrdersItemsControl.ItemsSource = shiftOrders;
            txtNoTechnicianOrders.Visibility = shiftOrders.Any() ? Visibility.Collapsed : Visibility.Visible;

            // Обновляем видимость кнопок в зависимости от статусов
            UpdateButtonsVisibility();
        }

        private void UpdateButtonsVisibility()
        {
            // Эта функция будет обновлять видимость кнопок в зависимости от статуса заказа
            // Реализуем позже через конвертеры или в code-behind
        }

        private void ManageOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int orderId = (int)button.Tag;

            var order = _context.order.Find(orderId);
            if (order != null)
            {
                var techWindow = new TechnicianOrderWindow(_context, order);
                techWindow.Owner = Application.Current.MainWindow;
                techWindow.Closed += (s, args) =>
                {
                    if (techWindow.OrderUpdated)
                    {
                        LoadTechnicianOrders(); // Обновляем список если были изменения
                    }
                };
                techWindow.ShowDialog();
            }
        }

        // Обновите существующие методы для обновления интерфейса
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
