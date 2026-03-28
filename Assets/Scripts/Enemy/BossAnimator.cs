using UnityEngine;

namespace Enemy
{
    public class BossAnimator : MonoBehaviour
    {
        // ОПТИМИЗАЦИЯ: Переводим строковые имена параметров аниматора в числовые хэши один раз на весь класс.
        // Это избавляет Unity от необходимости вычислять хэш каждый раз при вызове анимации.
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Hitted = Animator.StringToHash("Hitted");
        private static readonly int SuperAttack = Animator.StringToHash("SuperAttack");
        

        // Свойство для получения доступа к компоненту Animator извне (только для чтения)
        public Animator _animator { get; private set; }
        

        // ВАЖНЫЙ МЕТОД: Проверяет, проигралась ли нужная анимация до определенного момента.
        // time - это процент завершения (например, 0.99f - это 99% завершения анимации).
        // stateName - точное имя состояния (квадратика) в окне Animator.
        public bool CheckAnimationState(int layerIndex, float time, string stateName) => 
            _animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= time && 
            _animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);

        private void Awake()
        {
            // Кэшируем компонент при рождении объекта, чтобы не вызывать медленный GetComponent каждый кадр
            _animator = GetComponent<Animator>();
        }
        // Мы используем заранее заготовленные числовые хэши (Death, Walk и т.д.)
        public void DeathEvent()
        {
            _animator.SetTrigger(Death);
        }
        
        
        public void WalkEvent()
        {
            _animator.SetBool(Walk, true);
            _animator.SetBool(Idle,false);
        }

        public void IdleEvent()
        {
            _animator.SetBool(Walk, false);
            _animator.SetBool(Idle, true);
        }
        
        public void DoAttack()
        {
            _animator.SetTrigger(Attack);
        }
        
        public void DoHitEvent()
        {
            _animator.SetTrigger(Hitted);
        }

        public void DoSuperAttack()
        {
            _animator.SetTrigger(SuperAttack);
        }
    }
}