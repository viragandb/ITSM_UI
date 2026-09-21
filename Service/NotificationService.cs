using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class NotificationService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<NotificationLog> GetNotificationsByUser(string userId, string includeProperties)
        {
            return _contextUow.NotificationLogRepository.GetAsNoTracking(e => e.UserId == userId
            , includeProperties: includeProperties);

        }

        public IEnumerable<NotificationLog> GetNotificationsByUserAction(string userId,string controller,string action)
        {
            return _contextUow.NotificationLogRepository.Get(e => e.UserId == userId
            && e.NotificationCategory.Controller == controller
            && e.NotificationCategory.Action == action
            , includeProperties: "");

        }

        public bool InsertLog(string empNo, NotificationTypeEnum type, long eventId)
        {
            NotificationLog item = new NotificationLog();
            item.UserId = empNo;
            item.Type = type;
            item.NotificationCategoryId = (long)type;
            item.EventId = eventId;
            item.CreatedDate = DateTimeService.GetUserDate();

            try
            {
                _contextUow.NotificationLogRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void RemoveNotifications(string userId, string controller, string action)
        {
            var items = GetNotificationsByUserAction(userId, controller, action);
            foreach (NotificationLog log  in items)
            {
                DeleteLog(log);
            }
        }
        public bool DeleteLog(NotificationLog item)
        {

            try
            {
                _contextUow.NotificationLogRepository.Delete(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
