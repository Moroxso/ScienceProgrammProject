using ScienceProgrammProject.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScienceProgrammProject.Core.DataScripts
{
    public class LocalDatabaseService : IDatabaseService
    {
        private ScienceProgrammProjectEntities _context;
        public bool IsConnected { get; private set; }
        public ScienceProgrammProjectEntities Context => _context;

        public void Initialize()
        {
            try
            {
                // Используем стандартную строку подключения
                _context = new ScienceProgrammProjectEntities();

                if (TestConnectionWithRetry())
                {
                    IsConnected = true;
                    System.Diagnostics.Debug.WriteLine("Успешное подключение к LocalDB");
                    return;
                }

                CreateDatabase();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации LocalDB: {ex.Message}");
                IsConnected = false;
            }
        }

        private bool TestConnectionWithRetry()
        {
            string[] connectionStrings = {
                @"metadata=res://*/ScienceProgrammProject.csdl|res://*/ScienceProgrammProject.ssdl|res://*/ScienceProgrammProject.msl;provider=System.Data.SqlClient;provider connection string=""data source=(localdb)\MSSQLLocalDB;initial catalog=ScienceProgrammProject;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework""",
                @"data source=(localdb)\MSSQLLocalDB;initial catalog=ScienceProgrammProject;integrated security=True;MultipleActiveResultSets=True",
            };

            foreach (string connString in connectionStrings)
            {
                try
                {
                    _context = new ScienceProgrammProjectEntities();
                    _context.Database.Connection.ConnectionString = connString;

                    if (_context.Database.Exists() && CheckDatabaseStructure())
                    {
                        return true;
                    }
                }
                catch
                {
                    _context?.Dispose();
                    _context = null;
                }
            }
            return false;
        }

        private bool CheckDatabaseStructure()
        {
            try
            {
                // Проверяем существование основных таблиц
                _context.Database.ExecuteSqlCommand("SELECT TOP 1 1 FROM [user]");
                _context.Database.ExecuteSqlCommand("SELECT TOP 1 1 FROM [order]");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CreateDatabase()
        {
            try
            {
                string connectionString = @"data source=(localdb)\MSSQLLocalDB;initial catalog=ScienceProgrammProject;integrated security=True;MultipleActiveResultSets=True";

                _context = new ScienceProgrammProjectEntities();
                _context.Database.Connection.ConnectionString = connectionString;

                if (!_context.Database.Exists())
                {
                    _context.Database.Create();
                    CreateTablesManually();
                    SeedInitialData();
                }

                IsConnected = true;
                System.Diagnostics.Debug.WriteLine("База данных успешно создана");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания базы: {ex.Message}");
                IsConnected = false;
            }
        }

        private void CreateTablesManually()
        {
            try
            {
                // SQL скрипты создания таблиц для нашего проекта
                string[] createScripts = {
                    @"CREATE TABLE [userrole] (
                        [userroleid] INT IDENTITY(1,1) PRIMARY KEY,
                        [namerole] NVARCHAR(50) NOT NULL
                    )",

                    @"CREATE TABLE [user] (
                        [userid] INT IDENTITY(1,1) PRIMARY KEY,
                        [login] NVARCHAR(50) NOT NULL UNIQUE,
                        [password] NVARCHAR(50) NOT NULL,
                        [status] NVARCHAR(50) NULL,
                        [lastname] NVARCHAR(50) NOT NULL,
                        [firstname] NVARCHAR(50) NOT NULL,
                        [middlename] NVARCHAR(50) NOT NULL,
                        [userroleid] INT NOT NULL,
                        CONSTRAINT [FK_user_userrole] FOREIGN KEY ([userroleid]) REFERENCES [userrole]([userroleid])
                    )",

                    @"CREATE TABLE [shift] (
                        [shiftid] INT IDENTITY(1,1) PRIMARY KEY,
                        [datestart] DATE NOT NULL,
                        [dateend] DATE NOT NULL
                    )",

                    @"CREATE TABLE [userlist] (
                        [userlistid] INT IDENTITY(1,1) PRIMARY KEY,
                        [userid] INT NOT NULL,
                        [shiftid] INT NOT NULL,
                        CONSTRAINT [FK_userlist_user] FOREIGN KEY ([userid]) REFERENCES [user]([userid]),
                        CONSTRAINT [FK_userlist_shift] FOREIGN KEY ([shiftid]) REFERENCES [shift]([shiftid])
                    )",

                    @"CREATE TABLE [order] (
                        [orderid] INT IDENTITY(1,1) PRIMARY KEY,
                        [datecreation] DATE NOT NULL,
                        [orderstatus] NVARCHAR(50) NOT NULL,
                        [paymentstatus] NVARCHAR(50) NOT NULL,
                        [nameconference] NVARCHAR(50) NOT NULL,
                        [equipment] NVARCHAR(50) NOT NULL,
                        [amountguests] INT NOT NULL
                    )",

                    @"CREATE TABLE [orderuserlist] (
                        [orderuserlistid] INT IDENTITY(1,1) PRIMARY KEY,
                        [userid] INT NOT NULL,
                        [orderid] INT NOT NULL,
                        CONSTRAINT [FK_orderuserlist_user] FOREIGN KEY ([userid]) REFERENCES [user]([userid]),
                        CONSTRAINT [FK_orderuserlist_order] FOREIGN KEY ([orderid]) REFERENCES [order]([orderid])
                    )"
                };

                foreach (var script in createScripts)
                {
                    try
                    {
                        _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, script);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка выполнения скрипта: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания таблиц: {ex.Message}");
            }
        }

        private void SeedInitialData()
        {
            try
            {
                // Добавляем роли
                if (!_context.userrole.Any())
                {
                    _context.userrole.Add(new userrole { namerole = "Заведующий подразделением" });
                    _context.userrole.Add(new userrole { namerole = "Организатор" });
                    _context.userrole.Add(new userrole { namerole = "Техник" });
                    _context.SaveChanges();
                }

                // Добавляем тестовых пользователей
                if (!_context.user.Any())
                {
                    var managerRole = _context.userrole.First(r => r.namerole == "Заведующий подразделением");
                    var organizerRole = _context.userrole.First(r => r.namerole == "Организатор");
                    var technicianRole = _context.userrole.First(r => r.namerole == "Техник");

                    _context.user.Add(new user
                    {
                        login = "admin",
                        password = "admin123",
                        lastname = "Иванов",
                        firstname = "Иван",
                        middlename = "Иванович",
                        userroleid = managerRole.userroleid
                    });

                    _context.user.Add(new user
                    {
                        login = "organizer",
                        password = "org123",
                        lastname = "Петрова",
                        firstname = "Мария",
                        middlename = "Сергеевна",
                        userroleid = organizerRole.userroleid
                    });

                    _context.user.Add(new user
                    {
                        login = "technician",
                        password = "tech123",
                        lastname = "Сидоров",
                        firstname = "Алексей",
                        middlename = "Владимирович",
                        userroleid = technicianRole.userroleid
                    });

                    _context.SaveChanges();
                }

                // Добавляем тестовые заказы
                if (!_context.order.Any())
                {
                    _context.order.Add(new order
                    {
                        datecreation = DateTime.Now.Date,
                        orderstatus = "принят",
                        paymentstatus = "принят",
                        nameconference = "Тестовая конференция",
                        equipment = "проектор",
                        amountguests = 50
                    });

                    _context.SaveChanges();
                }

                System.Diagnostics.Debug.WriteLine("Тестовые данные успешно добавлены");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка добавления тестовых данных: {ex.Message}");
            }
        }

        public void TestConnection()
        {
            IsConnected = _context?.Database?.Exists() ?? false;
        }

        public void CreateBackup()
        {
            System.Diagnostics.Debug.WriteLine("Функция бэкапа не реализована");
        }

        public void RestoreFromBackup(string backupPath)
        {
            System.Diagnostics.Debug.WriteLine("Функция восстановления не реализована");
        }
    }
}
