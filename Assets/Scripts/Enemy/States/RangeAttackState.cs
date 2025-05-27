using System.Collections;
using Controllers.Entities;
using UnityEngine;
using UnityEngine.AI;
using Weapons.Base;
using Weapons.Skills;

namespace Enemy.States
{
    public class RangeAttackState : StatesEnemyConst
    {
        
        private SkillsController _skillsController;
        public RangeAttackState(EnemyController enemyController, EnemyAnimator animator, NavMeshAgent navMeshAgent, SkillsController skillsController ) : base(enemyController, animator, navMeshAgent)
        {
            _skillsController = skillsController;
        }

        public override void Enter()
        {
            Debug.Log("Entering ENEMY RANGE ATTACK");
            EnemyAnimator.DoAttack();
            NavMeshAgent.isStopped = true;
            EnemyAnimator.StartCoroutine(SpellCast());
        }

        public override void Execute()
        {
            EnemyController.RotateToPlayer();
        }

        public override void Exit()
        {
            
        }
        
        private IEnumerator SpellCast()
        {
            yield return new WaitUntil(()=>EnemyAnimator.CheckAnimationState(0, 0.425f, "attackTest"));
            var plug = (SpellSkill)_skillsController.Skills[SkillType.Fireball];
            if (Random.Range(0, 2) == 1)
            {
                _skillsController.Skills[SkillType.Fireball].Cast();
            }
            else
            {
                if (_skillsController.Skills[SkillType.Meteor]._isReady)
                {
                    _skillsController.Skills[SkillType.Meteor].Cast();
                    plug.CastPlug();
                }
                else
                {
                    _skillsController.Skills[SkillType.Fireball].Cast();
                }
            }
            
        }
    }
}