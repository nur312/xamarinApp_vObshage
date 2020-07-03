using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using appUI.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace appUI.DataModels
{
    /// <summary>
    /// Модель чек-листа.
    /// </summary>
    public class CheckList : INotifyPropertyChanged
    {
        /// <summary>
        /// Оповещает об изменеии свойств.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Название узла в онлайн базе данных Firebase database.
        /// </summary>
        public static readonly string resourseName = "CheckLists";
        /// <summary>
        /// Заголовок чек-листа.
        /// </summary>
        string title;
        /// <summary>
        /// Идентификатор чек-листа.
        /// </summary>
        string id;
        /// <summary>
        /// Дата чек-лиса.
        /// </summary>
        DateTime date;
        /// <summary>
        /// Время чек-листа.
        /// </summary>
        TimeSpan time;
        /// <summary>
        /// Свойство доступа к title. Оповещает об изменениях.
        /// </summary>
        public string Title {
            get => title;
            set {
                if(value != title)
                {
                    title = value;
                    SetProperty();
                }
            }
        }
        /// <summary>
        /// Свойство доступа к id. Оповещает об изменениях.
        /// </summary>
        public string Id {
            get => id;
            set {
                if (value != id)
                {
                    id = value;
                    SetProperty();
                }
            }
        }
        /// <summary>
        /// Свойство доступа к date. Оповещает об изменениях.
        /// </summary>
        public DateTime Date {
            get => date;
            set {
                if (value != date)
                {
                    date = value;
                    SetProperty();
                }
            }
        }
        /// <summary>
        /// Свойство доступа к time. Оповещает об изменениях.
        /// </summary>
        public TimeSpan Time {
            get => time;
            set {
                if (value != time)
                {
                    time = value;
                    SetProperty();
                }
            }
        }
        /// <summary>
        /// Задачи чек-личта. Оповещает об изменениях.
        /// </summary>
        public ObservableCollection<TaskItem> Tasks { get; set; }
        /// <summary>
        /// Флаг уведомления.
        /// </summary>
        bool isRemind;
        /// <summary>
        /// Свойство доступа к isRemind. Оповещает об изменениях.
        /// </summary>
        [JsonIgnore]
        public bool IsRemind
        {
            get => isRemind;
            set {
                if (value != isRemind)
                {
                    isRemind = value;
                    SetProperty();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemindIcon)));
                }
            }
        }
        /// <summary>
        /// Возвращает картинку в зависимости от состояния уведомлений.
        /// </summary>
        [JsonIgnore]
        public string RemindIcon
        {
            get => isRemind ? "bell.png" : "nbell.png";
        }
        /// <summary>
        /// Идентификатор уведомлений.
        /// </summary>
        public int? NotificfId { get; set; }
        /// <summary>
        /// Оповещение об изменеии определенного свойства.
        /// </summary>
        /// <param name="name">Имя свойства.</param>
        protected void SetProperty([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            //await firebase.Child(resourseName).Child(Id).PutAsync(this);

        }
    }
}
