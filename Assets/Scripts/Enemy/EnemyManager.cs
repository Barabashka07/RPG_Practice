// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Bootstraps;
// using Controllers.Entities;
// using Controllers.Entities.HealthController;
// using Controllers.SaveLoad.Saveables;
// using Controllers.SaveLoad.Settings;
// using UnityEngine;
// using Views.Gameplay;
// using Object = UnityEngine.Object;
// using Random = UnityEngine.Random;
//
// namespace Enemy
// {
//     public class EnemyManager
//     {
//         private SettingsInteractor _settingsInteractor;
//         private GameObject[] _enemyPrefabs;
//         private Vector2 _enemiesSpawnAreaExtents;
//         private Vector3 _enemiesSpawnAreaCenter;
//         private int _enemiesCount;
//         private List<EnemyController> _enemies = new();
//         private List<BossController> _boss = new();
//
//         private GameObject _bossPrefab;
//         private Vector3 _bossSpawnPoint;
//         
//         public EnemyManager(SettingsInteractor settingsInteractor, Vector2 enemiesSpawnAreaSize, GameObject[] enemyPrefabs, Vector3 enemiesSpawnAreaCenter, int enemiesCount, GameObject _boss, Vector3 bossSpawnPoint)
//         {
//             _settingsInteractor = settingsInteractor;
//             _enemiesSpawnAreaExtents = enemiesSpawnAreaSize;
//             _enemyPrefabs = enemyPrefabs;
//             _enemiesSpawnAreaCenter = enemiesSpawnAreaCenter;
//             _enemiesCount = enemiesCount;
//             _bossPrefab = _boss;
//             SpawnEnemies(_enemiesCount);
//             _bossSpawnPoint = bossSpawnPoint;
//         }
//         
//         
//         public void SpawnEnemies(int enemiesCount)
//         {   
//             
//                  
//             var enemiesPower = _settingsInteractor.LoadSettings().EnemiesPower;
//             for (int i = 0; i < enemiesCount; i++)
//             {
//                 int prefabIndex = Random.Range(0, _enemyPrefabs.Length);
//
//                 var enemyPrefab = _enemyPrefabs[prefabIndex];
//                 var enemy = Object.Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity)
//                     .GetComponent<EnemyController>();
//                 var healthSystem = enemy.GetComponent<HealthSystem>();
//                 healthSystem.onHealthChanged.AddListener(enemy.GetComponentInChildren<HealthBarView>().ChangeHp);
//                 healthSystem.Init(enemiesPower);
//                 healthSystem.onDeath.AddListener((_) => OnEnemyDeath());
//                 enemy.GetComponent<SkillsController>().Init(null);
//                 enemy.UniqueId = System.Guid.NewGuid().ToString(); // Уникальный ID
//                 enemy.PrefabIndex = prefabIndex; // Сохраняем индекс префаба
//
//                 _enemies.Add(enemy);
//
//                 
//                 // healthSystem.onDeath.AddListener((isDead) => CheckAllEnemiesDead());
//             }
//         }
//         
//         public List<EnemyData> GetEnemyData()
//         {
//             List<EnemyData> enemyDataList = new List<EnemyData>();
//             foreach (var enemy in _enemies)
//             {
//                 if (enemy != null && !enemy.isDead) // Сохраняем только живых врагов
//                 {
//                     enemyDataList.Add(new EnemyData
//                     {
//                         Id = enemy.UniqueId,
//                         Position = enemy.transform.position,
//                         Health = enemy.GetComponent<HealthSystem>().Health, // Предполагается, что HealthSystem имеет CurrentHealth
//                         PrefabIndex = enemy.PrefabIndex
//                     });
//                 }
//             }
//             return enemyDataList;
//         }
//
//         public void LoadEnemies(List<EnemyData> enemyDataList)
//         {   
//             if (enemyDataList.Count == 0) return;
//             // Очищаем существующих врагов
//             foreach (var enemy in _enemies)
//             {
//                 if (enemy != null)
//                 {
//                     Object.Destroy(enemy.gameObject);
//                 }
//             }
//             _enemies.Clear();
//             
//             // Создаём врагов из сохранённых данных
//
//             var enemiesPower = _settingsInteractor.LoadSettings().EnemiesPower;
//             foreach (var enemyData in enemyDataList)
//             {   
//                 if (enemyData.PrefabIndex >= 0 && enemyData.PrefabIndex < _enemyPrefabs.Length)
//                 {
//                     var enemyPrefab = _enemyPrefabs[enemyData.PrefabIndex];
//                     var enemy = Object.Instantiate(enemyPrefab, enemyData.Position, Quaternion.identity)
//                         .GetComponent<EnemyController>();
//                     enemy.UniqueId = enemyData.Id;
//                     enemy.PrefabIndex = enemyData.PrefabIndex;
//                     
//                     var healthSystem = enemy.GetComponent<HealthSystem>();
//
//                     healthSystem.onHealthChanged.AddListener(enemy.GetComponentInChildren<HealthBarView>().ChangeHp);
//                     healthSystem.Init(enemiesPower);
//                     healthSystem.onDeath.AddListener((_) => OnEnemyDeath());
//                     healthSystem.SetHealth(enemyData.Health);
//                     enemy.GetComponent<SkillsController>().Init(null);
//                     _enemies.Add(enemy);
//
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"Недопустимый PrefabIndex: {enemyData.PrefabIndex}");
//                 }
//             }
//         }
//         
//         private void OnEnemyDeath()
//         {   
//             foreach (var obj in _enemies)
//             {
//                 Debug.Log(obj.UniqueId + "АЙДИ");
//                 Debug.Log(obj.isDead + "СОСТОЯНИЕ");
//
//                 if (!obj.isDead)
//                 {
//                     return;
//                 }
//             }
//             
//             Debug.Log("все умерли");
//             SpawnBoss();
//         }
//         
//         
//         
//         private Vector3 GetRandomPosition()
//         {
//             return new Vector3(
//                 Random.Range(_enemiesSpawnAreaCenter.x - _enemiesSpawnAreaExtents.x, _enemiesSpawnAreaCenter.x + _enemiesSpawnAreaExtents.x),
//                 _enemiesSpawnAreaCenter.y,
//                 Random.Range(_enemiesSpawnAreaCenter.z - _enemiesSpawnAreaExtents.y, _enemiesSpawnAreaCenter.z + _enemiesSpawnAreaExtents.y));
//         }
//
//         private void SpawnBoss()
//         {
//             var bossPrefab = _bossPrefab;
//             var enemy = Object.Instantiate(bossPrefab, _bossSpawnPoint, Quaternion.identity)
//                 .GetComponent<BossController>();
//             // enemy.UniqueId = enemyData.Id;
//             // enemy.PrefabIndex = enemyData.PrefabIndex;
//                     
//             var healthSystem = enemy.GetComponent<HealthSystem>();
//
//             healthSystem.onHealthChanged.AddListener(enemy.GetComponentInChildren<HealthBarView>().ChangeHp);
//             // healthSystem.Init(enemiesPower);
//             healthSystem.onDeath.AddListener((_) => OnEnemyDeath());
//             // healthSystem.SetHealth(b.Health);
//             enemy.GetComponent<SkillsController>().Init(null);
//         }
//         
//         
//         
//     }
// }


