using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

namespace appUI.Droid
{
    /// <summary>
    /// Загрузка Android приложения.
    /// </summary>
    [Activity(Label = "vObshage", Icon = "@drawable/reminder", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// Загрузке приложения.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Firebase.FirebaseApp.InitializeApp(Application.Context);

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            LoadApplication(new App());
        }
        /// <summary>
        /// Запрос разрешений.
        /// </summary>
        /// <param name="requestCode">Код запроса</param>
        /// <param name="permissions">Разренения</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}