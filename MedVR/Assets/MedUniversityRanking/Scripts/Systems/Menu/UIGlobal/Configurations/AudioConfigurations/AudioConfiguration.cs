using Nato.Sound;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nato.UI.Configurations
{
    public class AudioConfiguration : MonoBehaviour
    {
        private float effectVolume = 1;
        private float musicVolume = 1;

        public Slider effectSlider;
        public Slider musicSlider;

        private int effectValueSlider;
        private int musicValueSlider;

        private static readonly int[] VolumeLevels = { -80, -35, -30, -20, -15, -5, 0 };


        private void OnEnable()
        {
            effectSlider.onValueChanged.AddListener(delegate { OnSoundVolumeChanged(effectSlider); });
            musicSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChanged(musicSlider); });
            Setup();
        }

        private void OnDisable()
        {
            effectSlider.onValueChanged.RemoveListener(delegate { OnSoundVolumeChanged(effectSlider); });
            musicSlider.onValueChanged.RemoveListener(delegate { OnMusicVolumeChanged(musicSlider); });
        }

        private void Setup()
        {
            effectVolume = MasterSound.GetSavedVolume(MasterSound.AUDIO_OUTPUT.EFFECT);
            musicVolume = MasterSound.GetSavedVolume(MasterSound.AUDIO_OUTPUT.MUSIC);

            effectValueSlider = GetSliderValue(effectVolume);
            musicValueSlider = GetSliderValue(musicVolume);

            MasterSound.SetVolume(effectVolume, MasterSound.AUDIO_OUTPUT.EFFECT);
            MasterSound.SetVolume(musicVolume, MasterSound.AUDIO_OUTPUT.MUSIC);

            effectSlider.maxValue = VolumeLevels.Length - 1;
            effectSlider.minValue = 0;
            effectSlider.wholeNumbers = true;

            musicSlider.maxValue = VolumeLevels.Length - 1;
            musicSlider.minValue = 0;
            musicSlider.wholeNumbers = true;

            effectSlider.value = effectValueSlider;
            musicSlider.value = musicValueSlider;
        }
        private void OnSoundVolumeChanged(Slider slider)
        {
            int volume = GetVolume(slider.value);
            MasterSound.SetVolume(volume, MasterSound.AUDIO_OUTPUT.EFFECT);
            MasterSound.SaveVolume(volume, MasterSound.AUDIO_OUTPUT.EFFECT);

            //if (slider.gameObject.activeInHierarchy)
            //    MasterSound.PlaySFX(GlobalAsset.Instance?.VolumeChanged);
        }
        private void OnMusicVolumeChanged(Slider slider)
        {
            int volume = GetVolume(slider.value);
            MasterSound.SetVolume(volume, MasterSound.AUDIO_OUTPUT.MUSIC);
            MasterSound.SaveVolume(volume, MasterSound.AUDIO_OUTPUT.MUSIC);
        }


        public int GetVolume(float sliderValue)
        {
            int index = Mathf.Clamp((int)sliderValue, 0, VolumeLevels.Length - 1);
            return VolumeLevels[index];
        }

        public int GetSliderValue(float volume)
        {
            int index = Array.IndexOf(VolumeLevels, (int)volume);
            return index >= 0 ? index : VolumeLevels.Length - 1;
        }

        //public int GetVolume(float sliderValue)
        //{
        //    int volume = 0;
        //    switch (sliderValue)
        //    {
        //        case 0:
        //            volume = -80;
        //            break;
        //        case 1:
        //            volume = -35;
        //            break;
        //        case 2:
        //            volume = -30;
        //            break;
        //        case 3:
        //            volume = -20;
        //            break;
        //        case 4:
        //            volume = -15;
        //            break;
        //        case 5:
        //            volume = -5;
        //            break;
        //        case 6:
        //            volume = 0;
        //            break;
        //        default:
        //            volume = 0;
        //            break;
        //    }
        //    return volume;

        //}

        //public int GetSliderValue(float volume)
        //{
        //    int value = 0;
        //    switch (volume)
        //    {
        //        case -80:
        //            value = 0;
        //            break;
        //        case -35:
        //            value = 1;
        //            break;
        //        case -30:
        //            value = 2;
        //            break;
        //        case -20:
        //            value = 3;
        //            break;
        //        case -15:
        //            value = 4;
        //            break;
        //        case -5:
        //            value = 5;
        //            break;
        //        case 0:
        //            value = 6;
        //            break;
        //        default:
        //            value = 6;
        //            break;
        //    }
        //    return value;
        //}
    }
}