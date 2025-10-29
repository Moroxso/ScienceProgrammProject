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
    /// Логика взаимодействия для EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        private ScienceProgrammProjectEntities _context;
        private order _currentOrder;
        private user _currentUser;

        public EditOrderWindow(ScienceProgrammProjectEntities context, order orderToEdit, user currentUser)
        {
            InitializeComponent();
            _context = context;
            _currentOrder = orderToEdit;
            _currentUser = currentUser;

            LoadOrderData();
            ConfigureForUserRole();
        }

        private void LoadOrderData()
        {
            // Загружаем данные заказа
            txtConferenceName.Text = _currentOrder.nameconference;
            txtEquipment.Text = _currentOrder.equipment;
            txtGuestsCount.Text = _currentOrder.amountguests.ToString();

            // Устанавливаем статусы
            SetComboBoxValue(cmbOrderStatus, _currentOrder.orderstatus);
            SetComboBoxValue(cmbPaymentStatus, _currentOrder.paymentstatus);

            // Информация о заказе
            txtTitle.Text = $"Редактирование заказа #{_currentOrder.orderid}";
            txtOrderId.Text = $"ID заказа: #{_currentOrder.orderid}";
            txtCreationDate.Text = $"Дата создания: {_currentOrder.datecreation:dd.MM.yyyy}";
            txtLastModified.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }

        private void SetComboBoxValue(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == value)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void ConfigureForUserRole()
        {
            // Настройка доступности элементов в зависимости от роли
            switch (_currentUser.userroleid)
            {
                case 2: // Организатор
                    cmbOrderStatus.IsEnabled = false; // Организатор не меняет статус выполнения
                    btnMarkInProgress.Visibility = Visibility.Collapsed;
                    btnMarkReady.Visibility = Visibility.Collapsed;
                    break;
                case 3: // Техник
                    txtConferenceName.IsEnabled = false;
                    txtEquipment.IsEnabled = false;
                    txtGuestsCount.IsEnabled = false;
                    cmbPaymentStatus.IsEnabled = false;
                    break;
                case 1: // Заведующий - полный доступ
                    // Все элементы доступны
                    break;
            }
        }

        private void UpdateStatus(string newStatus)
        {
            try
            {
                _currentOrder.orderstatus = newStatus;
                _context.SaveChanges();

                // Обновляем интерфейс
                SetComboBoxValue(cmbOrderStatus, newStatus);
                txtLastModified.Text = $"только что ({DateTime.Now:HH:mm})";

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
            UpdateStatus("готовится");
        }

        private void btnMarkReady_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("готов");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(txtConferenceName.Text))
                {
                    MessageBox.Show("Введите название конференции", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtConferenceName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEquipment.Text))
                {
                    MessageBox.Show("Введите требуемое оборудование", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEquipment.Focus();
                    return;
                }

                if (!int.TryParse(txtGuestsCount.Text, out int guests) || guests <= 0)
                {
                    MessageBox.Show("Введите корректное количество гостей", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtGuestsCount.Focus();
                    return;
                }

                // Обновление данных
                _currentOrder.nameconference = txtConferenceName.Text.Trim();
                _currentOrder.equipment = txtEquipment.Text.Trim();
                _currentOrder.amountguests = guests;
                _currentOrder.orderstatus = (cmbOrderStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
                _currentOrder.paymentstatus = (cmbPaymentStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

                _context.SaveChanges();

                MessageBox.Show("Изменения сохранены успешно!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
