using appUI.DataModels;
using appUI.Persistents;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using appUI.IListeners;
using System.Collections.ObjectModel;
using Firebase.Database.Query;
using Xamarin.Essentials;

namespace appUI.pages
{
    /// <summary>
    /// Страница с чек-листами.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TeamsPage : ContentPage
    {
        /// <summary>
        /// Текущий пользователь.
        /// </summary>
        readonly User CurrentUser;
        /// <summary>
        /// Текущая команда.
        /// </summary>
        Team CurrentTeam;
        /// <summary>
        /// Интерфейс для работы с системой регистрации\авторизации.
        /// </summary>
        readonly FirebaseClient firebase;
        /// <summary>
        /// Интерфейс для оповещения пользователя.
        /// </summary>
        readonly ILocalNotificationService notification;
        /// <summary>
        /// Интерфейс для определения изменения данных.
        /// </summary>
        readonly IMyValueListener checListsListener;
        /// <summary>
        /// URL онлайн базы данных.
        /// </summary>
        private static readonly string fbUrl =
            "https://vobshage-e86bd.firebaseio.com/";
        /// <summary>
        /// Название узла чек-листов в онлайн базе данных Firbase
        /// Realtime database.
        /// </summary>
        readonly string checListsPath = "CheckLists";
        /// <summary>
        /// Название узла команд в онлайн базе данных Firbase
        /// Realtime database.
        /// </summary>
        readonly string teamsPath = "Teams";
        /// <summary>
        /// Чек-листы команды.
        /// </summary>
        public ObservableCollection<CheckList> CheckLists { get; set; }
        /// <summary>
        /// Задает начальные значения.
        /// </summary>
        /// <param name="user">Текущий пользователь.</param>
        /// <param name="team">Текущая команда.</param>
        public TeamsPage(User user, Team team)
        {
            InitializeComponent();
            firebase = new FirebaseClient(fbUrl);
            notification = DependencyService.Get<ILocalNotificationService>();
            checListsListener = DependencyService.Get<IMyValueListener>();
            CurrentUser = user;
            CurrentTeam = team;
            Title = "Checklists";
            HeaderLabel.Text = CurrentTeam.Name;

        }
        /// <summary>
        /// Задание "тяжелых" начальных значений.
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                if (CurrentTeam != null && CurrentUser != null)
                {
                    CheckLists = new ObservableCollection<CheckList>(await GetListsAsync());
                    MainList.ItemsSource = CheckLists;
                    string path = teamsPath + "/" + CurrentTeam.Id;
                    checListsListener.Subscribe(path);
                    checListsListener.OnChange += async (s, e) =>
                    {
                        CurrentTeam = await firebase.Child(path).OnceSingleAsync<Team>();
                        CheckLists = new ObservableCollection<CheckList>(await GetListsAsync());
                        MainList.ItemsSource = CheckLists;
                    };
                }
            }
            catch
            {
                CheckLists = new ObservableCollection<CheckList>();
                notification.LongAlert("Please check internet connection.");
            }
            base.OnAppearing();
        }
        /// <summary>
        /// Получение чек-листов из онлайн базы данных по их идентификаторам.
        /// </summary>
        /// <returns>Чек-листы.</returns>
        async Task<CheckList[]> GetListsAsync()
        {
            if (CurrentTeam.CheckListsId != null)
            {
                CheckList[] checkLists = new CheckList[CurrentTeam.CheckListsId.Count];
                for (int i = 0; i < CurrentTeam.CheckListsId.Count; i++)
                {
                    checkLists[i] = await firebase.Child(checListsPath).
                        Child(CurrentTeam.CheckListsId[i]).OnceSingleAsync<CheckList>();
                }
                return checkLists;
            }
            else
                return new CheckList[0];
        }
        /// <summary>
        /// Добавление нового чек-листа.
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private async void PlusFab_Clicked(object sender, EventArgs e)
        {
            try
            {
                string title = await Input("New check list title", "Enter your title", "Title", "Create new check list");
                if (title == null)
                    return;
                var checkList = new CheckList { Title = title };
                checkList.Id = (await firebase.Child(checListsPath).PostAsync(checkList)).Key;
                await firebase.Child(checListsPath).Child(checkList.Id).PutAsync(checkList);
                if (CurrentTeam.CheckListsId == null)
                    CurrentTeam.CheckListsId = new List<string>();
                CurrentTeam.CheckListsId.Add(checkList.Id);
                CheckLists.Add(checkList);

                await firebase.Child(teamsPath).Child(CurrentTeam.Id).PutAsync(CurrentTeam);
            }
            catch
            {
                notification.LongAlert("Please check your internet connection.");
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
        async Task<string> Input(string title, string message, string placeholder,
            string accept, string initialValue = "")
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
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private void MainList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MainList.SelectedItem = null;
        }
        /// <summary>
        /// Показ страницы с чек-листом.
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private async void MainList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var item = e.Item as CheckList;
                await Navigation.PushAsync(new CheckListPage(item));
            }
            else
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Редактирование названия чек-листа.
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private async void EditCheckList_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (CheckList)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    string temp = await Input("Edit the check list", "Please enter here", "Check list title", "Save", item.Title);
                    if (temp != null)
                    {
                        item.Title = temp;
                        await firebase.Child(checListsPath).Child(item.Id).PutAsync(item);
                    }
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Удаление чек-листа.
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private async void DeleteCheckList_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (CheckList)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    CheckLists.Remove(item);
                    CurrentTeam.CheckListsId.Remove(item.Id);
                    await firebase.Child(teamsPath).Child(CurrentTeam.Id).PutAsync(CurrentTeam);
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Обновление чек-листов.
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private async void MainList_Refreshing(object sender, EventArgs e)
        {
            try
            {
                if (CheckLists != null)
                {
                    CheckLists = new ObservableCollection<CheckList>(await GetListsAsync());
                    MainList.ItemsSource = CheckLists;
                    Title = CurrentTeam.Name;
                    MainList.IsRefreshing = false;
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Копирование идентификтора команды.
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Данные</param>
        private async void CopyId_Clicked(object sender, EventArgs e)
        {
            if (CurrentTeam != null)
            {
                await Clipboard.SetTextAsync(CurrentTeam.Id);
            }
        }
    }
}