using appUI.Models;
using appUI.Persistents;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using Xamarin.Essentials;

namespace appUI.pages
{
    /// <summary>
    /// Страница персонального чек-листа.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModalTasks : ContentPage
    {
        /// <summary>
        /// Предостовляет асинхронное соединение с локальной базой данных.
        /// </summary>
        readonly SQLiteAsyncConnection connection;
        /// <summary>
        /// Интерфейс для оповещения пользователя.
        /// </summary>
        private readonly ILocalNotificationService notification;
        /// <summary>
        /// Модель страницы.
        /// </summary>
        public TaskPageModel Model { get; private set; }
        /// <summary>
        /// Задание начальных значений.
        /// </summary>
        /// <param name="pageModel"></param>
        /// <param name="connection"></param>
        public ModalTasks(TaskPageModel pageModel, SQLiteAsyncConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Model = pageModel;
            MyDatePicker.MinimumDate = DateTime.Today.AddSeconds(1);
            MyTimePicker.Time = new TimeSpan(9, 0, 1);
            BindingContext = Model;
            notification = DependencyService.Get<ILocalNotificationService>();

        }
        /// <summary>
        /// Задание "тяжелых" начальных значений.
        /// </summary>
        protected override void OnAppearing()
        {
            if (Model.ListToString != null)
            {
                try
                {
                    Model.ToDoList = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(Model.ListToString);
                }
                catch
                {
                    notification.LongAlert("Data is corrupted, please add new tasks.");
                    Model.ToDoList = new ObservableCollection<TaskItem>();
                }
            }
            else
            {
                Model.ToDoList = new ObservableCollection<TaskItem>();
            }
            TasksList.ItemsSource = Model.ToDoList;
            base.OnAppearing();
        }
        /// <summary>
        /// Отметка\снятие галочки о выполнении. 
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private async void TasksList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var item = (e.Item as TaskItem);
                item.IsDone = !item.IsDone;
                Model.ListToString = JsonSerializer.Serialize(Model.ToDoList);
                if (!Model.ToDoList.Any(el => el.IsDone == false))
                    Model.IsDone = true;
                else
                    Model.IsDone = false;
                await connection.UpdateAsync(Model);
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Метод для редактирования текста задачи.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        async private void Edit_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (TaskItem)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    var temp = await DisplayPromptAsync("Complete", "edit your notes", initialValue: $"{item.TaskTitle}");
                    if (!string.IsNullOrEmpty(temp) && !string.IsNullOrWhiteSpace(temp))
                    {
                        temp = temp.Trim(' ');
                        item.TaskTitle = temp;
                        Model.ListToString = JsonSerializer.Serialize(Model.ToDoList);
                        await connection.UpdateAsync(Model);
                    }
                }
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }

        }
        /// <summary>
        /// Метод для удаления задачи из листа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (TaskItem)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    Model.ToDoList.Remove(item);
                    Model.ListToString = JsonSerializer.Serialize(Model.ToDoList);
                    await connection.UpdateAsync(Model);
                }
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Добавляет задачу в чек-лист.
        /// </summary>
        /// <param name="sender">Отправителью</param>
        /// <param name="e">Данные.</param>
        async private void Fabplus_Clicked(object sender, EventArgs e)
        {
            try
            {
                var temp = await DisplayPromptAsync("New task", "Enter task title", accept: "Create task",
                    placeholder: "Title");
                if (!string.IsNullOrEmpty(temp) && !string.IsNullOrWhiteSpace(temp))
                {
                    temp = temp.Trim(' ');
                    Model.ToDoList.Add(new TaskItem { TaskTitle = temp });
                    TasksList.ScrollTo(Model.ToDoList[Model.ToDoList.Count - 1], ScrollToPosition.End, true);
                    Model.ListToString = JsonSerializer.Serialize(Model.ToDoList);
                    await connection.UpdateAsync(Model);

                }
                else if (temp != null)
                    notification.LongAlert("Empty title. Please try enter correct title");
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Включение\отключение уведомлений.
        /// </summary>
        /// <param name="sender">Отправителью</param>
        /// <param name="e">Данные.</param>
        private async void RemaindTb_Clicked(object sender, EventArgs e)
        {
            try
            {
                Model.IsRemind = !Model.IsRemind;
                if (Model.IsRemind)
                {
                    var date = new DateTime(Model.Date.Year, Model.Date.Month, Model.Date.Day, Model.Time.Hours, Model.Time.Minutes, 0);
                    if (date < DateTime.Now)
                    {
                        Model.IsRemind = false;
                        notification.LongAlert("Please select correct time");
                        return;
                    }
                    if (Model.NotificfId == null)
                        Model.NotificfId = Guid.NewGuid().GetHashCode();
                    MyDatePicker.IsEnabled = false;
                    MyTimePicker.IsEnabled = false;
                    notification.LocalNotification("Remainder", Model.PageTitle, (int)Model.NotificfId, date);
                    notification.ShortAlert("Remind you at " + date.ToString("hh:mm tt, MMM dd"));
                }
                else
                {
                    MyDatePicker.MinimumDate = DateTime.Today.AddSeconds(1);
                    MyTimePicker.Time = new TimeSpan(9, 0, 1);
                    notification.Cancel((int)Model.NotificfId);
                    MyDatePicker.IsEnabled = true;
                    MyTimePicker.IsEnabled = true;

                }
                await connection.UpdateAsync(Model);
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Устанавливает приоритетность чек-листа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StarTb_Clicked(object sender, EventArgs e)
        {
            try
            {
                Model.IsImportant = !Model.IsImportant;
                await connection.UpdateAsync(Model);
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Метод срабатывает когда происходит нажатие на элемент списка.
        /// Нужно, чтобы цветом не выделялся выбранный элемент.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TasksList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
            => TasksList.SelectedItem = null;
        /// <summary>
        /// Копирует текст задания.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CopyId_Clicked(object sender, EventArgs e)
        {
            var item = (TaskItem)((MenuItem)sender)?.BindingContext;
            if (item != null)
            {
                await Clipboard.SetTextAsync(item.TaskTitle);
            }
        }
    }
}