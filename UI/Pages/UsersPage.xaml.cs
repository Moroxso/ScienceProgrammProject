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
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;

        public UsersPage(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
            LoadUsers();
            UpdateStatusColors();
        }

        private void LoadUsers()
        {
            var users = _context.user
                .Include("userrole")
                .ToList();

            UsersItemsControl.ItemsSource = users;
            txtNoUsers.Visibility = users.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateStatusColors()
        {
            // Обновляем цвета статусов
            foreach (var item in UsersItemsControl.Items)
            {
                if (item is user user)
                {
                    // Найдем Border для этого пользователя и установим цвет
                    // Это упрощенная версия - в реальном приложении лучше использовать конвертеры
                }
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow(_context);
            addUserWindow.Owner = Application.Current.MainWindow;
            addUserWindow.Closed += (s, args) => LoadUsers(); // Обновляем список после закрытия
            addUserWindow.ShowDialog();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void FireEmployee(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int userId = (int)button.Tag;

            var user = _context.user.Find(userId);
            if (user != null && string.IsNullOrEmpty(user.status))
            {
                var result = MessageBox.Show($"Вы уверены, что хотите уволить {user.lastname} {user.firstname}?",
                                           "Подтверждение увольнения",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    user.status = "уволен";
                    _context.SaveChanges();
                    LoadUsers();
                    MessageBox.Show("Сотрудник уволен", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (user?.status == "уволен")
            {
                MessageBox.Show("Этот сотрудник уже уволен", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditEmployee(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int userId = (int)button.Tag;

            MessageBox.Show($"Редактирование сотрудника ID: {userId} будет реализовано позже",
                          "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
