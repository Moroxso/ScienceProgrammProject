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

namespace ScienceProgrammProject.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;

        public OrdersPage(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
            LoadOrders();
        }

        private void LoadOrders()
        {
            // Для организатора показываем все заказы
            var orders = _context.order.ToList();
            OrdersItemsControl.ItemsSource = orders;
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            var createOrderWindow = new CreateOrderWindow(_currentUser, _context);
            createOrderWindow.Owner = Application.Current.MainWindow;
            createOrderWindow.Closed += (s, args) => LoadOrders(); // Обновляем список после закрытия
            createOrderWindow.ShowDialog();
        }
    }
}
