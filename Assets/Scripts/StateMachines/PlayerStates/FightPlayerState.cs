using Anims;
using Controllers.Entities;

namespace StateMachines.PlayerStates
{
    // abstract означает, что мы не можем создать просто "Боевое состояние".
    // IState обязывает нас реализовать методы Enter, Execute и Exit.
    public abstract class FightPlayerState : IState
    {
        // protected делает эти переменные доступными только для наследников этого класса.
        protected FightController FightController;
        protected SkillsController SkillsController;
        protected PlayerAnimator PlayerAnimator;
        
        protected FightPlayerState(FightController fightController, SkillsController skillsController, PlayerAnimator animator)
        {
            // Конструктор: когда мы создаем состояние атаки, мы обязаны передать ему эти 3 компонента.
            FightController = fightController;
            SkillsController = skillsController;
            PlayerAnimator = animator;
        }
        // virtual означает, что классы-наследники могут переопределить (override) эти методы под себя.
        public virtual void Enter()
        {
        }

        public virtual void Execute()
        {
        }

        public virtual void Exit()
        {
        }
    }
}