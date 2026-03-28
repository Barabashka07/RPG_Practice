using StateMachines;
using UnityEngine.AI;

namespace Enemy
{
    // abstract означает, что мы не можем создать экземпляр этого класса напрямую.
    // IState - интерфейс конечного автомата, требующий методы Enter, Execute, Exit.
    public abstract class StatesEnemyConst : IState
    {
        // protected означает, что эти переменные видны только внутри этого класса 
        // и внутри классов-наследников (например, AttackState).
        protected EnemyController EnemyController;
        protected EnemyAnimator EnemyAnimator;
        protected NavMeshAgent NavMeshAgent;
  
        // Конструктор: при создании состояния мы передаем ему ссылки на контроллер, аниматор и агента,
        // чтобы состояние могло ими управлять.
        protected StatesEnemyConst(EnemyController enemyController, EnemyAnimator animator, NavMeshAgent navMeshAgent)
        {
            EnemyController = enemyController;
            EnemyAnimator = animator;
            NavMeshAgent = navMeshAgent;
        }
        // virtual означает, что классы-наследники МОГУТ переопределить (override) эти методы своей логикой.
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