using System.Collections.Generic;

namespace Controllers.SaveLoad
{
    //Тут наследование от базового IDataRepository, там лежат обычные ключи сохранений
    public interface IPlayerDataRepository : IDataRepository
    {
        //Загрузка именно по времени при помощи timestamp
        public T Load<T>(string key, string timestamp, T defaultValue = default);
        //Тут мы плучаем лист-список всех сохранений
        public List<string> GetAllTimestamps();
    }
}