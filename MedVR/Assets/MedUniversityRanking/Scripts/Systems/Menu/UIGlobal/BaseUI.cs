using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nato.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseUI : MonoBehaviour
    {
        [Header("Base UI Settings")]
        [Space(10)]
        [SerializeField, ReadOnly] protected GameObject Panel;
        [SerializeField, ReadOnly] protected CanvasGroup CanvasGroup;

        [SerializeField, ReadOnly] public GameObject LastPanelSelectButton;
        [SerializeField] public GameObject FirstSelectButton;

        [SerializeField] private float fadeDelayEnable = 0f;
        [SerializeField] private float fadeDelayDisable = 0f;

        [SerializeField, ReadOnly] public bool IsEnable = false;

        public UnityEvent OnPanelEnable;
        public UnityEvent OnPanelDisable;



        public enum UINavigationEnum
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            Panel = transform.GetChild(0).gameObject;
            Panel.SetActive(false);
        }

        public virtual void Enable()
        {
            if (Panel == null)
                return;
            OnPanelEnable?.Invoke();
            Panel.SetActive(true);
            CanvasGroup.alpha = 0.1f;
            CanvasGroup.blocksRaycasts = false;

        
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.Append(CanvasGroup.DOFade(1, fadeDelayEnable).SetUpdate(true).OnComplete(() =>
            {
                Panel.SetActive(true);
                CanvasGroup.alpha = 1f;
            }));
            sequence.AppendInterval(0.1f);
            sequence.OnComplete(() =>
            {
                CanvasGroup.blocksRaycasts = true;
                IsEnable = true;
            });

            SetSelectedButton(FirstSelectButton);
        }

        public virtual void Disable()
        {
            if (!Panel.activeInHierarchy)
                return;


            OnPanelDisable?.Invoke();

            IsEnable = false;

            CanvasGroup.alpha = 1;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.DOFade(0, fadeDelayDisable).SetUpdate(true).OnComplete(() =>
            {
                CanvasGroup.blocksRaycasts = false;
                Panel.SetActive(false);
            });
        }

        public void SetLastPanelSelectButton(Button button)
        {
            LastPanelSelectButton = button.gameObject;
        }

        public void SetSelectedButton(GameObject button)
        {
            if (button == null)
                return;


            StartCoroutine(HighlightButtonCoroutine(button));
        }

        private IEnumerator HighlightButtonCoroutine(GameObject button)
        {
            EventSystem.current?.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current?.SetSelectedGameObject(button.gameObject);
            //ButtonSelected buttonSelected = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonSelected>();
            //if (buttonSelected != null)
            //    buttonSelected.UpdateSelectUIPosition(isSelect: true);
        }

        public void SetNavigationButton(Button buttonOrigin, Button buttonEnding, UINavigationEnum navigationEnum)
        {
            Navigation navigation = buttonOrigin.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            switch (navigationEnum)
            {
                case UINavigationEnum.UP:
                    navigation.selectOnUp = buttonEnding;
                    break;
                case UINavigationEnum.DOWN:
                    navigation.selectOnDown = buttonEnding;
                    break;
                case UINavigationEnum.LEFT:
                    navigation.selectOnLeft = buttonEnding;
                    break;
                case UINavigationEnum.RIGHT:
                    navigation.selectOnRight = buttonEnding;
                    break;
            }

            buttonOrigin.navigation = navigation;
        }

    }
}