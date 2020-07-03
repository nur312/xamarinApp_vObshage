using System.Threading.Tasks;

namespace appUI.Persistents
{
    /// <summary>
    /// Интерфейс для работы с системой регистрации\авторизации.
    /// </summary>
    public interface IFirebaseAuth
    {
        /// <summary>
        /// Авторизация пользвателя.
        /// </summary>
        /// <param name="email">Электронная почта.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Токен</returns>
        Task<string> LoginWithEmailPassword(string email, string password);
        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="email">Электронная почта.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Токен</returns>
        Task<string> CreateWithEmailPassword(string email, string password);
        /// <summary>
        /// Опеделяет выполнил ли пользовтель вход.
        /// </summary>
        /// <returns>Результат.</returns>
        bool IsCurrentUser();
        /// <summary>
        /// Выполняет выход учетной записи из системы.
        /// </summary>
        void SignOut();
        /// <summary>
        /// Возвращает уникальный идентификатор пользоателя.
        /// </summary>
        string GetUserID();
    }
}
