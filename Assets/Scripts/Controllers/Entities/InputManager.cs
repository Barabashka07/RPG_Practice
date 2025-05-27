using System;
using UnityEngine;

namespace Controllers.Entities
{   
    public class InputManager : MonoBehaviour
    {   
        // private bool _isPaused;
        private InputSystem_Actions _inputSystem;
        
        private static InputManager _instance;
        
        public static InputManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindAnyObjectByType<InputManager>();
                if (_instance != null) return _instance;
                var go = new GameObject("InputManager");
                _instance = go.AddComponent<InputManager>();

                return _instance;
            }
        }

        #region PlayerInput
        public Vector2 MoveInput {get; private set;}
        public bool JumpInput {get;  set;}
        public bool SprintInput {get; private set;}
        public bool MeleeInput {get; private set;}
        public bool SpellInput { get; private set; }
        public bool RMBInput { get; private set; }
        public bool IsSheathed { get; private set; }

        #endregion

        #region GameInput

        public bool MenuInput { get; private set; }

        #endregion
        
        private void Awake()
        {
            _inputSystem = new InputSystem_Actions();
            
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }


            PlayerActionsInit();
            GameActionsInit();
        }



        private void PlayerActionsInit()
        {
            _inputSystem.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _inputSystem.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
            _inputSystem.Player.Jump.performed += ctx => JumpInput = true;
            _inputSystem.Player.Jump.canceled += ctx => JumpInput = false;
            _inputSystem.Player.Sprint.performed += ctx => SprintInput = true;
            _inputSystem.Player.Sprint.canceled += ctx => SprintInput = false;
            _inputSystem.Player.Attack.performed += ctx => MeleeInput = true;
            _inputSystem.Player.Attack.canceled += ctx => MeleeInput = false;
            _inputSystem.Player.Spell.performed += ctx => SpellInput = true;
            _inputSystem.Player.Spell.canceled += ctx => SpellInput = false;
            _inputSystem.Player.RMB.performed += ctx => RMBInput = true;
            _inputSystem.Player.RMB.canceled += ctx => RMBInput = false;
            _inputSystem.Player.Sheath.performed += ctx => IsSheathed = !IsSheathed;
        }
        
        
        private void GameActionsInit()
        {
            _inputSystem.Game.Menu.performed += ctx => MenuInput = !MenuInput;
        }
        

        private void OnEnable()
        {
            _inputSystem.Enable();
        }
        
    
        private void OnDisable()
        {
            _inputSystem.Disable();
        }
    }
}