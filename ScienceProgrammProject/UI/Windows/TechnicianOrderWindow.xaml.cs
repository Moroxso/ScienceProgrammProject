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
using System.Windows.Shapes;

namespace ScienceProgrammProject.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для TechnicianOrderWindow.xaml
    /// </summary>
    public partial class TechnicianOrderWindow : Window
    {
        private ScienceProgrammProjectEntities _context;
        private order _currentOrder;

        public bool OrderUpdated { get; private set; }

        public TechnicianOrderWindow(ScienceProgrammProjectEntities context, order orderToEdit)
        {
            InitializeComponent();
            _context = context;
            _currentOrder = orderToEdit;

            LoadOrderData();
            UpdateButtonsVisibility();
        }

        private void LoadOrderData()
        {
            // Загружаем информацию о заказе
            txtConferenceName.Text = _currentOrder.nameconference;
            txtEquipment.Text = _currentOrder.equipment;
            txtGuestsCount.Text = _currentOrder.amountguests.ToString();

            // Обновляем отображение статуса
            UpdateStatusDisplay(_currentOrder.orderstatus);
        }

        private void UpdateStatusDisplay(string status)
        {
            txtCurrentStatus.Text = status.ToUpper();

            // Меняем цвет в зависимости от статуса
            switch (status.ToLower())
            {
                case "принят":
                    borderCurrentStatus.Background = new SolidColorBrush(Color.FromRgb(241, 196, 15)); // желтый
                    break;
                case "готовится":
                    borderCurrentStatus.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)); // синий
                    break;
                case "готов":
                    borderCurrentStatus.Background = new SolidColorBrush(Color.FromRgb(39, 174, 96)); // зеленый
                    break;
                default:
                    borderCurrentStatus.Background = new SolidColorBrush(Color.FromRgb(149, 165, 166)); // серый
                    break;
            }
        }

        private void UpdateButtonsVisibility()
        {
            // Показываем/скрываем кнопки в зависимости от текущего статуса
            switch (_currentOrder.orderstatus.ToLower())
            {
                case "принят":
                    btnMarkInProgress.Visibility = Visibility.Visible;
                    btnMarkReady.Visibility = Visibility.Collapsed;
                    break;
                case "готовится":
                    btnMarkInProgress.Visibility = Visibility.Collapsed;
                    btnMarkReady.Visibility = Visibility.Visible;
                    break;
                case "готов":
                    btnMarkInProgress.Visibility = Visibility.Collapsed;
                    btnMarkReady.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void UpdateOrderStatus(string newStatus)
        {
            try
            {
                _currentOrder.orderstatus = newStatus;
                _context.SaveChanges();

                OrderUpdated = true;
                UpdateStatusDisplay(newStatus);
                UpdateButtonsVisibility();

                MessageBox.Show($"Статус заказа изменен на '{newStatus}'", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении статуса: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnMarkInProgress_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Взять этот заказ в работу?\n\nПосле этого вы будете отвечать за его подготовку.",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UpdateOrderStatus("готовится");
            }
        }

        private void btnMarkReady_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Завершить подготовку заказа?\n\nУбедитесь, что всё оборудование готово к конференции.",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UpdateOrderStatus("готов");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
