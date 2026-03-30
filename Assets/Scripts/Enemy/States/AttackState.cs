using System.Collections;
using Controllers.Entities;
using UnityEngine;
using UnityEngine.AI;
using Weapons.Base;

namespace Enemy.States
{
    // Наследуемся от нашего шаблона FightPlayerState
    public class AttackState : StatesEnemyConst
    {
        private SkillsController _skillsController;
        // Передаем контроллеры в базовый класс (base)
        public AttackState(EnemyController enemyController, EnemyAnimator animator, NavMeshAgent navMeshAgent, SkillsController skillsController) : base(enemyController, animator, navMeshAgent)
        {
            _skillsController = skillsController;
        }
        // Срабатывает ровно один раз в момент клика ЛКМ (когда StateMachine переключает на этот стейт)
        public override void Enter()
        {
            //Использование ближ атаки + кд скилла
            _skillsController.Skills[SkillType.Melee].Cast();
            //Корутина которая следит за проигрыванием анимки
            EnemyAnimator.StartCoroutine(SwordColliderSwitch());
            Debug.Log("Entering ENEMY ATTACK");
            EnemyAnimator.DoAttack();
            NavMeshAgent.isStopped = true;
        }
        // Срабатывает каждый кадр, пока идет атака.
        public override void Execute()
        {
            EnemyController.RotateToPlayer();
        }

        public override void Exit()
        {
            
        }
        
        private IEnumerator SwordColliderSwitch()
        {
            //Замахивание вперед
            yield return new WaitUntil(()=>EnemyAnimator.CheckAnimationState(0, 0.3f, "attackTest"));
            EnemyController.SwordCollider.enabled = true;
            yield return new WaitUntil(()=>EnemyAnimator.CheckAnimationState(0, 0.53f, "attackTest"));
            //Пауза замахивания пока не дойдтет до 0.53f типо траектория
            EnemyController.SwordCollider.enabled = false;
        }
    }
}