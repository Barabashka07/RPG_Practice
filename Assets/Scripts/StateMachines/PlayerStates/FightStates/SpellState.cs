using System.Collections;
using Anims;
using Controllers.Entities;
using UnityEngine;
using Weapons.Base;

namespace StateMachines.PlayerStates.FightStates
{
    public class SpellState : FightPlayerState
    {
        public SpellState(FightController fightController, SkillsController skillsController, PlayerAnimator animator) : base(fightController, skillsController, animator)
        {
        }

        public override void Enter()
        {
            Debug.Log("Entering Spell");
            //Анимация каста
            PlayerAnimator.DoSpell();
            //FightController.swordGameObject.SetActive(false);
            PlayerAnimator.StartCoroutine(SpellCast()); // Таймер вылета снаряда через корутину

        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            //FightController.swordGameObject.SetActive(true);
            Debug.Log("Exiting Spell");
        }

        private IEnumerator SpellCast()
        {
            //Пауза до выброса руки для магии 
            yield return new WaitUntil(()=>PlayerAnimator.CheckAnimationState((int)LayerNames.Fight, 0.425f, "Spell"));
            //Спавн шарика с уроном
            SkillsController.Skills[SkillType.Fireball].Cast();
        }
    }
}