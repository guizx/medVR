namespace EasyPeasyFirstPersonController
{
    using UnityEngine;

    public class PlayerCrouchingState : PlayerBaseState
    {
        public PlayerCrouchingState(FirstPersonController currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            float crouchHeight = ctx.crouchingCharacterControllerHeight;
            ctx.characterController.height = crouchHeight;
            ctx.characterController.center = new Vector3(0, crouchHeight / 2f, 0);
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
            HandleCrouchMovement();

            ctx.targetCameraY = ctx.crouchingCameraHeight;
        }

        public override void ExitState()
        {

        }

        public override void CheckSwitchStates()
        {
            if (!ctx.input.crouch && !ctx.HasCeiling())
            {
                SwitchState(factory.Grounded());
            }
            else if (!ctx.isGrounded)
            {
                SwitchState(factory.Fall());
            }
        }

        private void HandleCrouchMovement()
        {
            Vector2 input = ctx.input.moveInput;
            Vector3 move = ctx.transform.right * input.x + ctx.transform.forward * input.y;
            ctx.characterController.Move(move * ctx.crouchSpeed * Time.deltaTime);

            if (ctx.isGrounded) ctx.moveDirection.y = -10;
            else ctx.moveDirection.y = 0;
            ctx.characterController.Move(new Vector3(0, ctx.moveDirection.y, 0) * Time.deltaTime);
        }
    }
}