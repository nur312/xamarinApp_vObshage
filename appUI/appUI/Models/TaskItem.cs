using SQLite;
using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace appUI.Models
{
    /// <summary>
    /// Модель задачи.
    /// </summary>
    [Serializable]
    public class TaskItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Заголовок задания.
        /// </summary>
        string taskTitle;
        /// <summary>
        /// Флаг, показывающий выполнено или не выполнено задание.
        /// </summary>
        bool isDone;
        /// <summary>
        /// Идентификотор задания.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Заголовок задания. Оповещает об изменениях.
        /// </summary>
        public string TaskTitle {
            get => taskTitle;
            set {
                if(value != taskTitle)
                    SetProperty(ref taskTitle, value, nameof(TaskTitle));
            }
        }
        /// <summary>
        /// Флаг, показывающий выполнено или не выполнено задание.
        /// Оповещает об изменениях.
        /// </summary>
        public bool IsDone { get => isDone;
            set {
                if(isDone != value)
                {
                    SetProperty(ref isDone, value, nameof(IsDone));
                    PropertyChanged?.Invoke(
                        this, new PropertyChangedEventArgs(nameof(MyColor)));
                    PropertyChanged?.Invoke(
                        this, new PropertyChangedEventArgs(nameof(MyTextDecorations)));
                }
            }
        }
        /// <summary>
        /// Цвет текста.
        /// </summary>
        [Ignore, JsonIgnore]
        public Color MyColor { get =>IsDone ? Color.DarkGray : Color.DimGray; }
        /// <summary>
        /// Стиль текста: перечеркнутый или нормальный текст.
        /// </summary>
        [Ignore, JsonIgnore]
        public TextDecorations MyTextDecorations { get => IsDone ? TextDecorations.Strikethrough : TextDecorations.None;}
        /// <summary>
        /// Свойство для оповещения об изменениях.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
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
