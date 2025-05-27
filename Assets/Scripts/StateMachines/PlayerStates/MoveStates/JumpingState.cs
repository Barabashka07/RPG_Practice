using System.Collections;
using Anims;
using Controllers.Entities;
using UnityEngine;

namespace StateMachines.PlayerStates.MoveStates
{
    public class JumpingState : MovementPlayerState
    {
        public JumpingState(MovementController movementController, PlayerAnimator animator) : base(movementController, animator)
        {
        }

        public override void Enter()
        {
            MonoBehaviour.print("Entering Jumping State");
            PlayerAnimator.StartCoroutine(JumpRoutine());
            InputManager.Instance.JumpInput = false;
        }

        public override void Execute()
        {
            MovementController.moveSpeed -= 2f * Time.deltaTime;
            MovementController.Move();
        }

        public override void Exit()
        {   
            MonoBehaviour.print("Exiting Jumping State");
        }

        private IEnumerator JumpRoutine()
        {
            PlayerAnimator.DoJump();
            yield return new WaitUntil(() => PlayerAnimator.CheckAnimationState((int)LayerNames.Movement, .63f, "Jump"));
            MovementController.Jump();
            yield return new WaitUntil(() => PlayerAnimator.CheckAnimationState((int)LayerNames.Movement, .99f, "Jump"));
        }
    }
}