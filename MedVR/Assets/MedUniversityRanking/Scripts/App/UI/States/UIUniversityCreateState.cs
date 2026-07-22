using Nato.StateMachine;
using UnityEngine;

namespace MedGames
{
    public class UIUniversityCreateState : BaseState<UIStateManager>
    {
        public override void OnStart(UIStateManager manager)
        {
            base.OnStart(manager);
            Manager.UIPanels.UniversityCreateUI.Enable();
        }
        public override void OnEnd()
        {
            base.OnEnd();
            Manager.UIPanels.UniversityCreateUI.Disable();
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
