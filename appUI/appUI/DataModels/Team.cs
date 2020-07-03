using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace appUI.DataModels
{
    /// <summary>
    /// Модель команды.
    /// </summary>
    public class Team : INotifyPropertyChanged
    {
        /// <summary>
        /// Название узла в онлайн базе данных Firebase database.
        /// </summary>
        public static readonly string resourseName = "Teams";
        /// <summary>
        /// Идентификатор команды.
        /// </summary>
        string id;
        /// <summary>
        /// Имя команды.
        /// </summary>
        string name;
        /// <summary>
        /// Идентификатор-токены членов команды.
        /// </summary>
        public List<string> MembersToken { get; set; }
        /// <summary>
        /// Идентификатор чек-листов команды.
        /// </summary>
        public List<string> CheckListsId { get; set; }
        /// <summary>
        /// Свойство для доступа name. Оповещает об изменениях.
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetProperty([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
