using System.Threading.Tasks;
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
    public class BossController : MonoBehaviour, ICharacterController
    {
        [SerializeField] private GameObject[] _particles;
        
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
        
        
        
        
        
        
        [SerializeField] private GameObject _fist;
        
        public Collider SwordCollider { get; private set; }
        // public string UniqueId;
        // public int PrefabIndex;
        private Canvas hpCanvas;
        private StateMachine enemyStateMachine;
        private BossAnimator bossAnimator;
        private HealthSystem healthSystem;
        private NavMeshAgent _agent;
        private Transform _playerTransform;
        private SkillsController _skillsController;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float searchRadius;
        [SerializeField] private float attackRange;

        private bool _isPeaceful;
        
        public bool IsChasing { get; private set;}
        // public bool isDead;
        public bool IsInAttackRange { get; private set;}
        //public bool _superAttack { get; private set; }
        private void Awake()
        {
            enemyStateMachine = new StateMachine();
            _skillsController = GetComponent<SkillsController>();
            bossAnimator = GetComponent<BossAnimator>();
            healthSystem = GetComponent<HealthSystem>();
            gameObject.GetComponent<IHittable>().onHit.AddListener(bossAnimator.DoHitEvent);
            _agent = GetComponent<NavMeshAgent>();
            hpCanvas = GetComponentInChildren<Canvas>();
        }

        public async void Init(bool isPeaceful)
        {
            _isPeaceful = isPeaceful;
            await GetPlayer();
            print(_playerTransform);
            CreateParticles();
            BossStatesInit();
        }

        private void CreateParticles()
        {
            var part = _particles[Random.Range(0, _particles.Length)];
            Instantiate(part, _fist.transform.position, Quaternion.identity, _fist.transform);
        }

        private async Task GetPlayer()
        {
            CharacterController player;
            do
            {
                player = FindFirstObjectByType<CharacterController>();
                await Task.Delay(10);
            } while (!player);

            _playerTransform = player.transform;
        }

        private void Update()
        {   
            if(!_playerTransform) return;
            ChasingChecker();
            _agent.SetDestination(_playerTransform.position);
            enemyStateMachine.Tick();
        }

        public void RotateToPlayer()
        {
            Vector3 direction = _playerTransform.position - transform.position;
            Quaternion desiredRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationSpeed * Time.deltaTime);
        }

        private void BossStatesInit()
        {
            
            var attackState = new BossAttackState(this, bossAnimator, _agent, _skillsController);
            bool AttackReady() => _skillsController.Skills[SkillType.Punch]._isReady;
            bool HeavyAttackReady() => _skillsController.Skills[SkillType.Heavy]._isReady;

            
            var superAttackState = new BossSuperAttackState(this, bossAnimator, _agent, _skillsController);
            
            var idleState = new BossIdleState(this,bossAnimator, _agent);
            var walkState =  new BossWalkState(this,bossAnimator,_agent);
            var deathState = new BossDeathState(this,bossAnimator,_agent, hpCanvas);
            
            bool AttackAnimationEnded() => bossAnimator.CheckAnimationState(0,.99f,"BossAttack");
            bool HeavyAttackAnimationEnded() => bossAnimator.CheckAnimationState(0,.99f,"BossSuperAttack");

            bool WasHit() => !Mathf.Approximately(healthSystem.Health, healthSystem.MaxHealth);
            

            enemyStateMachine.AddAnyTransition(deathState, () => healthSystem.Health <= 0f);
            if (_isPeaceful)
            {
                enemyStateMachine.AddTransition(idleState, walkState, () => IsChasing && !IsInAttackRange && WasHit());
                enemyStateMachine.AddTransition(idleState, superAttackState,
                    () => IsInAttackRange && HeavyAttackReady() && WasHit());
                enemyStateMachine.AddTransition(idleState, attackState,
                    () => IsInAttackRange && AttackReady() && !HeavyAttackReady() && WasHit());
            }
            else
            {
                enemyStateMachine.AddTransition(idleState, walkState, () => IsChasing && !IsInAttackRange);
                enemyStateMachine.AddTransition(idleState, superAttackState,
                    () => IsInAttackRange && HeavyAttackReady());
                enemyStateMachine.AddTransition(idleState, attackState,
                    () => IsInAttackRange && AttackReady() && !HeavyAttackReady());
            }
            
            enemyStateMachine.AddTransition(walkState, superAttackState, () => IsInAttackRange && HeavyAttackReady());
            enemyStateMachine.AddTransition(walkState, attackState, () => IsInAttackRange && AttackReady() && !HeavyAttackReady());
            enemyStateMachine.AddTransition(walkState, idleState, () => !IsChasing || IsInAttackRange);
            enemyStateMachine.AddTransition(attackState, idleState,
                () => IsInAttackRange && AttackAnimationEnded());
            enemyStateMachine.AddTransition(attackState, superAttackState,
                () => IsInAttackRange && AttackAnimationEnded() && HeavyAttackReady());
            enemyStateMachine.AddTransition(attackState, walkState,
                () => !IsInAttackRange && AttackAnimationEnded());


            enemyStateMachine.AddTransition(superAttackState, idleState,
                () => IsInAttackRange && HeavyAttackAnimationEnded());
            enemyStateMachine.AddTransition(superAttackState, walkState,
                () => !IsInAttackRange && HeavyAttackAnimationEnded());


            enemyStateMachine.SetState(idleState);

            
        }

        
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