using System;
using System.Collections.Generic; 
using Bootstraps;
using Controllers.Entities; 
using Controllers.Entities.HealthController;
using Controllers.Entities.HealthController.Interfaces;
using Controllers.SaveLoad.Saveables; 
using Controllers.SaveLoad.Settings; 
using UnityEngine; 
using Views.Gameplay; 
using Object = UnityEngine.Object; 
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyManager
    {
        private CharacterController _player;
        private readonly SettingsInteractor _settingsInteractor;
        private readonly GameObject[] _enemyPrefabs;
        private readonly Vector2 _enemiesSpawnAreaExtents;
        private readonly Vector3 _enemiesSpawnAreaCenter;
        private readonly int _enemiesCount;
        private readonly List<ICharacterController> _characters = new();
        private readonly GameObject _bossPrefab;
        private readonly Vector3 _bossSpawnPoint;
        private bool _bossSpawned;
        private int _aliveCount;

        public EnemyManager(SettingsInteractor settingsInteractor, Vector2 enemiesSpawnAreaSize,
            GameObject[] enemyPrefabs,
            Vector3 enemiesSpawnAreaCenter, int enemiesCount, GameObject bossPrefab, Vector3 bossSpawnPoint,
            CharacterController player)
        {
            _settingsInteractor = settingsInteractor ?? throw new ArgumentNullException(nameof(settingsInteractor));
            _enemiesSpawnAreaExtents = enemiesSpawnAreaSize;
            _enemyPrefabs = enemyPrefabs ?? throw new ArgumentNullException(nameof(enemyPrefabs));
            _enemiesSpawnAreaCenter = enemiesSpawnAreaCenter;
            _enemiesCount = enemiesCount;
            _bossPrefab = bossPrefab ?? throw new ArgumentNullException(nameof(bossPrefab));
            _bossSpawnPoint = bossSpawnPoint;
            _player = player;
            SpawnEnemies(_enemiesCount);
        }

        public void SpawnEnemies(int enemiesCount)
        {
            _aliveCount = enemiesCount;
            var loadSettings = _settingsInteractor.LoadSettings();
            var enemiesPower = loadSettings.EnemiesPower;
            var peaceMode = loadSettings.PeaceMode;
            for (int i = 0; i < enemiesCount; i++)
            {
                int prefabIndex = Random.Range(0, _enemyPrefabs.Length);
                var enemyPrefab = _enemyPrefabs[prefabIndex];
                var enemy = Object.Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity)
                    .GetComponent<EnemyController>();
                if (enemy == null)
                {
                    Debug.LogError($"Enemy prefab {enemyPrefab.name} does not have EnemyController component.");
                    continue;
                }

                var healthSystem = enemy.GetComponent<HealthSystem>();
                if (healthSystem == null)
                {
                    Debug.LogError($"Enemy {enemy.name} does not have HealthSystem component.");
                    Object.Destroy(enemy.gameObject);
                    continue;
                }

                var healthBarView = enemy.GetComponentInChildren<HealthBarView>();
                if (healthBarView == null)
                {
                    Debug.LogWarning($"Enemy {enemy.name} does not have HealthBarView component.");
                }
                else
                {
                    healthSystem.onHealthChanged.AddListener(healthBarView.ChangeHp);
                }

                var skillsController = enemy.GetComponent<SkillsController>();
                if (skillsController == null)
                {
                    Debug.LogWarning($"Enemy {enemy.name} does not have SkillsController component.");
                }

                enemy.Init(peaceMode);
                healthSystem.Init(enemiesPower);
                healthSystem.onDeath.AddListener((_) => OnEnemyDeath());
                skillsController?.Init(null);
                enemy.UniqueId = Guid.NewGuid().ToString();
                enemy.PrefabIndex = prefabIndex;
                _characters.Add(enemy);
            }
        }

        public List<EnemyData> GetEnemyData()
        {
            List<EnemyData> enemyDataList = new();
            foreach (ICharacterController character in _characters)
            {
                if (character != null && !character.isDead)
                {
                    var healthSystem = character.GetComponent<HealthSystem>();
                    if (healthSystem == null)
                    {
                        Debug.LogWarning($"Character {character.UniqueId} does not have HealthSystem component.");
                        continue;
                    }

                    enemyDataList.Add(new EnemyData
                    {
                        Id = character.UniqueId,
                        Position = character.transform.position,
                        Health = healthSystem.Health,
                        PrefabIndex = character is BossController ? _enemyPrefabs.Length : character.PrefabIndex,
                        IsBoss = character is BossController
                    });
                }
            }

            return enemyDataList;
        }

        public void LoadEnemies(List<EnemyData> enemyDataList)
        {
            if (enemyDataList == null || enemyDataList.Count == 0) return;

            foreach (var character in _characters)
            {
                if (character != null)
                {
                    Object.Destroy(character.gameObject);
                }
            }

            _characters.Clear();
            _aliveCount = enemyDataList.Count;

            var loadSettings = _settingsInteractor.LoadSettings();
            var enemiesPower = loadSettings.EnemiesPower;
            foreach (var enemyData in enemyDataList)
            {
                if (enemyData.PrefabIndex < 0 || enemyData.PrefabIndex >= _enemyPrefabs.Length && !enemyData.IsBoss)
                {
                    Debug.LogWarning($"Invalid PrefabIndex: {enemyData.PrefabIndex} for non-boss enemy.");
                    _aliveCount--;
                    continue;
                }

                ICharacterController character;
                GameObject instantiatedObject;
                if (enemyData.IsBoss)
                {
                    instantiatedObject = Object.Instantiate(_bossPrefab, enemyData.Position, Quaternion.identity);
                    character = instantiatedObject.GetComponent<BossController>();
                }
                else
                {
                    var enemyPrefab = _enemyPrefabs[enemyData.PrefabIndex];
                    instantiatedObject = Object.Instantiate(enemyPrefab, enemyData.Position, Quaternion.identity);
                    character = instantiatedObject.GetComponent<EnemyController>();
                }

                if (character == null)
                {
                    Debug.LogError(
                        $"Instantiated object {instantiatedObject.name} does not have required controller component.");
                    Object.Destroy(instantiatedObject);
                    _aliveCount--;
                    continue;
                }

                var healthSystem = character.GetComponent<HealthSystem>();
                if (healthSystem == null)
                {
                    Debug.LogError($"Character {instantiatedObject.name} does not have HealthSystem component.");
                    Object.Destroy(instantiatedObject);
                    _aliveCount--;
                    continue;
                }

                var healthBarView = character.GetComponentInChildren<HealthBarView>();
                if (healthBarView == null)
                {
                    Debug.LogWarning($"Character {instantiatedObject.name} does not have HealthBarView component.");
                }
                else
                {
                    healthSystem.onHealthChanged.AddListener(healthBarView.ChangeHp);
                }

                var skillsController = instantiatedObject.GetComponent<SkillsController>();
                if (skillsController == null)
                {
                    Debug.LogWarning($"Character {instantiatedObject.name} does not have SkillsController component.");
                }

                character.UniqueId = enemyData.Id;
                character.PrefabIndex = enemyData.PrefabIndex;
                character.Init(loadSettings.PeaceMode);
                healthSystem.Init(enemiesPower);
                healthSystem.onDeath.AddListener((_) => OnEnemyDeath());
                healthSystem.SetHealth(enemyData.Health);
                skillsController?.Init(null);
                _characters.Add(character);
            }
        }

        private void OnEnemyDeath()
        {
            if (--_aliveCount <= 0)
            {
                Debug.Log("All characters are dead");
                if (!_bossSpawned)
                {
                    SpawnBoss();
                }
                else
                {
                    Debug.Log("Boss is dead. Game over or proceed to next stage.");
                    // Здесь можно добавить логику завершения игры или перехода к следующему этапу
                }
            }
        }

        private Vector3 GetRandomPosition()
        {
            return new Vector3(
                Random.Range(_enemiesSpawnAreaCenter.x - _enemiesSpawnAreaExtents.x,
                    _enemiesSpawnAreaCenter.x + _enemiesSpawnAreaExtents.x),
                _enemiesSpawnAreaCenter.y,
                Random.Range(_enemiesSpawnAreaCenter.z - _enemiesSpawnAreaExtents.y,
                    _enemiesSpawnAreaCenter.z + _enemiesSpawnAreaExtents.y));
        }

        private void SpawnBoss()
        {
            if (_bossSpawned) return;
            _bossSpawned = true;
            _aliveCount = 1;

            var boss = Object.Instantiate(_bossPrefab, _bossSpawnPoint, Quaternion.identity)
                .GetComponent<BossController>();
            if (boss == null)
            {
                Debug.LogError($"Boss prefab {_bossPrefab.name} does not have BossController component.");
                _bossSpawned = false;
                _aliveCount = 0;
                return;
            }

            var healthSystem = boss.GetComponent<HealthSystem>();
            if (healthSystem == null)
            {
                Debug.LogError($"Boss {boss.name} does not have HealthSystem component.");
                Object.Destroy(boss.gameObject);
                _bossSpawned = false;
                _aliveCount = 0;
                return;
            }

            var healthBarView = boss.GetComponentInChildren<HealthBarView>();
            if (healthBarView == null)
            {
                Debug.LogWarning($"Boss {boss.name} does not have HealthBarView component.");
            }
            else
            {
                healthSystem.onHealthChanged.AddListener(healthBarView.ChangeHp);
            }

            var skillsController = boss.GetComponent<SkillsController>();
            if (skillsController == null)
            {
                Debug.LogWarning($"Boss {boss.name} does not have SkillsController component.");
            }

            healthSystem.Init(_settingsInteractor.LoadSettings().EnemiesPower);
            healthSystem.onDeath.AddListener((_) => OnEnemyDeath());
            skillsController?.Init(null);
            boss.Init(_settingsInteractor.LoadSettings().PeaceMode);
            boss.UniqueId = Guid.NewGuid().ToString();
            boss.PrefabIndex = -1; // Босс не использует индекс префаба из _enemyPrefabs
            _characters.Add(boss);
        }
    }
}