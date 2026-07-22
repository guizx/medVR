using Nato.PubSub;
using Nato.Singleton;
using Nato.Sound;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Nato.GameEvents;

namespace Nato.StateMachine
{
    [RequireComponent(typeof(UIPanels))]
    [RequireComponent(typeof(UIStates))]
    public class UIStateManager : Singleton<UIStateManager>
    {
        public UIPanels UIPanels;
        public UIStates States;
        public StateMachine<UIStateManager> StateMachine { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            StateMachine = new(this);
            EventManager<UIStateChangedEvent>.Subscribe(HandleMenuStateChanged);
            Application.targetFrameRate = 60;
        }

        private void OnDestroy()
        {
            EventManager<UIStateChangedEvent>.Unsubscribe(HandleMenuStateChanged);
        }

        private void HandleMenuStateChanged(UIStateChangedEvent @event)
        {
            StateMachine.TransitionTo(@event.NewState);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            StateMachine.TransitionTo(States.TitleScreenState);
        }

        private void Update()
        {
            StateMachine.OnTick();
        }
      
        private IEnumerator SetSelectedButtonCoroutine(Button buttonToSelect)
        {
            yield return new WaitForEndOfFrame();
            SetSelectedButton(buttonToSelect);
        }

        public void SetSelectedButton(Button button)
        {
            if (button == null)
                return;

            EventSystem.current.SetSelectedGameObject(button.gameObject);
            //ButtonSelected buttonSelected = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonSelected>();
            //if (buttonSelected != null)
            //    buttonSelected.UpdateSelectUIPosition(isSelect: true);
        }
    }
}
