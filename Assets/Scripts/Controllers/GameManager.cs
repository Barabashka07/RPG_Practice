using System;
using Controllers.SaveLoad.PlayerSaves;
using Controllers.SaveLoad.Settings;
using UnityEngine;

namespace Controllers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindAnyObjectByType<GameManager>();
                if (_instance != null) return _instance;
                var go = new GameObject("InputManager");
                _instance = go.AddComponent<GameManager>();

                return _instance;
            }
        }

        private static GameManager _instance;


        private SettingsInteractor _settingsInteractor;
        private PlayerDataInteractor _playerDataInteractor;
        
        public PlayerDataInteractor GetPlayerDataInteractor() => _playerDataInteractor;
        public SettingsInteractor GetSettingsInteractor() => _settingsInteractor;
        //Сюда перекидываем все наши Bootstraper
        public void Init(SettingsInteractor settingsInteractor, PlayerDataInteractor playerDataInteractor)
        {
            _settingsInteractor = settingsInteractor;
            _playerDataInteractor = playerDataInteractor;
        }
        

        private void Awake()
        {
            //От дубдикатов, если есть уже то удаляем
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                //Если уже есть не удаляем
                DontDestroyOnLoad(gameObject);
            }
            
        }
        
    }
}