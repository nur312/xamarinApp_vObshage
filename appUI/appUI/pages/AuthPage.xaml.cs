using appUI.DataModels;
using appUI.Persistents;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appUI.pages
{
    /// <summary>
    /// Страница для регистрации\авторизации пользователя.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthPage : ContentPage
    {
        /// <summary>
        /// Интерфейс для взаимодейтвия с Firebase Authentication SDK.
        /// </summary>
        readonly IFirebaseAuth auth;
        /// <summary>
        /// Узел пользователей в базе данных Firebase Realtime Database.
        /// </summary>
        readonly string userPath = "Users";
        /// <summary>
        /// Интерфейс для оповещений.
        /// </summary>
        private readonly ILocalNotificationService notificationManager;
        /// <summary>
        /// Ссылка для доступа к базе данных.
        /// </summary>
        private static readonly string fbUrl = "https://vobshage-e86bd.firebaseio.com/";
        /// <summary>
        /// Предоставляет доступ к онлайн базе данных.
        /// </summary>
        readonly FirebaseClient firebase;
        /// <summary>
        /// Установка начальных значений
        /// </summary>
        public AuthPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseAuth>();
            notificationManager = DependencyService.Get<ILocalNotificationService>();
            firebase  = new FirebaseClient(fbUrl);
        }
        /// <summary>
        /// Вход и регистрация в системе Firebase Authentication SDK.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Входные данные.</param>
        private async void LoginBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                string Token;
                if (LoginBtn.Text == "Login")
                {
                    Token = await auth.LoginWithEmailPassword(EmailInput.Text, PasswordInput.Text);
                    if (Token != "")
                    {
                        Application.Current.MainPage = new MainPage();
                    }
                    else
                    {
                        notificationManager.LongAlert("E-mail or password are incorrect.Try again!");
                    }
                }
                else
                {
                    Token = await auth.CreateWithEmailPassword(EmailInput.Text, PasswordInput.Text);

                    if (Token != "")
                    {
                        var user = new User { Name = EmailInput.Text.Split('@')[0], Id = auth.GetUserID(), TeamsId = new List<string>() };

                        user.Token = (await firebase.Child(userPath).PostAsync(user)).Key;

                        await firebase.Child(userPath).Child(user.Token).PutAsync(user);

                        Application.Current.MainPage = new MainPage();
                    }
                    else
                    {
                        notificationManager.LongAlert("E-mail or password are incorrect. Password shoud be longer. Try again! ");
                    }
                }
            }
            catch
            {
                notificationManager.LongAlert("Problems with Internet connectivity. Please try again.");
            }
        }
        /// <summary>
        /// Меняет функции авторизации\входа.
        /// </summary>
        /// <param name="sender">Отправель.</param>
        /// <param name="e">Входные данные.</param>
        private void NewAccountBtn_Clicked(object sender, EventArgs e)
        {
            var str = LoginBtn.Text;
            LoginBtn.Text = NewAccountBtn.Text;
            NewAccountBtn.Text = str;
        }
    }
}