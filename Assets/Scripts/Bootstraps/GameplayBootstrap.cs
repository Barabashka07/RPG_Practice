using System;
using System.Collections;
using Anims;
using Controllers;
using Controllers.Entities;
using Controllers.Entities.HealthController;
using Controllers.SaveLoad.PlayerSaves;
using Controllers.SaveLoad.Saveables;
using Controllers.Scenes;
using Controllers.UI;
using Enemy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using Views.Gameplay;
using Weapons.Base;

namespace Bootstraps
{
    public class GameplayBootstrap : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Camera _camera;
        [SerializeField] private CooldownView _cooldownView;
        [SerializeField] private HealthBarView _healthBarView;
        [SerializeField] private InputManager _inputManager;
        private GameObject _player;
        private HealthSystem _playerHealthSystem;
        private PlayerDataInteractor _playerDataInteractor;
  


        [Header("Enemies")]
        [SerializeField] private GameObject[] _enemies;
        [SerializeField] private Vector2 _enemiesSpawnAreaExtents;
        [SerializeField] private int _enemiesCount;
        [SerializeField] private Transform _enemiesSpawnPoint;
       

        [Header("Boss")]
        [SerializeField] private GameObject _boss;
        [SerializeField] private Vector3 _bossSpawnPoint;
        

        private void Awake()
        {
            //Спрятан курсор
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            var settngsInteractor = GameManager.Instance.GetSettingsInteractor();
            var gameplayManager = gameObject.AddComponent<GameplayManager>();
            var viewManager = GetComponent<ViewManager>();
            _playerDataInteractor = GameManager.Instance.GetPlayerDataInteractor();
            //var uiManager = new GameplayUIManager(_playerDataInteractor, viewManager);
            SetUpPlayer();
            //Настройка UI кд способностей
            var skillsController = _player.GetComponent<SkillsController>();
            skillsController.Init(_camera);
            _cooldownView.SetMeleeListener(() =>
                _cooldownView.SetMeleeFillAmount(skillsController.Skills[SkillType.Melee].GetReadyPercent()));
            _cooldownView.SetSpellListener(() =>
                _cooldownView.SetSpellFillAmount(skillsController.Skills[SkillType.Fireball].GetReadyPercent()));
            _bossSpawnPoint = new Vector3(323.6f,261.71f, -103.5f);
            

            //Сохранение позиции врагов
            var enemyManager = new EnemyManager(settngsInteractor,
                _enemiesSpawnAreaExtents,
                _enemies,
                _enemiesSpawnPoint.position,
                _enemiesCount,
                _boss, _bossSpawnPoint,_player.GetComponent<CharacterController>());
            gameplayManager.Init(_inputManager, _playerHealthSystem, viewManager, _playerDataInteractor, enemyManager);
            enemyManager.LoadEnemies(_playerDataInteractor.CurrentSave.Enemies); // Восстанавливаем врагов
            
        }
        
        
        private void SetUpPlayer()
        {   
            // Спавн игрока
            Debug.Log(_playerDataInteractor.CurrentSave.Position);
            _player = Instantiate(_playerPrefab, _playerSpawnPoint.position, Quaternion.identity);
            Debug.Log(_player.transform.position);
            // Настройка здоровья и привязка полоски (UI)
            _playerHealthSystem = _player.GetComponent<HealthSystem>();
            // healthBarView.Init(_playerHealthSystem.onHealthChanged);
            _playerHealthSystem.onHealthChanged.AddListener(_healthBarView.ChangeHp);
            _playerHealthSystem.Init(1);
            _player.GetComponent<MovementController>().Init(_camera);
            _playerHealthSystem.SetHealth(_playerDataInteractor.CurrentSave.Health);
      
            if (_playerDataInteractor.CurrentSave.Position == default) return;
            //_player.transform.position = _playerDataInteractor.CurrentSave.Position;
            /*Vector3 position = _playerDataInteractor.CurrentSave.Position;
            position.y += 3f;
            _player.transform.position = position;*/
            Debug.Log(_player.transform.position);
            //Блокируем через Invoke смену позиции при загрузке сейва
            Invoke(nameof(SetPos),.05f);
            Invoke(nameof(LogPos), 1f);
        }

        private void LogPos()
        {
            Debug.Log(_player.transform.position);
        }
        
        private void SetPos()
        {
            _player.transform.position = _playerDataInteractor.CurrentSave.Position;
        }
        
    }
}