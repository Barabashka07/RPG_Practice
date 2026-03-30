using System.Collections;
using Anims;
using Controllers.Entities;
using UnityEngine;
using Weapons.Base;

namespace StateMachines.PlayerStates.FightStates
{
    // Наследуемся от нашего шаблона FightPlayerState
    public class AttackState : FightPlayerState
    {

        public AttackState(FightController fightController, SkillsController skillsController, PlayerAnimator animator) : base(fightController, skillsController, animator)
        {
        }
        
        
        // Срабатывает ровно один раз в момент клика ЛКМ (когда StateMachine переключает на этот стейт)
        public override void Enter()
        {
            Debug.Log("Entering Melee"); //Для отладки в консоль смска
            //Корутина таймер для вкл/выкл меча
            PlayerAnimator.StartCoroutine(SwordColliderSwitch());
            //Говорим проиграй аниму удара
            PlayerAnimator.DoAttack();
            //Скилл завершен, перезарядка
            SkillsController.Skills[SkillType.Melee].Cast();

        }

        public override void Execute()
        {
            //Пустой так как за все отвечает корутина
        }

        public override void Exit()
        {
            //Выход из состояния 
            Debug.Log("Exiting Melee");
        }

        private IEnumerator SwordColliderSwitch()
        {
            //yield return ждёт, пока анимация Attack на слое Fight не проиграется на 40% (0.4f)
            yield return new WaitUntil(()=>PlayerAnimator.CheckAnimationState((int)LayerNames.Fight, 0.4f, "Attack"));
            //Засчет урона 
            FightController.SwordCollider.enabled = true;

            yield return new WaitUntil(()=>PlayerAnimator.CheckAnimationState((int)LayerNames.Fight, 0.53f, "Attack"));
             //Проверка попадания, чтобы был удар а не убийтсво воллайдером меча
            FightController.SwordCollider.enabled = false;
        }
    }
}