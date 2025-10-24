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
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        private ScienceProgrammProjectEntities _context;
        private Random _random;

        public AddUserWindow(ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _context = context;
            _random = new Random();
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(chars[_random.Next(chars.Length)]);
            }

            return password.ToString();
        }

        private void btnGeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string generatedPassword = GenerateRandomPassword();
            txtPassword.Password = generatedPassword;

            MessageBox.Show($"Сгенерирован пароль: {generatedPassword}\n\nНе забудьте сообщить его сотруднику!",
                          "Пароль сгенерирован",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
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

                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    ShowError("Введите пароль сотрудника");
                    txtPassword.Focus();
                    return;
                }

                // Проверка уникальности логина
                bool loginExists = _context.user.Any(u => u.login == txtLogin.Text.Trim());
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

                int userRoleId;
                if (!int.TryParse(selectedRole.Tag.ToString(), out userRoleId))
                {
                    ShowError("Ошибка при определении роли");
                    return;
                }

                // Создание нового сотрудника
                var newUser = new user
                {
                    lastname = txtLastName.Text.Trim(),
                    firstname = txtFirstName.Text.Trim(),
                    middlename = txtMiddleName.Text.Trim(),
                    login = txtLogin.Text.Trim(),
                    password = txtPassword.Password,
                    userroleid = userRoleId,
                    status = null // Активный сотрудник
                };

                _context.user.Add(newUser);
                _context.SaveChanges();

                string roleName = selectedRole.Content.ToString();
                MessageBox.Show($"Сотрудник {newUser.lastname} {newUser.firstname} успешно добавлен!\n\n" +
                               $"Логин: {newUser.login}\n" +
                               $"Пароль: {txtPassword.Password}\n" +
                               $"Роль: {roleName}",
                               "Сотрудник добавлен",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        // Обработчик для нажатия Enter в текстовых полях
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // Переход к следующему полю
                var element = sender as System.Windows.UIElement;
                if (element != null)
                {
                    element.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
                }
            }
        }
    }
}
