using appUI.pages;
using appUI.Persistents;
using Xamarin.Forms;

namespace appUI
{
    /// <summary>
    /// Класс, представляющий кроссплатформенное мобильное приложение.
    /// </summary>
    public partial class App : Application
    {
        readonly IFirebaseAuth auth;
        /// <summary>
        /// Установка начальных значений.
        /// </summary>
        public App()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseAuth>();
            if (auth.IsCurrentUser())
                MainPage = new NavigationPage(new MainPage());
            else
                MainPage = new NavigationPage(new AuthPage());
        }

        protected  override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
