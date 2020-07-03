using SQLite;

namespace appUI.Persistents
{
    /// <summary>
    /// Интерфей для работы с локальной базой данных.
    /// </summary>
    public interface ISQLite
    {
        /// <summary>
        /// Возвращает соединение для работы с базой данных.
        /// </summary>
        /// <param name="id">Имя файла базы данныхю</param>
        /// <returns></returns>
        SQLiteAsyncConnection GetConnectionPersonal(string id);
    }
}
