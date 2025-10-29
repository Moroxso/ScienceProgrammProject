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
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        private ScienceProgrammProjectEntities _context;
        private user _currentUser;

        public EditUserWindow(ScienceProgrammProjectEntities context, user userToEdit)
        {
            InitializeComponent();
            _context = context;
            _currentUser = userToEdit;

            LoadUserData();
        }

        private void LoadUserData()
        {
            txtTitle.Text = $"Редактирование: {_currentUser.lastname} {_currentUser.firstname}";
            txtLastName.Text = _currentUser.lastname;
            txtFirstName.Text = _currentUser.firstname;
            txtMiddleName.Text = _currentUser.middlename;
            txtLogin.Text = _currentUser.login;

            // Устанавливаем роль
            foreach (ComboBoxItem item in cmbRole.Items)
            {
                if (int.Parse(item.Tag.ToString()) == _currentUser.userroleid)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            // Устанавливаем статус
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Tag.ToString() == _currentUser.status)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    ShowError("Введите фамилию сотрудника");
                    txtLastName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                {
                    ShowError("Введите имя сотрудника");
                    txtFirstName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMiddleName.Text))
                {
                    ShowError("Введите отчество сотрудника");
                    txtMiddleName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtLogin.Text))
                {
                    ShowError("Введите логин сотрудника");
                    txtLogin.Focus();
                    return;
                }

                // Проверка уникальности логина (кроме текущего пользователя)
                bool loginExists = _context.user.Any(u => u.login == txtLogin.Text.Trim() && u.userid != _currentUser.userid);
                if (loginExists)
                {
                    ShowError("Пользователь с таким логином уже существует");
                    txtLogin.Focus();
                    return;
                }

                // Получение выбранной роли
                var selectedRole = cmbRole.SelectedItem as ComboBoxItem;
                if (selectedRole == null)
                {
                    ShowError("Выберите роль сотрудника");
                    return;
                }

                int userRoleId = int.Parse(selectedRole.Tag.ToString());

                // Обновление данных
                _currentUser.lastname = txtLastName.Text.Trim();
                _currentUser.firstname = txtFirstName.Text.Trim();
                _currentUser.middlename = txtMiddleName.Text.Trim();
                _currentUser.login = txtLogin.Text.Trim();
                _currentUser.userroleid = userRoleId;

                // Обновление пароля (если указан)
                if (!string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    _currentUser.password = txtPassword.Password;
                }

                // Обновление статуса
                var selectedStatus = cmbStatus.SelectedItem as ComboBoxItem;
                if (selectedStatus != null)
                {
                    _currentUser.status = selectedStatus.Tag.ToString();
                }

                _context.SaveChanges();

                MessageBox.Show("Данные сотрудника успешно обновлены!", "Успех",
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

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
