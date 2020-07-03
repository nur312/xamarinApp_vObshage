using System;
using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using appUI.Droid.Persistents;
using appUI.Models;
using appUI.Persistents;
using Java.Lang;
using AndroidApp = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(LocalNotificationService))]

namespace appUI.Droid.Persistents
{
    /// <summary>
    /// Класс для оповещения пользователя.
    /// </summary>
    public class LocalNotificationService : ILocalNotificationService
    {
        /// <summary>
        /// Unix-время
        /// </summary>
        readonly DateTime _jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Для задания уникального идентификатора.
        /// </summary>
        internal string _randomNumber;
        /// <summary>
        /// Установка локальных уведомлений на устройстве.
        /// </summary>
        /// <param name="title">Заголовокю</param>
        /// <param name="body">Текст сообщения.</param>
        /// <param name="id">Идентификатор уведомления.</param>
        /// <param name="notifyTime">Время уведомленияю</param>
        /// <param name="repeateMs">Интервал повтора.</param>
        public void LocalNotification(string title, string body, int id, DateTime notifyTime, long repeateMs = 0)
        {
            long totalMilliSeconds = (long)(notifyTime.ToUniversalTime() - _jan1st1970).TotalMilliseconds;
            if (totalMilliSeconds < JavaSystem.CurrentTimeMillis())
            {
                totalMilliSeconds += repeateMs;
            }

            var intent = CreateIntent(id);
            var localNotification = new LocalNotification
            {
                Title = title,
                Body = body,
                Id = id,
                NotifyTime = notifyTime,
                IconId = Resource.Drawable.tick
            };

            var serializedNotification = SerializeNotification(localNotification);
            intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

            Random generator = new Random();
            _randomNumber = generator.Next(100000, 999999).ToString("D6");

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, Convert.ToInt32(_randomNumber), intent, PendingIntentFlags.Immutable);
            var alarmManager = GetAlarmManager();
            if (repeateMs == 0)
                alarmManager.Set(AlarmType.RtcWakeup, totalMilliSeconds, pendingIntent);
            else
                alarmManager.SetRepeating(AlarmType.RtcWakeup, totalMilliSeconds, repeateMs, pendingIntent);
        }
        /// <summary>
        /// Отмена уведомления.
        /// </summary>
        /// <param name="id">Идентификатор уведомления.</param>
        public void Cancel(int id)
        {
            var intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, Convert.ToInt32(_randomNumber), intent, PendingIntentFlags.Immutable);
            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.CancelAll();
            notificationManager.Cancel(id);
        }
        /// <summary>
        /// Возвращает "хороший" механизм для выполнения задач в переднем плане.
        /// </summary>
        public static Intent GetLauncherActivity() => 
            Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
        /// <summary>
        /// Возвращает  подзадачу, используется для показа уведомлений.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Intent CreateIntent(int id) => new Intent(Application.Context, typeof(ScheduledAlarmHandler))
                .SetAction("LocalNotifierIntent" + id);
        /// <summary>
        /// Возвращает системный сервис для управления сигналами. 
        /// </summary>
        private AlarmManager GetAlarmManager() => Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
        /// <summary>
        /// Сериализует уведомление.
        /// </summary>
        /// <param name="notification">Уведомление.</param>
        /// <returns>Сериализованное уведомление.</returns>
        private string SerializeNotification(LocalNotification notification)
        {
            var xmlSerializer = new XmlSerializer(notification.GetType());

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, notification);
                return stringWriter.ToString();
            }
        }
        /// <summary>
        /// Показ длительнго Toast уведомления.
        /// </summary>
        /// <param name="message">Сообщение для показа</param>
        public void LongAlert(string message) => Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        /// <summary>
        /// Показ кратковременного Toast уведомления.
        /// </summary>
        /// <param name="message">Сообщение для показа</param>
        public void ShortAlert(string message) => Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
    }
    /// <summary>
    /// Класс принимает уведомления.
    /// </summary>
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
    public class ScheduledAlarmHandler : BroadcastReceiver
    {
        /// <summary>
        /// Уведомление пользователя о событии.
        /// </summary>
        NotificationManager manager;
        /// <summary>
        /// Ключ-константа.
        /// </summary>
        public const string LocalNotificationKey = "LocalNotification";
        /// <summary>
        /// Идентификатор канала.
        /// </summary>
        const string channelId = "default";
        /// <summary>
        /// Имя канала.
        /// </summary>
        const string channelName = "Default";
        /// <summary>
        /// Описание канала.
        /// </summary>
        const string channelDescription = "The default channel for notifications.";
        /// <summary>
        /// Флаг инициализации канала.
        /// </summary>
        bool channelInitialized = false;
        /// <summary>
        /// Этот метод вызывается, когда ScheduledAlarmHandler получает механизм для вещания.
        /// </summary>
        /// <param name="context">Context в котором работает получатель.</param>
        /// <param name="intent">Полученный механизм(Intent)</param>
        public override void OnReceive(Context context, Intent intent)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);
            //Generating notification    
            var builder = new NotificationCompat.Builder(Application.Context, channelId)
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Body)
                .SetSmallIcon(Resource.Drawable.tick)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            var resultIntent = LocalNotificationService.GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);

            Random random = new Random();
            int randomNumber = random.Next(9999 - 1000) + 1000;

            var resultPendingIntent =
                stackBuilder.GetPendingIntent(randomNumber, (int)PendingIntentFlags.Immutable);
            builder.SetContentIntent(resultPendingIntent);
            // Sending notification    
            manager.Notify(randomNumber, builder.Build());
        }
        /// <summary>
        /// Создание канала для оповещения.
        /// </summary>
        void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }
            channelInitialized = true;
        }
        /// <summary>
        /// Десериализация оповещения.
        /// </summary>
        /// <param name="notificationString"></param>
        /// <returns></returns>
        private LocalNotification DeserializeNotification(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
            using (var stringReader = new StringReader(notificationString))
            {
                var notification = (LocalNotification)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        }
    }
}