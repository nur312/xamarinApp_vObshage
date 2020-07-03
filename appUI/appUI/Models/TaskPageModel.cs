using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace appUI.Models
{
    /// <summary>
    /// Модель страницы Modal Tasks.
    /// </summary>
    [Serializable]
    public class TaskPageModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Свойство для оповещения об изменениях.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Заголовок страницы.
        /// </summary>
        string pageTitle;
        /// <summary>
        /// Иконка страницы.
        /// </summary>
        string pageIcon;
        /// <summary>
        /// Дата выполнения.
        /// </summary>
        DateTime date;
        /// <summary>
        /// Время выполнения.
        /// </summary>
        TimeSpan time;
        /// <summary>
        /// Флаг важности страницы: важен или не важен.
        /// </summary>
        bool isImportant;
        /// <summary>
        /// Флаг уведомления. Включен или не включен.
        /// </summary>
        bool isRemind;
        /// <summary>
        /// Идентификатор уведомления.
        /// </summary>
        public int? NotificfId { get; set; } 
        /// <summary>
        /// Идентификатор данной страницы.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Заголовок страницы. Оповещает о изменениях.
        /// </summary>
        public string PageTitle
        {
            get => pageTitle;
            set {
                pageTitle = value;
                if (value != pageTitle)
                    PropertyChanged?.Invoke(
                        this, new PropertyChangedEventArgs(nameof(PageTitle)));
            }
        }
        /// <summary>
        /// Иконка страницы. Оповещает о изменениях.
        /// </summary>
        public string PageIcon
        {
            get => pageIcon;
            set {
                if (value != pageIcon)
                    SetProperty(ref pageIcon, value, nameof(PageIcon));
            }
        }
        /// <summary>
        /// Горит звезда или нет.
        /// </summary>
        [Ignore, JsonIgnore]
        public string StarIcon { get => IsImportant ? "star.png" : "nstar.png"; }
        /// <summary>
        /// Флаг важности страницы: важен или не важен.
        /// Оповещает о изменениях.
        /// </summary>
        public bool IsImportant
        {
            get => isImportant;
            set {
                if (value != isImportant)
                {
                    SetProperty(ref isImportant, value, nameof(IsImportant));
                    PropertyChanged?.Invoke(
                        this, new PropertyChangedEventArgs(nameof(StarIcon)));
                }
            }
        }
        /// <summary>
        /// Флаг уведомления. Включен или не включен.
        /// Оповещает о изменениях.
        /// </summary>
        public bool IsRemind
        {
            get => isRemind;
            set {
                if(value != isRemind)
                {
                    SetProperty(ref isRemind, value, nameof(IsRemind));
                    PropertyChanged?.Invoke(
                        this, new PropertyChangedEventArgs(nameof(RemindIcon)));
                }
            }
        }
        /// <summary>
        /// Включенный звонок или не включенный.
        /// </summary>
        [Ignore, JsonIgnore]
        public string RemindIcon
        {
            get => isRemind ? "bell.png" : "nbell.png";
        }
        /// <summary>
        /// Выполнены ли все задания.
        /// </summary>
        public bool IsDone { get; set; }
        /// <summary>
        /// Цвет текста.
        /// </summary>
        [Ignore, JsonIgnore]
        public Color Color
        {
            get {
                if (IsDone)
                    return Color.Green;
                else
                    return Color.DimGray;

            }
        }
        /// <summary>
        /// Дата выполнения. Оповещает об изменениях.
        /// </summary>
        public DateTime Date { get => date.Date;
            set {
                if(value != date)
                    SetProperty(ref date, value, nameof(Date));
            }
        }
        /// <summary>
        /// Время выполнения. Оповещает об изменениях.
        /// </summary>
        public TimeSpan Time {
            get => time;
            set {
                if(value != time)
                    SetProperty(ref time, value, nameof(Time));
            }
        }
        /// <summary>
        /// Задачи страницы.
        /// </summary>
        [Ignore, JsonIgnore]
        public ObservableCollection<TaskItem> ToDoList { get; set; }
        /// <summary>
        /// Задачи страницы в виде строки(Json).
        /// </summary>
        public string ListToString { get; set; }
        /// <summary>
        /// Устанавливает значения и оповещает об изменениях.
        /// </summary>
        /// <typeparam name="T">Обобщение для всех типов.</typeparam>
        /// <param name="property">Ссылка на свойство.</param>
        /// <param name="value">Значение для установки.</param>
        /// <param name="name">Имя свойства.</param>
        protected void SetProperty<T>(ref T property, T value, string name)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
