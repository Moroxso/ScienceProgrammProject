using ScienceProgrammProject.Core.DataScripts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ScienceProgrammProject
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Инициализируем базу данных
                DatabaseManager.Initialize();

                if (!DatabaseManager.IsConnected())
                {
                    MessageBox.Show("Не удалось подключиться к базе данных. Приложение будет закрыто.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }

                // Запускаем главное окно
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска приложения: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
