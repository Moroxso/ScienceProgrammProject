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
using ScienceProgrammProject.UI.Windows;
using ScienceProgrammProject.Core.Services;

namespace ScienceProgrammProject.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;
        private ShiftService _shiftService;

        public OrdersPage(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
            _shiftService = new ShiftService(_context);
            LoadOrders();
        }

        private void LoadOrders()
        {
            if (_currentUser.userroleid == 2) // Организатор - только заказы активной смены
            {
                var shiftOrders = _shiftService.GetOrdersForUserShift(_currentUser.userid).ToList();
                OrdersItemsControl.ItemsSource = shiftOrders;

                txtNoOrders.Visibility = shiftOrders.Any() ? Visibility.Collapsed : Visibility.Visible;

                // Показываем информацию о смене
                var currentShift = _shiftService.GetCurrentShiftForUser(_currentUser.userid);
                if (currentShift != null)
                {
                    txtNoOrders.Text = $"Нет заказов для активной смены ({currentShift.datestart:dd.MM.yyyy} - {currentShift.dateend:dd.MM.yyyy})";
                }
                else
                {
                    txtNoOrders.Text = "У вас нет активной смены. Обратитесь к заведующему.";
                }
            }
            else // Заведующий - все заказы
            {
                var orders = _context.order.ToList();
                OrdersItemsControl.ItemsSource = orders;
                txtNoOrders.Visibility = orders.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.userroleid == 2) // Организатор
            {
                if (!_shiftService.HasActiveShift(_currentUser.userid))
                {
                    MessageBox.Show("У вас нет активной смены. Невозможно создать заказ.", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var createOrderWindow = new CreateOrderWindow(_currentUser, _context);
            createOrderWindow.Owner = Application.Current.MainWindow;
            createOrderWindow.Closed += (s, args) => LoadOrders();
            createOrderWindow.ShowDialog();
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int orderId = (int)button.Tag;

            var order = _context.order.Find(orderId);
            if (order != null)
            {
                var editWindow = new EditOrderWindow(_context, order, _currentUser);
                editWindow.Owner = Application.Current.MainWindow;
                editWindow.Closed += (s, args) => LoadOrders();
                editWindow.ShowDialog();
            }
        }
    }
}
