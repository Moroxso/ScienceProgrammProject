using ScienceProgrammProject.Core.Services;
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
    /// Логика взаимодействия для AssignToShiftWindow.xaml
    /// </summary>
    public partial class AssignToShiftWindow : Window
    {
        private ScienceProgrammProjectEntities _context;
        private shift _currentShift;
        private List<user> _availableUsers;
        private List<userlist> _shiftUsers;

        public AssignToShiftWindow(ScienceProgrammProjectEntities context, shift selectedShift)
        {
            InitializeComponent();
            _context = context;
            _currentShift = selectedShift;

            InitializeWindow();
            LoadData();
        }

        private void InitializeWindow()
        {
            txtTitle.Text = $"Назначение сотрудников на смену";
            txtShiftInfo.Text = $"Смена #{_currentShift.shiftid} ({_currentShift.datestart:dd.MM.yyyy} - {_currentShift.dateend:dd.MM.yyyy})";
        }

        private void LoadData()
        {
            try
            {
                // Загружаем доступных сотрудников (активные, не уволенные)
                _availableUsers = _context.user
                    .Include("userrole")
                    .Where(u => string.IsNullOrEmpty(u.status) || u.status != "уволен")
                    .ToList();

                // Загружаем сотрудников уже назначенных на эту смену
                _shiftUsers = _context.userlist
                    .Include("user")
                    .Include("user.userrole")
                    .Where(ul => ul.shiftid == _currentShift.shiftid)
                    .ToList();

                // Убираем из доступных тех, кто уже на смене
                var shiftUserIds = _shiftUsers.Select(ul => ul.userid).ToList();
                _availableUsers = _availableUsers.Where(u => !shiftUserIds.Contains(u.userid)).ToList();

                // Обновляем списки
                lstAvailableUsers.ItemsSource = _availableUsers;
                lstShiftUsers.ItemsSource = _shiftUsers;

                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            try
            {
                int total = _shiftUsers.Count;
                int organizers = _shiftUsers.Count(ul => ul.user.userroleid == 2); // Организаторы
                int technicians = _shiftUsers.Count(ul => ul.user.userroleid == 3); // Техники

                txtTotalCount.Text = total.ToString();
                txtOrganizersCount.Text = organizers.ToString();
                txtTechniciansCount.Text = technicians.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статистики: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddToShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null) return;

                int userId = (int)button.Tag;
                var userToAdd = _availableUsers.FirstOrDefault(u => u.userid == userId);

                if (userToAdd != null)
                {
                    // ПРОВЕРКА: Не назначен ли уже пользователь на другую активную смену
                    var shiftService = new ShiftService(_context);
                    var currentShift = shiftService.GetCurrentShiftForUser(userId);
                    if (currentShift != null && currentShift.shiftid != _currentShift.shiftid)
                    {
                        MessageBox.Show($"Сотрудник уже назначен на активную смену ({currentShift.datestart:dd.MM.yyyy} - {currentShift.dateend:dd.MM.yyyy})",
                                      "Конфликт смен", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Создаем новую запись о назначении на смену
                    var newUserList = new userlist
                    {
                        userid = userId,
                        shiftid = _currentShift.shiftid
                    };

                    _context.userlist.Add(newUserList);
                    _context.SaveChanges();

                    LoadData();

                    MessageBox.Show($"Сотрудник {userToAdd.lastname} {userToAdd.firstname} добавлен на смену",
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveFromShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null) return;

                int userListId = (int)button.Tag;
                var userListToRemove = _context.userlist.Find(userListId);

                if (userListToRemove != null)
                {
                    var result = MessageBox.Show("Вы уверены, что хотите удалить сотрудника из смены?",
                                               "Подтверждение удаления",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.userlist.Remove(userListToRemove);
                        _context.SaveChanges();

                        // Перезагружаем данные
                        LoadData();

                        MessageBox.Show("Сотрудник удален из смены", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
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
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
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
