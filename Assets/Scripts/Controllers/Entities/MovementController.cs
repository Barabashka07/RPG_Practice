using Anims;
using Controllers.Entities.HealthController;
using StateMachines.PlayerStates.MoveStates;
using UnityEngine;
using StateMachines;

namespace Controllers.Entities
{
    public class MovementController : MonoBehaviour
    {
        private InputManager _inputManager;
        private StateMachine _moveStateMachine;
        private PlayerAnimator _playerAnimator;
        private CharacterController _controller;
        [SerializeField] private Transform _spine;
        private Camera _camera;
        

        public bool IsGrounded { get; private set;}

        [SerializeField] public float moveSpeed = 2f;
        [SerializeField] public float runSpeed = 2f;
        [SerializeField] private float _velocityVertical = 0f;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _rotationSpeed = 1f;
        private float _groundCheckDistance;
        private bool _death;
        private Vector3 _inertialMoveDirection;


        [Header("Controller")] 
        [SerializeField] private Transform _foot;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _root;
        

        private void Awake()
        {
            _inputManager = FindAnyObjectByType<InputManager>();
            _moveStateMachine = new StateMachine();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _controller = GetComponent<CharacterController>();
        }

        public void Init(Camera camera)
        {
            _camera = camera;
        }

        private void Start()
        {
            _groundCheckDistance = (_controller.height / 2) + .1f;

            MoveStatesInit();
            
        }

        private void Update()
        {
            _moveStateMachine.Tick();
            //SetControllerParams();
            Rotate();
            ApplyGravity();
            CheckGround();
        }
        
        private void MoveStatesInit()
        {
            var idleState = new IdleState(this, _playerAnimator);
            var jumpingState = new JumpingState(this,_playerAnimator);
            var walkingState = new WalkingState(this,_playerAnimator);
            var sprintingState = new SprintingState(this,_playerAnimator);
            var fallingState = new FallingState(this,_playerAnimator);
            var deathState = new DeathState(this,_playerAnimator);
            gameObject.GetComponent<IKillable>().onDeath.AddListener(value => _death = value);

            //Если мы стоим и клавиатура или мышка не двигаются MoveInput, значит пора идти
            _moveStateMachine.AddTransition(idleState, walkingState, () => _inputManager.MoveInput != Vector2.zero);
            //Если мы встали и нажали шифт сразу переходим в бег
            _moveStateMachine.AddTransition(idleState, sprintingState, () => _inputManager.SprintInput);
            //Шли, отпустили, встали
            _moveStateMachine.AddTransition(walkingState, idleState, () => _inputManager.MoveInput == Vector2.zero);
            //Шли,нажади шифт побежали
            _moveStateMachine.AddTransition(walkingState, sprintingState, () => _inputManager.SprintInput);
            //Если мы допустим прыгнули и почвы под ногами нет, то анимка падения
            _moveStateMachine.AddTransition(walkingState,fallingState, ()=> !IsGrounded);
            //Если мы на земле и нажали прыжок-прыгаем
            _moveStateMachine.AddTransition(sprintingState,walkingState, () => !_inputManager.SprintInput);
            //Переход  покой после прыжка
            _moveStateMachine.AddTransition(sprintingState,fallingState, ()=> !IsGrounded);
            _moveStateMachine.AddTransition(jumpingState, fallingState, () => !IsGrounded);
            _moveStateMachine.AddTransition(fallingState, idleState, () => IsGrounded);
            //Запрет двойного прыжка
            _moveStateMachine.AddAntiState(fallingState,jumpingState);
            _moveStateMachine.AddAnyTransition(jumpingState, () => _inputManager.JumpInput && IsGrounded);
            _moveStateMachine.AddAnyTransition(deathState, () => _death);

            _moveStateMachine.SetState(idleState);
        }
        
        private void ApplyGravity()
        {
            if (!_controller.isGrounded)
            {
                _velocityVertical -= 15f * Time.deltaTime; //Имитация свободного падения 
                _controller.Move(Vector3.up * (_velocityVertical * Time.deltaTime)); //Тут та=янем персонажа вниз
            }
            else
            {
                _velocityVertical = 0; //Проверка если персонаж упал, и консулся земли то скорость падения 0
            }
        }

        private void Rotate()
        {
            if(!_camera) return;
            //Направление камеры и обнуление Y, чтобы персонаж не тянулся к земле
            Vector3 forwardDirection = _camera.transform.forward;
            Vector3 rightDirection = _camera.transform.right;
            forwardDirection.y = 0;
            rightDirection.y = 0;
            if (_inputManager.RMBInput)
            {
                //Хитровыебанная система, если наша камера всмотрит в другую строну, то персонаж повернется и к нец тоже
                Quaternion desiredRotation = Quaternion.LookRotation(forwardDirection, Vector3.up) * Quaternion.Euler(0,30f,0);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationSpeed * Time.deltaTime);

            }
            else if (_inputManager.MoveInput != Vector2.zero)
            {
                //Персонаж поворячивается лицом в сторону бега
                Vector3 moveDirection = forwardDirection.normalized * _inputManager.MoveInput.y + rightDirection.normalized * _inputManager.MoveInput.x;
                Quaternion desiredRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _rotationSpeed * Time.deltaTime);
            }
        }

        


        public void Move()
        {
            //Вектор камеры + поворот W\S A\D 
            Vector3 forwardDirection = _camera.transform.forward;
            Vector3 rightDirection = _camera.transform.right;
            forwardDirection.y = 0;
            rightDirection.y = 0;
            Vector3 moveDirection = forwardDirection.normalized * _inputManager.MoveInput.y + rightDirection.normalized * _inputManager.MoveInput.x;
            //Куда бежим
            _inertialMoveDirection = moveDirection;
            //Движение коллайдера
            _controller.Move(moveDirection * (Time.deltaTime * moveSpeed));
        }

        public void InertialMove()
        {
            //Инерция в полете - совершили прыжок в нажимаем WASD 
            _controller.Move(_inertialMoveDirection * (Time.deltaTime * moveSpeed));
        }
    
        public void Jump()
        {
            //Прыдок через _jumpForce толчок вверх
            _velocityVertical = _jumpForce;
            _controller.Move(Vector3.up * (_velocityVertical * Time.deltaTime));
        }

        private void SetControllerParams()
        {
            //Динамический коллайдер - проверка где мы(на кустике или на земле) 
            var size = _head.position.y - _foot.position.y;
            _controller.height = size+.4f;
            _controller.center = _root.localPosition;
        }
        

        private void CheckGround()
        {
            //Проверка что мы стоим на земле
            IsGrounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, LayerMask.GetMask("Ground"));
        }
    }
}