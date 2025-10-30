using ScienceProgrammProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScienceProgrammProject.Core.DataScripts
{
    public static class DatabaseManager
    {
        private static IDatabaseService _currentDatabaseService;
        public static DatabaseType CurrentDatabaseType { get; private set; }

        public enum DatabaseType
        {
            SqlServer,
            LocalDB
        }

        public static void Initialize()
        {
            // Сначала пробуем существующую базу, потом создаем локальную
            var databaseServices = new (IDatabaseService service, DatabaseType type)[]
            {
                (new LocalDatabaseService(), DatabaseType.LocalDB)
            };

            foreach (var (service, type) in databaseServices)
            {
                try
                {
                    service.Initialize();
                    if (service.IsConnected)
                    {
                        _currentDatabaseService = service;
                        CurrentDatabaseType = type;
                        System.Diagnostics.Debug.WriteLine($"Успешное подключение: {type}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка {type}: {ex.Message}");
                }
            }

            throw new Exception("Не удалось подключиться к базе данных");
        }

        public static ScienceProgrammProjectEntities GetContext()
        {
            if (_currentDatabaseService == null)
            {
                throw new InvalidOperationException("DatabaseManager не инициализирован.");
            }
            return _currentDatabaseService.Context;
        }

        public static bool IsConnected()
        {
            return _currentDatabaseService?.IsConnected ?? false;
        }

        public static void CreateBackup()
        {
            _currentDatabaseService?.CreateBackup();
        }

        public static void RestoreFromBackup(string backupPath)
        {
            _currentDatabaseService?.RestoreFromBackup(backupPath);
        }
    }
}
