using Nato.StateMachine;
using UnityEngine;

namespace MedGames
{
    public class UITitleScreenState : BaseState<UIStateManager>
    {
        public override void OnStart(UIStateManager manager)
        {
            base.OnStart(manager);
            Manager.UIPanels.TitleScreenUI.Enable();
        }
        public override void OnEnd()
        {
            base.OnEnd();
            Manager.UIPanels.TitleScreenUI.Disable();
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
