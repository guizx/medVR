using Nato.UI;
using UnityEngine.Events;

namespace Nato.StateMachine
{
    public class UIAchievementsState : BaseState<UIStateManager>
    {
        public override void OnStart(UIStateManager manager)
        {
            base.OnStart(manager);
            //Manager.UIPanels.AchievementUI.Enable();
        }
        public override void OnEnd()
        {
            base.OnEnd();
            //Manager.UIPanels.AchievementUI.Disable();
        }
        public override void OnDestroyTick()
        {
            base.OnDestroyTick();
        }

        public override void OnTick()
        {
            base.OnTick();
        }
    }
}