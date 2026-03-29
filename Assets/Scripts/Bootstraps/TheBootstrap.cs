using Controllers;
using Controllers.SaveLoad;
using Controllers.SaveLoad.PlayerSaves;
using Controllers.SaveLoad.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bootstraps
{
    public class TheBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            //Создаем менеджер 
            var gamemanager = new GameObject("GameManager").AddComponent<GameManager>();
            //сохрнение настроек в реестр
            var playerPrefs = new PlayerPrefsRepository();
            //сохранение прогресса в .json
            var jsonRepository = new JsonRepository("PlayerData");
            var playerDataInteractor = new PlayerDataInteractor(jsonRepository);
            var settings = new SettingsInteractor(playerPrefs);
            gamemanager.Init(settings, playerDataInteractor);
            //Запуск меню
            SceneManager.LoadScene("MainMenu");
        }
    }
}