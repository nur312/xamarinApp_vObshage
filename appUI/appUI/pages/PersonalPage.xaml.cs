using appUI.Models;
using appUI.Persistents;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appUI.pages
{
    /// <summary>
    /// Сраница с персональными чек-листами.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalPage : ContentPage
    {
        /// <summary>
        /// Предостовляет асинхронное соединение с локальной базой данных.
        /// </summary>
        readonly SQLiteAsyncConnection sqConnection;
        /// <summary>
        /// Интерфейс для работы с системой регистрации\авторизации.
        /// </summary>
        readonly IFirebaseAuth auth;
        /// <summary>
        /// Интерфейс для оповещения пользователя.
        /// </summary>
        readonly ILocalNotificationService notification;
        /// <summary>
        /// Модели чек-листов.
        /// </summary>
        public ObservableCollection<TaskPageModel> Models { get; private set; } 
        /// <summary>
        /// Задание начальных значений.
        /// </summary>
        public PersonalPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseAuth>();
            sqConnection = DependencyService.Get<ISQLite>().
                GetConnectionPersonal(auth.GetUserID());
            if (Models == null)
                Models = new ObservableCollection<TaskPageModel>();
            notification = DependencyService.Get<ILocalNotificationService>();
        }
        /// <summary>
        /// Задание "тяжелых" начальных значений.
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                await sqConnection.CreateTableAsync<TaskPageModel>();
                await sqConnection.CreateTableAsync<TaskItem>();
                Models = new ObservableCollection<TaskPageModel>(
                    await sqConnection.Table<TaskPageModel>().ToArrayAsync());
                if (Models == null)
                    Models = new ObservableCollection<TaskPageModel>();
                MainList.ItemsSource = Models;
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
            base.OnAppearing();
        }
        /// <summary>
        /// Добавление персонального чек-листа.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        async void PlusFab_Clicked(object sender, EventArgs e)
        {
            try
            {
                var temp = await DisplayPromptAsync("New title", "Enter list's title", placeholder: "Enter here",
                accept: "Create new list");
                if (!string.IsNullOrEmpty(temp) && !string.IsNullOrWhiteSpace(temp))
                {
                    temp = temp.Trim(' ');
                    temp = char.ToUpper(temp[0]) + temp.Substring(1);
                    var obj = new TaskPageModel { PageTitle = temp, Date = DateTime.Today };
                    await sqConnection.InsertAsync(obj);
                    Models.Add(obj);
                }
                else if (temp != null)
                    await DisplayAlert("Empty title", "Try enter correct title", "Ok");
                All_Clicked(null, null);
                if (Models.Count > 0)
                    MainList.ScrollTo(Models[Models.Count - 1], ScrollToPosition.End, true);
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Показ страницы с чек-листом.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        async private void MainList_ItemTapped(object sender, ItemTappedEventArgs e) =>
            await Navigation.PushAsync(new ModalTasks((TaskPageModel)e.Item, sqConnection), false);
        /// <summary>
        /// Устанавливает приоритетность чек-листу.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Important_Clicked(object sender, EventArgs e)
        {
            var temp = (TaskPageModel)((MenuItem)sender)?.BindingContext;
            temp.IsImportant = !temp.IsImportant;
        }
        /// <summary>
        /// Удаление чек-листа.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        async private void Delete_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (TaskPageModel)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    await sqConnection.DeleteAsync(item);
                    Models.Remove(item);
                }
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Редактирование названия чек-листа.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        async private void Edit_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (TaskPageModel)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    var temp = await DisplayPromptAsync("Complete", "Edit your list title", initialValue: $"{item.PageTitle}");
                    if (!string.IsNullOrEmpty(temp) && !string.IsNullOrWhiteSpace(temp))
                    {
                        temp = temp.Trim(' ');
                        item.PageTitle = char.ToUpper(temp[0]) + temp.Substring(1);
                        await sqConnection.UpdateAsync(item);
                    }
                }
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Показывает сегодняшние чек-листы.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        async private void MyDay_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Task.Run(MyDaySort);
                MainList.ItemsSource = result;
                ObservableCollection<TaskPageModel> MyDaySort()
                {
                    var temp = new ObservableCollection<TaskPageModel>();
                    foreach (var item in Models)
                        if (item.Date == DateTime.Today)
                        {
                            temp.Add(item);
                        }
                    return temp;
                }
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Покзывает приоритетные чек-листы.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>/// <param name="sender"></param>
        async private void ImportantLists_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await Task.Run(MyDaySort);
                MainList.ItemsSource = result;
                ObservableCollection<TaskPageModel> MyDaySort()
                {
                    var temp = new ObservableCollection<TaskPageModel>();
                    foreach (var item in Models)
                        if (item.IsImportant)
                            temp.Add(item);
                    return temp;
                }
            }
            catch
            {
                notification.LongAlert("Please try again.");
            }
        }
        /// <summary>
        /// Показывает все чек-листы.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        private void All_Clicked(object sender, EventArgs e) 
            => MainList.ItemsSource = Models;
        /// <summary>
        /// Метод срабатывает когда происходит нажатие на элемент списка.
        /// Нужно, чтобы цветом не выделялся выбранный элемент.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        private void MainList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
            => MainList.SelectedItem = null;
        /// <summary>
        /// Показывает страницу с подсказками.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Данные.</param>
        private void HelpTb_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage());
        }
    }
}