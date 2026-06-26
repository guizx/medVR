namespace EasyPeasyFirstPersonController
{
    using UnityEngine;

    public class PlayerSlidingState : PlayerBaseState
    {
        private float slideTimer;
        private Vector3 slideDirection;
        public PlayerSlidingState(FirstPersonController currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            slideTimer = ctx.slideDuration;

            float crouchHeight = ctx.crouchingCharacterControllerHeight;
            ctx.characterController.height = crouchHeight;
            ctx.characterController.center = new Vector3(0, crouchHeight / 2f, 0);

            slideDirection = ctx.transform.forward;
        }

        public override void UpdateState()
        {
            slideTimer -= Time.deltaTime;

            float progress = slideTimer / ctx.slideDuration;

            ctx.targetFov = ctx.sprintFov + (ctx.slideFovBoost * progress);
            ctx.currentBobIntensity = 0;
            ctx.targetTilt = -5f * progress;
            ctx.targetCameraY = ctx.crouchingCameraHeight;

            HandleSlideMovement(progress);
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (slideTimer <= 0 || !ctx.isGrounded)
            {
                if (ctx.HasCeiling() || ctx.input.crouch)
                {
                    SwitchState(factory.Crouching());
                }
                else
                {
                    SwitchState(factory.Grounded());
                }
            }
        }

        private void HandleSlideMovement(float progress)
        {
            float speedCurve = Mathf.Pow(progress, 0.5f);
            float speed = ctx.slideSpeed * Mathf.Lerp(0.5f, 1f, speedCurve);

            ctx.characterController.Move(slideDirection * speed * Time.deltaTime);

            if (ctx.isGrounded) ctx.moveDirection.y = -20f;
            else ctx.moveDirection.y = 0;
            ctx.characterController.Move(new Vector3(0, ctx.moveDirection.y, 0) * Time.deltaTime);
        }
    }
}