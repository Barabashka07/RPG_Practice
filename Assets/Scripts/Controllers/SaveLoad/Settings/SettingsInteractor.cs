using Controllers.SaveLoad.Saveables;

namespace Controllers.SaveLoad.Settings
{
    public class SettingsInteractor
    {
        // Уникальный "ключ", по которому мы будем искать настройки в базе
        private const string SettingsKey = "gameSettings";
        //Ссылка на репо
        private readonly IDataRepository _playerPrefsRepository;

        //Внедрение зависимостей, тут находится интерактор, который не создает репо, ему его передает Bootstrapper
        public SettingsInteractor(IDataRepository playerPrefsRepository)
        {
            _playerPrefsRepository = playerPrefsRepository;
        }

        //Ползунок GameSettings
        public void SaveSettings(GameSettings settings)
        {
            //Сохранение по ключу 
            _playerPrefsRepository.Save(SettingsKey, settings);
        }

        public GameSettings LoadSettings()
        {
            //Если настроек нет, то даем базовые настройки
            return _playerPrefsRepository.Load(SettingsKey,new GameSettings());
        }

        public bool HasSettings()
        {
            return _playerPrefsRepository.HasKey(SettingsKey);
        }

        public void DeleteSettings()
        {
            _playerPrefsRepository.Delete(SettingsKey);
        }
        
    }
}