using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace appUI.DataModels
{
    /// <summary>
    /// Модель пользоветля.
    /// </summary>
    [Serializable]
    public class User : INotifyPropertyChanged
    {
        /// <summary>
        /// Оповещает об изменеии свойств.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Название узла в базе данных Firebase Realtime Database.
        /// </summary>
        public static readonly string resourseName = "Users";
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        string id;
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        string name;
        /// <summary>
        /// Идентификаторы команд пользователя.
        /// </summary>
        public List<string> TeamsId { get; set; }
        /// <summary>
        /// Свойство для доступа к name. Оповещает об изменениях.
        /// </summary>
        public string Name
        {
            get => name;
            set {
                if (value != name)
                {
                    name = value;
                    SetProperty();
                }
            }
        }
        /// <summary>
        /// Токен пользователя.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Свойство для доступа к id. Оповещает об изменениях.
        /// </summary>
        public string Id
        {
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
        /// Оповещает об изменеии свойств. Оповещает об изменениях.
        /// </summary>
        protected void SetProperty([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
