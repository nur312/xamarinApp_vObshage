using System.Threading.Tasks;
using appUI.Droid.Persistents;
using appUI.Persistents;
using Firebase.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(MyFirebaseAuth))]

namespace appUI.Droid.Persistents
{
    /// <summary>
    /// Класс для регистрации\авторзации пользователей.
    /// </summary>
    public  class MyFirebaseAuth : IFirebaseAuth
    {
        /// <summary>
        /// Авторизация пользвателя.
        /// </summary>
        /// <param name="email">Электронная почта.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Токен</returns>
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                return token.Token;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="email">Электронная почта.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Токен</returns>
        public async Task<string> CreateWithEmailPassword(string email, string password)
        {
            try
            {
                var user = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                return token.Token;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Определяет выполнил ли пользователь вход.
        /// </summary>
        /// <returns>Результат.</returns>
        public bool IsCurrentUser() => FirebaseAuth.Instance.CurrentUser != null;
        /// <summary>
        /// Возвращает уникальный идентификатор пользователя.
        /// </summary>
        public string GetUserID() => FirebaseAuth.Instance.CurrentUser.Uid;
        /// <summary>
        /// Выполняет выход учетной записи из системы.
        /// </summary>
        public void SignOut()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
            }
            catch {
                var notification = new LocalNotificationService();
                notification.LongAlert("Please check your internet connection.");
            }
        }
    }
}