using System;

namespace appUI.IListeners
{
    /// <summary>
    /// Интерфейс для подписки на событие - изменение данных.
    /// </summary>
    public interface IMyValueListener
    {
        event EventHandler OnChange;
        void Subscribe(string path);
    }
}
