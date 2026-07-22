using Nato.StateMachine;

namespace Nato
{
    public class GameEvents
    {
        public class UIStateChangedEvent
        {
            public BaseState<UIStateManager> NewState { get; private set; }

            public UIStateChangedEvent(BaseState<UIStateManager> newState)
            {
                NewState = newState;
            }
        }
    }
}
