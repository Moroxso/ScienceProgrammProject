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
    /// Логика взаимодействия для CreateOrderWindow.xaml
    /// </summary>
    public partial class CreateOrderWindow : Window
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;

        public CreateOrderWindow(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
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

                // Создание заказа
                var newOrder = new order
                {
                    datecreation = DateTime.Now.Date,
                    orderstatus = "принят",
                    paymentstatus = (cmbPaymentStatus.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    nameconference = txtConferenceName.Text.Trim(),
                    equipment = txtEquipment.Text.Trim(),
                    amountguests = guests
                };

                _context.order.Add(newOrder);
                _context.SaveChanges();

                MessageBox.Show("Заказ успешно создан!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании заказа: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
