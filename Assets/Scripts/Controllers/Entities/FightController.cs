using Anims;
using Controllers.Entities.HealthController.Interfaces;
using UnityEngine;
using StateMachines;
using StateMachines.PlayerStates.FightStates;
using Weapons.Base;
using Weapons.Colliding;

namespace Controllers.Entities
{
    public class FightController : MonoBehaviour
    {
        private StateMachine _fightStateMachine;
        private PlayerAnimator _playerAnimator;
        private SkillsController _skillsController;
        private InputManager _inputManager;

        [HideInInspector] public bool IsSheathed;

        [SerializeField] public GameObject swordGameObject;
        [SerializeField] public GameObject hipSwordGameObject;
        public Sword Sword { get; private set; }
        public Collider SwordCollider { get; private set; }

        //Блок подготовки через Awake
        private void Awake()
        {
            _inputManager = FindAnyObjectByType<InputManager>();
            _fightStateMachine = new StateMachine();
            _skillsController = GetComponent<SkillsController>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            //Тут мы берем систему хп и проверяем удары 
            gameObject.GetComponent<IHittable>().onHit.AddListener(_playerAnimator.DoHit);
            SwordCollider = swordGameObject.GetComponent<BoxCollider>();
        }
        

        private void Start()
        {
            AttackStatesInit();
        }
        //Что то типо EventTick из анрила каждую секунду у нас 60 вызовов
        private void Update()
        {
            _fightStateMachine.Tick();
        }
        
        private void AttackStatesInit()
        {
            //Процесс вмаха мечом
            var attackState = new AttackState(this,_skillsController, _playerAnimator);
            //Процесс каста фаербола
            var spellState = new SpellState(this,_skillsController, _playerAnimator);
            //Боевая стойка, мы готовы бить
            var idleAttackState = new IdleAttackState(this, _skillsController, _playerAnimator);
            //Мирная стойка мы готовы бить
            var sheathState = new SheathedSwordState(this,_skillsController,_playerAnimator);

            //Проверки дошли ли анимашки до 99% готовности, чтобы ударить и убрать хп
            bool MeleeAnimationEnded() => _playerAnimator.CheckAnimationState((int)LayerNames.Fight,0.99f,"Attack");
            bool SpellAnimationEnded() => _playerAnimator.CheckAnimationState((int)LayerNames.Fight,0.99f,"Spell");
            bool SheathAnimationEnded() => _playerAnimator.CheckAnimationState((int)LayerNames.Fight,0.99f,"Sheath");

            //Тут мы достаем и убираем меч
            _fightStateMachine.AddTransition(idleAttackState,sheathState, () => _inputManager.IsSheathed);
            _fightStateMachine.AddTransition(sheathState,idleAttackState, () => !_inputManager.IsSheathed);
            //Тут мы используем удар мечом
            _fightStateMachine.AddTransition(idleAttackState,attackState, () => _inputManager.MeleeInput && _skillsController.Skills[SkillType.Melee]._isReady);
            _fightStateMachine.AddTransition(attackState,idleAttackState, MeleeAnimationEnded);
            //Тут мы используем магию если меч убран + каст магии если мы готовы
            _fightStateMachine.AddTransition(sheathState,spellState, () => _inputManager.RMBInput && _inputManager.SpellInput && _skillsController.Skills[SkillType.Fireball]._isReady);
            _fightStateMachine.AddTransition(spellState,sheathState, SpellAnimationEnded);
            
            _fightStateMachine.SetState(idleAttackState);
            
        }

        
    }
}