using Android.App;
using Firebase;
using Firebase.Database;

namespace appUI.Droid.Listeners
{
    /// <summary>
    /// Класс для работы с базой данных Firebase Realtime Database.
    /// </summary>
    public static class AppDataHelper
    {
        /// <summary>
        /// Возвращает экземпляр для работы с базой данных Firebase Realtime Database.
        /// </summary>
        /// <returns></returns>
        public static FirebaseDatabase GetDatabase()
        {

            var app = FirebaseApp.InitializeApp(Application.Context);
            FirebaseDatabase database;

            if (app == null)
            {
                var option = new Firebase.FirebaseOptions.Builder()
                    .SetApplicationId("vobshage-e86bd")
                    .SetApiKey("AIzaSyDGPBtwHsbTwZxZfp-CmA-7wX3YUUA6jgs")
                    .SetDatabaseUrl("https://vobshage-e86bd.firebaseio.com")
                    .SetStorageBucket("vobshage-e86bd.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, option);
                database = FirebaseDatabase.GetInstance(app);
            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

            return database;
        }
    }
}