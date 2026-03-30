// using System;
// using Controllers.Entities.HealthController.Interfaces;
// using Enemy;
// using UnityEngine;
// using UnityEngine.Events;
// using Weapons;
// using Weapons.Base;
//
// namespace Controllers.Entities.HealthController
// {
//     public class HealthSystem : MonoBehaviour, IDamageable, IHealthChange, IHittable, IKillable
//     {
//         private EnemyController enemyController;
//         private BossController bossController;
//
//         public void Awake()
//         {
//             enemyController = GetComponent<EnemyController>();
//             bossController = GetComponent<BossController>();
//             
//         }
//
//         public float Health
//         {
//             get => _health;
//             set
//             {
//                 _health = value;
//                 onHealthChanged?.Invoke(_health,MaxHealth);
//             }
//         }
//
//         public float MaxHealth { get; private set; } = 100f;
//         
//         public UnityEvent<float, float> onHealthChanged { get; } = new();
//         public UnityEvent<bool> onDeath { get; } = new();
//         public UnityEvent onHit { get; } = new();
//         [SerializeField] private float _health;
//
//         public void Init(float healthMultiplier)
//         {
//             MaxHealth *= healthMultiplier;
//             SetHealthToMax();
//         }
//
//         private void SetHealthToMax()
//         {
//             Health = MaxHealth;
//         }
//         
//         public void SetHealth(float health)
//         {
//             Health = health;
//         }
//
//         public void TakeDamage(Damage damage)
//         {   
//             if (Health - damage.Value <= 0)
//             {   
//                 
//                 if(enemyController){enemyController.isDead = true;}
//                 else if(bossController){bossController.isDead = true;}
//                 
//                 Health = 0;
//                 onDeath?.Invoke(true);
//             }
//             else
//             {
//                 Health -= damage.Value;
//                 onHit?.Invoke();
//             }
//         }
//  
//     }
//
//     public interface IKillable
//     {
//         public UnityEvent<bool> onDeath { get; }
//     }
// }
//
//


using System;
using Controllers.Entities.HealthController.Interfaces;
using Enemy;
using UnityEngine;
using UnityEngine.Events;
using Weapons;
using Weapons.Base;

namespace Controllers.Entities.HealthController
{
    //(получение урона, изменение ХП, смерть)
    public class HealthSystem : MonoBehaviour, IDamageable, IHealthChange, IHittable, IKillable
    {
        private ICharacterController _controller;

        public void Awake()
        {
            _controller = GetComponent<ICharacterController>();
        }

        public float Health
        {
            get => _health;
            set
            {
                _health = value; //Значение ХП, обновление UI
                onHealthChanged?.Invoke(_health, MaxHealth);
            }
        }

        public float MaxHealth { get; private set; } = 100f;

        public UnityEvent<float, float> onHealthChanged { get; } = new(); //Тут передаем хп
        public UnityEvent<bool> onDeath { get; } = new();
        public UnityEvent onHit { get; } = new(); //Получение удара
        [SerializeField] private float _health;

        public void Init(float healthMultiplier) //Через init передаем зп боссу и всем остальным - healthMultiplier
        {
            MaxHealth *= healthMultiplier;
            SetHealthToMax();
        }

        private void SetHealthToMax()
        {
            Health = MaxHealth;
        }

        public void SetHealth(float health)
        {
            Health = health;
        }

        public void TakeDamage(Damage damage)
        {
            //Если урон боьше текущего хп = смэрть
            if (Health - damage.Value <= 0) // Фатальный урон
            {
                if (_controller != null)
                {
                    _controller.isDead = true; //Проверка смерти через стейт машину
                }

                Health = 0;
                onDeath?.Invoke(true); // Смска о смерти 
            }
            else
            {
                //Если хп много тут м просто отнимаем хп
                Health -= damage.Value; // Отнимаем Хп
                onHit?.Invoke();
            }
        }
    }
    
    public interface IKillable
    {
        public UnityEvent<bool> onDeath { get; }
    }
}