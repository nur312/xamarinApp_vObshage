using appUI.DataModels;
using appUI.Persistents;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appUI.pages
{
    /// <summary>
    /// Страница для работы с командами.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneralPage : ContentPage
    {
        /// <summary>
        /// URL онлайн базы данных.
        /// </summary>
        private static readonly string fbUrl = "https://vobshage-e86bd.firebaseio.com/";
        /// <summary>
        /// Предоставляет доступ к онлайн базе данных.
        /// </summary>
        readonly FirebaseClient firebase = new FirebaseClient(fbUrl);
        /// <summary>
        /// Интерфейс для работы с системой регистрации\авторизации.
        /// </summary>
        readonly IFirebaseAuth auth;
        /// <summary>
        /// Интерфейс для оповещения пользователя.
        /// </summary>
        private readonly ILocalNotificationService notification;
        /// <summary>
        /// Название узла команд в онлайн базе данных.
        /// </summary>
        const string teamPath = "Teams";
        /// <summary>
        /// Название узла пользователей в онлайн базе данных.
        /// </summary>
        const string usersPath = "Users";
        /// <summary>
        /// Текущий пользователь.
        /// </summary>
        User CurrentUser;
        /// <summary>
        /// Команды пользователя.
        /// </summary>
        ObservableCollection<Team> Teams;
        /// <summary>
        /// Задание начальных значений.
        /// </summary>
        public GeneralPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseAuth>();
            notification = DependencyService.Get<ILocalNotificationService>();
        }
        /// <summary>
        /// Задание "тяжелых" начальных значений.
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                CurrentUser = (await firebase.Child(usersPath).OnceAsync<User>())
                        .Where(a => a.Object.Id == auth.GetUserID()).First().Object;
                if (CurrentUser.TeamsId == null)
                    CurrentUser.TeamsId = new List<string>();
                var teams = await GetTeamsAsync(CurrentUser.TeamsId);
                Teams = new ObservableCollection<Team>(teams);
            }
            catch { notification.LongAlert("Please check internet connection."); }

            if (Teams == null)
                Teams = new ObservableCollection<Team>();
            MainList.ItemsSource = Teams;

            base.OnAppearing();
        }
        /// <summary>
        /// Получение команд из онлайн базы данных.
        /// </summary>
        /// <param name="ids">Идентификаторы команд.</param>
        /// <returns></returns>
        async Task<Team[]> GetTeamsAsync(List<string> ids)
        {
            Team[] teams = new Team[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                teams[i] = await firebase.Child(teamPath).Child(ids[i]).OnceSingleAsync<Team>();
            }
            return teams;
        }
        /// <summary>
        /// Добавление\создание команд.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PlusFab_Clicked(object sender, EventArgs e)
        {
            string act = await DisplayActionSheet("Select mode", "Cancel",
                null, "Join the team", "Create new team");
            if (act == "Join the team")
            {
                //Для присоединения к существующей команде.
                string id = await Input("Team ID", "Enter here secret team ID",
                    "Team ID", "Join");
                try
                {
                    if (id == null)
                        return;
                    var team = await firebase.Child(teamPath + "/" + id)
                        .OnceSingleAsync<Team>();

                    Teams.Add(team);

                    CurrentUser.TeamsId.Add(id);
                    await firebase.Child(usersPath).Child(CurrentUser.Token).PutAsync(CurrentUser);
                }
                catch
                {
                    notification.LongAlert("Incorrect id. Please try again");
                }
            }
            else if (act == "Create new team")
            {
                //Создание новой команды.
                try
                {
                    string name = await Input("Team name", "Enter here team name", "Team name", "Create new team");
                    if (name == null)
                        return;
                    var team = new Team
                    {
                        Name = name,
                        MembersToken = new List<string> { CurrentUser.Token }
                    };
                    team.Id = (await firebase.Child(teamPath).PostAsync(team)).Key;
                    await firebase.Child(teamPath).Child(team.Id).PutAsync(team);

                    Teams.Add(team);

                    CurrentUser.TeamsId.Add(team.Id);
                    await firebase.Child(usersPath).Child(CurrentUser.Token).PutAsync(CurrentUser);
                }
                catch
                {
                    notification.LongAlert("Please try again");
                }
            }
        }
        /// <summary>
        /// Метод для ввода строки.
        /// </summary>
        /// <param name="title">Заголовок.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="placeholder">Текст при пустом вводе.</param>
        /// <param name="accept">Текст принятия действий.</param>
        /// <param name="initial">Начальное значение.</param>
        /// <returns>Полученная строка.</returns>
        async Task<string> Input(string title, string message, string placeholder, string accept, string initial = "")
        {
            var temp = await DisplayPromptAsync(title, message, placeholder: placeholder,
               accept: accept, initialValue: initial);
            if (!string.IsNullOrEmpty(temp) && !string.IsNullOrWhiteSpace(temp))
            {
                temp = temp.Trim(' ');
                if (temp.Length == 1)
                    temp += " ";
                temp = char.ToUpper(temp[0]) + temp.Substring(1);
            }
            else if (temp != null)
            {
                await DisplayAlert("Empty input", "Please enter value", "Ok");
                temp = null;
            }

            return temp;
        }
        /// <summary>
        /// Выход из системы авторизации\регистрации.
        /// </summary>
        /// <param name="sender">Отправтель</param>
        /// <param name="e">Экземпляр с данными.</param>
        private void LogOut_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    auth.SignOut();
                    Application.Current.MainPage = new AuthPage();
                }
                else
                {
                    notification.LongAlert("Please check internet connection.");
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Метод срабатывает когда происходит нажатие на элемент списка.
        /// Нужно, чтобы цветом не выделялся выбранный элемент.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainList_ItemSelected(object sender, SelectedItemChangedEventArgs e) =>
            MainList.SelectedItem = null;
        /// <summary>
        /// Метод срабатывает когда происходит нажатие на элемент списка.
        /// Отмечает, что задание выполнено \ не выполнено.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private async void MainList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var item = e.Item as Team;
                await Navigation.PushAsync(new TeamsPage(CurrentUser, item));
            }
            else
            {
                notification.LongAlert("Please check internet connection.");
            }            
        }
        /// <summary>
        /// Метод для редактирования названия команды.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private async void TeamEdit_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (Team)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    string temp = await Input("Edit team's name", "Please enter here",
                        "Team name", "Save", item.Name);
                    if (temp != null)
                    {
                        item.Name = temp;
                        await firebase.Child(teamPath).Child(item.Id).PutAsync(item);
                    }
                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Метод для удаления команды.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private async void TeamDelete_Clicked(object sender, EventArgs e)
        {
            try
            {
                var item = (Team)((MenuItem)sender)?.BindingContext;
                if (item != null)
                {
                    CurrentUser.TeamsId.Remove(item.Id);
                    Teams.Remove(item);
                    await firebase.Child(usersPath).Child(CurrentUser.Token).PutAsync(CurrentUser);

                }
            }
            catch
            {
                notification.LongAlert("Please check internet connection.");
            }
        }
        /// <summary>
        /// Копирование идентификатора команды.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private async void CopyId_Clicked(object sender, EventArgs e)
        {
            var item = (Team)((MenuItem)sender)?.BindingContext;
            if (item != null)
            {
                await Clipboard.SetTextAsync(item.Id);
            }
        }
        /// <summary>
        /// Показ окна с подсказками.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Экземпляр с данными.</param>
        private void HelpTb_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage());
        }
    }
}