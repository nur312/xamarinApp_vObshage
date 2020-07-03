using appUI.Droid.Listeners;
using appUI.Droid.Persistents;
using appUI.IListeners;
using appUI.Listeners;
using Firebase.Database;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(CheckListListener))]
namespace appUI.Listeners
{
    /// <summary>
    /// Класс для получения изменений на подписанный узел.
    /// </summary>
    public class CheckListListener : Java.Lang.Object, IValueEventListener, IMyValueListener
    {
        /// <summary>
        /// Событие для получения уведомлений об измении в определенном узле базы данных.
        /// </summary>
        public event EventHandler OnChange;
        public void OnCancelled(DatabaseError error)
        {

        }
        /// <summary>
        /// Получение оповещения об изменении данных.
        /// </summary>
        /// <param name="snapshot">Обновленные данные.</param>
        public void OnDataChange(DataSnapshot snapshot)
        {
            OnChange?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Устанавливает данный объект к узлу в базе данных, чтобы он оповещал о изменениях.
        /// </summary>
        /// <param name="path"></param>
        public void Subscribe(string path)
        {
            try
            {
                DatabaseReference teamsRef = AppDataHelper.GetDatabase().GetReference(path);
                teamsRef.AddValueEventListener(this);
            }
            catch {
                var notification = new LocalNotificationService();
                notification.LongAlert("Please check your internet connection.");
            }
        }
    }
}
