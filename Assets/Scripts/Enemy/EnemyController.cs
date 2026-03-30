using Controllers.Entities;
using Controllers.Entities.HealthController;
using Controllers.Entities.HealthController.Interfaces;
using Enemy.States;
using StateMachines;
using UnityEngine;
using UnityEngine.AI;
using Views.Gameplay;
using StateMachine = StateMachines.StateMachine;
using Weapons.Base;
using AttackState = Enemy.States.AttackState;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, ICharacterController
    {   
        
        // public bool isDead { get; set; }
        // public string UniqueId { get; set; }
        // public int PrefabIndex { get; set; }
        // Transform ICharacterController.transform => transform;
        // GameObject ICharacterController.gameObject => gameObject;
        // HealthSystem ICharacterController.GetComponent<T>() => GetComponent<HealthSystem>();
        // T ICharacterController.GetComponentInChildren<T>() => GetComponentInChildren<T>();
        
        public bool isDead { get; set; }
        public string UniqueId { get; set; }
        public int PrefabIndex { get; set; }
        Transform ICharacterController.transform => transform;
        GameObject ICharacterController.gameObject => gameObject;
        HealthSystem ICharacterController.GetComponent<T>() => GetComponent<HealthSystem>();
        T ICharacterController.GetComponentInChildren<T>() => GetComponentInChildren<T>();
        
        
        
        
        
        
        
        
        [SerializeField] private GameObject _sword;
        
        public Collider SwordCollider { get; private set; } //Коллайдер для нанесения урона
        // public string UniqueId;
        // public int PrefabIndex;
        private Canvas hpCanvas; //Вывод канваса с полоской хп
        //Тут компы ИИ
        private StateMachine enemyStateMachine;
        private EnemyAnimator enemyAnimator;
        private HealthSystem healthSystem;
        private NavMeshAgent _agent;
        private Transform _playerTransform;
        private SkillsController _skillsController;
        [SerializeField] private float _rotationSpeed; //Скорость поворота
        [SerializeField] private float searchRadius; //Радуис поиска игрока
        [SerializeField] private float attackRange; //Радиус атаки ИИ
        public int deadEnemies;
        //Тут что то типо флагов как у босса
        public bool IsChasing { get; private set;}
        // public bool isDead;
        public bool IsInAttackRange { get; private set;}
        private bool _isPeaceful; //Мирный режим
        private void Awake()
        {
            SwordCollider = _sword.GetComponent<BoxCollider>();
            enemyStateMachine = new StateMachine();
            _skillsController = GetComponent<SkillsController>();
            enemyAnimator = GetComponent<EnemyAnimator>();
            healthSystem = GetComponent<HealthSystem>();
            //Получение урона связывается с получением хита
            gameObject.GetComponent<IHittable>().onHit.AddListener(enemyAnimator.DoHitEvent);
            _agent = GetComponent<NavMeshAgent>();
            hpCanvas = GetComponentInChildren<Canvas>();
        }

        public void Init(bool isPeaceful)
        {
            _isPeaceful = isPeaceful;
            //Ищем игрока на сцене
            _playerTransform = FindFirstObjectByType<CharacterController>().transform;
            EnemyStatesInit();
        }

        private void Update()
        {   
            if(!_playerTransform) return;
            ChasingChecker(); //Обновление/проверка дистанции до игрока
            enemyStateMachine.Tick(); //Процесс тиков через стейт
        }

        public void SetFollowPlayer()
        {
            //Тут моб идет к игроку
            _agent.SetDestination(_playerTransform.position);
        }

        public void SetRunFromPlayer()
        {
            //Тут убегает от игрока
            _agent.SetDestination(GetRunPoint());
        }
        //Тут я не особо понимаю 
        private Vector3 GetRunPoint() => (transform.position - _playerTransform.position).normalized * 2f + transform.position;
 

        public void RotateToPlayer()
        {
            Vector3 direction = _playerTransform.position - transform.position;
            Quaternion desiredRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationSpeed * Time.deltaTime);
        }
        //Инициализируем ИИ черех стейт
        private void EnemyStatesInit()
        {
            IState attackState;
            SkillType type;
            //Проверка тега если тег маг до он будет дальнего боя
            if (CompareTag("Wizard"))
            {
                attackState = new RangeAttackState(this, enemyAnimator, _agent, _skillsController);
                type = SkillType.Fireball;
            }
            else
            {
                //Проверка если тег не маг тогда это милишник и будет бить атакой ближнего боя
                attackState = new AttackState(this, enemyAnimator, _agent, _skillsController);
                type = SkillType.Melee;
            }
            // Локальные методы для читаемости переходов
            bool AttackReady() => _skillsController.Skills[type]._isReady;
            
            
            //Тут создание состояний для врагов 
            var idleState = new IdleState(this,enemyAnimator, _agent); //Покой
            var walkState =  new WalkState(this,enemyAnimator,_agent); //Бежит бить по жопе
            var fearState =  new FearState(this,enemyAnimator,_agent); //Страх
            var deathState = new DeathState(this,enemyAnimator,_agent, hpCanvas); //Смэрть
            
            // Локальные методы для читаемости переходов
            bool AttackAnimationEnded() => enemyAnimator.CheckAnimationState(0,1f,"attackTest");

            //Проверка через стейт если хп равно 0, то смэрть
            enemyStateMachine.AddAnyTransition(deathState, () => healthSystem.Health <= 0f);

            //Мирный режим
            if (_isPeaceful)
            {
                // МЕХАНИКА СТРАХА: Если ХП меньше 30% в мирном режиме - враг убегает
                enemyStateMachine.AddTransition(idleState, fearState, () => healthSystem.Health/healthSystem.MaxHealth <= .3f);
            }
            else
            {
                //Таж самая проверка как с магом или ближником, только через мирный или обычный режим
                enemyStateMachine.AddTransition(idleState, walkState, () => IsChasing && !IsInAttackRange);
                enemyStateMachine.AddTransition(idleState, attackState,
                    () => IsInAttackRange && AttackReady());
            }
            
            //Возвраты состояний
            enemyStateMachine.AddTransition(walkState, idleState, () => !IsChasing || IsInAttackRange);
            enemyStateMachine.AddTransition(walkState, attackState, () => IsInAttackRange && AttackReady());
            enemyStateMachine.AddTransition(attackState, idleState,
                () => IsInAttackRange && AttackAnimationEnded());
            enemyStateMachine.AddTransition(attackState, walkState,
                () => !IsInAttackRange && AttackAnimationEnded());


            // enemyStateMachine.AddTransition(idleState, spellState, () => IsInCastRange && (Time.time - lastAttackTime >= attackCooldown));
            // enemyStateMachine.AddTransition(spellState, idleState, () => IsInCastRange && RangeAnimationEnded());
            // enemyStateMachine.AddTransition(walkState, spellState, () => IsInCastRange);
            // enemyStateMachine.AddTransition(spellState, walkState, () => !IsInCastRange && RangeAnimationEnded());
            
            
            
            
            enemyStateMachine.SetState(idleState);
        }
        
        //Система радара для врагов
        private void ChasingChecker()
        {
            if (Vector3.Distance(_agent.transform.position, _playerTransform.position) <= searchRadius)
            {   
                IsChasing = true;
                if (Vector3.Distance(_agent.transform.position, _playerTransform.position) <= attackRange)
                {
                    IsInAttackRange = true;
                }
                else
                {   
                    IsInAttackRange = false;
                }
            }
            else
            {   
                IsChasing = false;
            }
        }
        
        
}
}
