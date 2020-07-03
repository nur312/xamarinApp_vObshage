using System;

namespace appUI.Models
{
    /// <summary>
    /// Класс для локальных уведомлений.
    /// </summary>
    public class LocalNotification
    {
        /// <summary>
        /// Заголовок уведомления.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Текст уведомления.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Идентификатор уведомления.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Индекс иконки уведомления.
        /// </summary>
        public int IconId { get; set; }
        /// <summary>
        /// Дата и время уведомления.
        /// </summary>
        public DateTime NotifyTime { get; set; }
    }
}
