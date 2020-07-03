using System;
using System.IO;
using appUI.Droid.Persistents;
using appUI.Persistents;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDb))]
namespace appUI.Droid.Persistents
{
    /// <summary>
    /// Класс для получения доступа к локальной базе данных.
    /// </summary>
    public class SQLiteDb : ISQLite
    {
        /// <summary>
        /// Путь для сохранения файла базы данных.
        /// </summary>
        readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        /// <summary>
        /// Возвращает соединение для работы с базой данных.
        /// </summary>
        /// <param name="id">Имя файла базы данныхю</param>
        /// <returns></returns>
        public SQLiteAsyncConnection GetConnectionPersonal(string id) =>
            new SQLiteAsyncConnection(Path.Combine(path, id +".db3"));
    }
}