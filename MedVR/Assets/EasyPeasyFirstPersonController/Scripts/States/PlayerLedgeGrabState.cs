namespace EasyPeasyFirstPersonController
{
    using UnityEngine;

    public class PlayerLedgeGrabState : PlayerBaseState
    {
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float climbTimer;
        private float climbDuration = 0.6f;
        private bool isClimbing;

        public PlayerLedgeGrabState(FirstPersonController currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            ctx.CheckLedge(out targetPosition);
            startPosition = ctx.transform.position;
            ctx.moveDirection = Vector3.zero;
            isClimbing = false;
            climbTimer = 0;
        }

        public override void UpdateState()
        {
            if (!isClimbing)
            {
                if (ctx.useClimbTilt)
                    ctx.targetCameraY = ctx.standingCameraHeight - 0.2f;
                if (ctx.input.jump || ctx.input.moveInput.y > 0) isClimbing = true;
                if (ctx.input.crouch) SwitchState(factory.Fall());
            }
            else
            {
                HandleOrganicClimb();
            }
        }

        public override void ExitState() { }

        public override void CheckSwitchStates() { }

        private void HandleOrganicClimb()
        {
            climbTimer += Time.deltaTime;
            float linearT = Mathf.Clamp01(climbTimer / climbDuration);

            float easedT = linearT * linearT * (3f - 2f * linearT);

            float heightArc = Mathf.Sin(linearT * Mathf.PI) * 0.4f;

            Vector3 targetHorizontal = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);
            Vector3 currentPos = Vector3.Lerp(startPosition, targetHorizontal, easedT);

            float currentHeight = Mathf.Lerp(startPosition.y, targetPosition.y, easedT);

            ctx.transform.position = new Vector3(currentPos.x, currentHeight + heightArc, currentPos.z);

            if (ctx.useClimbTilt)
            {
                ctx.targetTilt = Mathf.Sin(linearT * Mathf.PI) * -7f;
                ctx.targetCameraY = ctx.standingCameraHeight + (Mathf.Sin(linearT * Mathf.PI) * 0.15f);
            }

            if (linearT >= 1f)
            {
                ctx.transform.position = targetPosition;
                SwitchState(factory.Grounded());
            }
        }
    }
}