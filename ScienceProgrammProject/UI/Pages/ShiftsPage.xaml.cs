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
    /// Логика взаимодействия для ShiftsPage.xaml
    /// </summary>
    public partial class ShiftsPage : Page
    {
        private user _currentUser;
        private ScienceProgrammProjectEntities _context;

        public ShiftsPage(user currentUser, ScienceProgrammProjectEntities context)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _context = context;
            LoadShifts();
        }

        private void LoadShifts()
        {
            var shifts = _context.shift
                .Include("userlist")
                .Include("userlist.user")
                .ToList();

            ShiftsItemsControl.ItemsSource = shifts;
            txtNoShifts.Visibility = shifts.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void btnCreateShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаем новую смену на следующие 7 дней
                var newShift = new shift
                {
                    datestart = DateTime.Today,
                    dateend = DateTime.Today.AddDays(7)
                };

                _context.shift.Add(newShift);
                _context.SaveChanges();

                LoadShifts();
                MessageBox.Show("Новая смена создана", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании смены: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AssignEmployees(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int shiftId = (int)button.Tag;

            var shift = _context.shift.Find(shiftId);
            if (shift != null)
            {
                var assignWindow = new AssignToShiftWindow(_context, shift);
                assignWindow.Owner = Application.Current.MainWindow;
                assignWindow.Closed += (s, args) => LoadShifts(); // Обновляем список после закрытия
                assignWindow.ShowDialog();
            }
        }

        private void DeleteShift(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int shiftId = (int)button.Tag;

            var shift = _context.shift.Find(shiftId);
            if (shift != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту смену?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.shift.Remove(shift);
                    _context.SaveChanges();
                    LoadShifts();
                    MessageBox.Show("Смена удалена", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
