using ScienceProgrammProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScienceProgrammProject.Core.Services
{
    public class ShiftService
    {
        private ScienceProgrammProjectEntities _context;

        public ShiftService(ScienceProgrammProjectEntities context)
        {
            _context = context;
        }

        // Получить текущую активную смену для пользователя
        public shift GetCurrentShiftForUser(int userId)
        {
            var today = DateTime.Today;

            var userShift = _context.userlist
                .Include("shift")
                .Where(ul => ul.userid == userId)
                .Where(ul => ul.shift.datestart <= today && ul.shift.dateend >= today)
                .Select(ul => ul.shift)
                .FirstOrDefault();

            return userShift;
        }

        // Проверить, есть ли у пользователя активная смена
        public bool HasActiveShift(int userId)
        {
            return GetCurrentShiftForUser(userId) != null;
        }

        // Получить заказы для активной смены пользователя
        public IQueryable<order> GetOrdersForUserShift(int userId)
        {
            var currentShift = GetCurrentShiftForUser(userId);
            if (currentShift == null)
                return Enumerable.Empty<order>().AsQueryable();

            // Находим пользователей на этой смене
            var shiftUserIds = _context.userlist
                .Where(ul => ul.shiftid == currentShift.shiftid)
                .Select(ul => ul.userid)
                .ToList();

            // Находим заказы, связанные с этими пользователями
            var orders = _context.orderuserlist
                .Include("order")
                .Where(oul => shiftUserIds.Contains(oul.userid))
                .Select(oul => oul.order)
                .Distinct();

            return orders;
        }

        // Добавить пользователя в смену
        public bool AddUserToShift(int userId, int shiftId)
        {
            try
            {
                // Проверяем, не назначен ли уже пользователь на эту смену
                var existingAssignment = _context.userlist
                    .FirstOrDefault(ul => ul.userid == userId && ul.shiftid == shiftId);

                if (existingAssignment != null)
                    return false; // Уже назначен

                var userList = new userlist
                {
                    userid = userId,
                    shiftid = shiftId
                };

                _context.userlist.Add(userList);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
