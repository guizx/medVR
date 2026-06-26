namespace EasyPeasyFirstPersonController
{
    using UnityEngine;

    public class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(FirstPersonController currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            ctx.moveDirection.y = -2f;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();

            ctx.targetCameraY = ctx.standingCameraHeight;

            bool isSprinting = ctx.input.sprint && ctx.input.moveInput.y > 0;
            float speed = isSprinting ? ctx.sprintSpeed : ctx.walkSpeed;

            ctx.targetFov = isSprinting ? ctx.sprintFov : ctx.normalFov;
            ctx.currentBobIntensity = ctx.bobAmount * (isSprinting ? 1.5f : 1f);
            ctx.currentBobSpeed = ctx.bobSpeed * (isSprinting ? 1.3f : 1f);
            ctx.targetTilt = 0;

            ctx.targetCameraY = ctx.standingCameraHeight;

            ctx.characterController.height = Mathf.MoveTowards(
                ctx.characterController.height,
                ctx.standingCharacterControllerHeight,
                Time.deltaTime * 5f
            );

            ctx.characterController.center = Vector3.MoveTowards(
                ctx.characterController.center,
                ctx.standingCharacterControllerCenter,
                Time.deltaTime * 2.5f
            );

            Vector2 input = ctx.input.moveInput;
            Vector3 move = ctx.transform.right * input.x + ctx.transform.forward * input.y;
            Vector3 finalVelocity = move * speed;
            finalVelocity.y = -20f;
            ctx.characterController.Move(finalVelocity * Time.deltaTime);
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (ctx.input.jump && ctx.isGrounded)
            {
                SwitchState(factory.Jumping());
            }
            else if (ctx.input.slide && ctx.input.sprint)
            {
                SwitchState(factory.Sliding());
            }
            else if (!ctx.isGrounded)
            {
                SwitchState(factory.Fall());
            }
            else if (ctx.input.crouch && ctx.isGrounded)
            {
                SwitchState(factory.Crouching());
            }
            else if (ctx.isInWater)
            {
                SwitchState(factory.Swimming());
            }

        }
    }
}