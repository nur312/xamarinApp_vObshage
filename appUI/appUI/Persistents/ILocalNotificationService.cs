using System;

namespace appUI.Persistents
{
    /// <summary>
    /// Интерфейс для оповещения пользователя.
    /// </summary>
    public interface ILocalNotificationService
    {
        /// <summary>
        /// Установка локальных уведомлений на устройстве.
        /// </summary>
        /// <param name="title">Заголовокю</param>
        /// <param name="body">Текст сообщения.</param>
        /// <param name="id">Идентификатор уведомления.</param>
        /// <param name="notifyTime">Время уведомленияю</param>
        /// <param name="repeateMs">Интервал повтора.</param>
        void LocalNotification(string title, string body, int id, DateTime notifyTime, long repeat = 0);
        /// <summary>
        /// Отмена уведомления.
        /// </summary>
        /// <param name="id">Идентификатор уведомления.</param>
        void Cancel(int id);
        /// <summary>
        /// Показ длительнго Toast уведомления.
        /// </summary>
        /// <param name="message">Сообщение для показа</param>
        void LongAlert(string message);
        /// <summary>
        /// Показ кратковременного Toast уведомления.
        /// </summary>
        /// <param name="message">Сообщение для показа</param>
        void ShortAlert(string message);
    }
}
