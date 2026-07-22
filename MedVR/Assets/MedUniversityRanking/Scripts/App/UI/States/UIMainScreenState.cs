using Nato.StateMachine;
using UnityEngine;

namespace MedGames
{
    public class UIMainScreenState : BaseState<UIStateManager>
    {
        public override void OnStart(UIStateManager manager)
        {
            base.OnStart(manager);
            Manager.UIPanels.MainScreenUI.Enable();
        }
        public override void OnEnd()
        {
            base.OnEnd();
            Manager.UIPanels.MainScreenUI.Disable();
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
