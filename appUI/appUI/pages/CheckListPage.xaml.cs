using appUI.DataModels;
using appUI.IListeners;
using appUI.Models;
using appUI.Persistents;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appUI.pages
{
    /// <summary>
    /// Страницы командного чек-листа.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckListPage : ContentPage
    {
        /// <summary>
        /// Модель страницы.
        /// </summary>
        CheckList CurrentCheckList;
        /// <summary>
        /// Предоставляет доступ к онлайн базе данных.
        /// </summary>
        readonly FirebaseClient firebase;
        /// <summary>
        /// Интерфейс для оповещений.
        /// </summary>
        readonly ILocalNotificationService notification;
        /// <summary>
        /// Интерфейс для определения изменения данных.
        /// </summary>
        readonly IMyValueListener checListListener;
        /// <summary>
        /// URL онлайн базы данных.
        /// </summary>
        private static readonly string fbUrl =
            "https://vobshage-e86bd.firebaseio.com/";
        /// <summary>
        /// Название узла в онлайн базе данных Firbase
        /// Realtime database.
        /// </summary>
        readonly string checListsPath = "CheckLists";
        /// <summary>
        /// Задание начальных значений.
        /// </summary>
        /// <param name="checkList"></param>
        public CheckListPage(CheckList checkList)
        {
            InitializeComponent();
            firebase = new FirebaseClient(fbUrl);
            notification = DependencyService.Get<ILocalNotificationService>();
            checListListener = DependencyService.Get<IMyValueListener>();
            CurrentCheckList = checkList;

            MyDatePicker.MinimumDate = DateTime.Today;
            MyDatePicker.MinimumDate = DateTime.Today.AddSeconds(1);
            MyTimePicker.Time = new TimeSpan(9, 0, 1);
            BindingContext = CurrentCheckList;
        }
        /// <summary>
        /// Подписка на события.
        /// </summary>
        protected override void OnAppearing()
        {
            try
            {
                if (CurrentCheckList != null)
                {
                    if (CurrentCheckList.Tasks == null)
                        CurrentCheckList.Tasks = new ObservableCollection<TaskItem>();
                    string path = checListsPath + "/" + CurrentCheckList.Id;

                    checListListener.Subscribe(path);
                    checListListener.OnChange += TasksList_Refreshing;

                }
                else
                {
                    notification.LongAlert("Please reload check list");
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
            base.OnAppearing();
        }
        /// <summary>
        /// Сохраняет привязанный чек-лист в базе данных.
        /// </summary>
        async void SaveCurrentCheckList()
        {
            try
            {
                if (CurrentCheckList != null)
                {
                    await firebase.Child(checListsPath)
                        .Child(CurrentCheckList.Id).PutAsync(CurrentCheckList);
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Добавляет задачу в чек-лист.
        /// </summary>
        /// <param name="sender">Отправителью</param>
        /// <param name="e">Данные.</param>
        private async void PlusFab_Clicked(object sender, EventArgs e)
        {
            try
            {
                var title = await Input("New task", "Enter your task here", "Task", "Add");
                if (title == null)
                    return;
                var task = new TaskItem { TaskTitle = title };
                if (CurrentCheckList.Tasks == null)
                    CurrentCheckList.Tasks = new ObservableCollection<TaskItem>();
                CurrentCheckList.Tasks.Add(task);
                SaveCurrentCheckList();
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Метод для ввода строки.
        /// </summary>
        /// <param name="title">Заголовок.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="placeholder">Текст при пустом вводе.</param>
        /// <param name="accept">Текст принятия действий.</param>
        /// <param name="initialValue">Начальное значение.</param>
        /// <returns>Полученная строка.</returns>
        async Task<string> Input(string title, string message,
            string placeholder, string accept, string initialValue = "")
        {
            var temp = await DisplayPromptAsync(title, message, placeholder: placeholder,
               accept: accept, initialValue: initialValue);
            if (!string.IsNullOrEmpty(temp) && !string.IsNullOrWhiteSpace(temp))
            {
                temp = temp.Trim(' ');
                if (temp.Length == 1)
                    temp += " ";
                temp = char.ToUpper(temp[0]) + temp.Substring(1);
            }
            else if (temp != null)
            {
                await DisplayAlert("Empty input", "Please enter correct value", "Ok");
                temp = null;
            }

            return temp;
        }
        /// <summary>
        /// Метод срабатывает когда происходит нажатие на элемент списка.
        /// Нужно, чтобы цветом не выделялся выбранный элемент.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TasksList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            TasksList.SelectedItem = null;
        }
        /// <summary>
        /// Метод срабатывает когда происходит нажатие на элемент списка.
        /// Отмечает, что задание выполнено \ не выполнено.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private void TasksList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (e.Item as TaskItem);
            item.IsDone = !item.IsDone;
            SaveCurrentCheckList();
        }
        /// <summary>
        /// Метод для редактирования текста задачи.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private async void EditTask_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (TaskItem)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    string temp = await Input("Edit the task", "Please enter here",
                        "Task", "Save", item.TaskTitle);
                    if (temp != null)
                    {
                        item.TaskTitle = temp;
                        SaveCurrentCheckList();
                    }
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Метод для удаления задачи из листа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTask_Clicked(object sender, EventArgs e)
        {
            var item = (TaskItem)((MenuItem)sender)?.BindingContext;
            if (item != null)
            {
                CurrentCheckList.Tasks.Remove(item);
                SaveCurrentCheckList();
            }
        }
        /// <summary>
        /// Обновление листа.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private async void TasksList_Refreshing(object sender, EventArgs e)
        {
            try
            {
                if (CurrentCheckList != null)
                {
                    CurrentCheckList = await firebase.Child(checListsPath)
                        .Child(CurrentCheckList.Id).OnceSingleAsync<CheckList>();
                    TasksList.ItemsSource = CurrentCheckList.Tasks;
                    HeaderLbl.Text = CurrentCheckList.Title;
                    TasksList.IsRefreshing = false;
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Срабатывает после выбора даты.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private void MyDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            CurrentCheckList.Date = e.NewDate;
            SaveCurrentCheckList();
        }
        /// <summary>
        /// Срабатывает при закрытии окно выбора времени.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private void MyTimePicker_Unfocused(object sender, FocusEventArgs e)
        {
            var time = MyTimePicker.Time;
            if (time.Seconds == 0)
            {
                CurrentCheckList.Time = new TimeSpan(time.Hours, time.Minutes, 1);
                SaveCurrentCheckList();
            }
        }
        /// <summary>
        /// Копирование текста задачи.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private async void CopyId_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (TaskItem)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    await Clipboard.SetTextAsync(item.TaskTitle);
                }
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
    }
}