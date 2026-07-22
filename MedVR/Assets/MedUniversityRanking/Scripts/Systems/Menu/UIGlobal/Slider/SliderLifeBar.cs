using DG.Tweening;
using Nato.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Nato.UI
{
    public class SliderLifeBar : MonoBehaviour
    {
        private float maxValue;
        private float currentValue;
        private int totalSoundPlays = 80; 
        private float soundPlayInterval;
        private float nextSoundPlayTime;

        [SerializeField] public Image LifeBarImage;
        [SerializeField] private AudioSO lifeUpdateSound;

        public void SetMaxLife(float maxValue)
        {
            this.maxValue = maxValue;
            soundPlayInterval = maxValue / totalSoundPlays;

        }
        public void SetLife(float currentValue)
        {
            this.currentValue = currentValue;
            UpdateSiderBar();
        }

        public void FullLifeBarAnimation(float duration, System.Action callback)
        {
            float value = 0;
            float previousValue = 0;
            nextSoundPlayTime = soundPlayInterval;

            DOTween.To(() => value, x => value = x, maxValue, duration)
                .OnUpdate(() =>
                {
                    float lifeBar = Mathf.Clamp01(value / maxValue);
                    LifeBarImage.fillAmount = lifeBar;

                    if (value >= nextSoundPlayTime)
                    {
                        MasterSound.PlaySFX(lifeUpdateSound);
                        nextSoundPlayTime += soundPlayInterval;
                    }

                    previousValue = value;
                }).OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }

        public void UpdateSiderBar()
        {
            LifeBarImage.fillAmount = (currentValue / maxValue);
        }
    }
}

