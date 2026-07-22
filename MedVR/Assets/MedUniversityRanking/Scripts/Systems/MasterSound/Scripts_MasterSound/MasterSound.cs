using Nato.SaveLoad;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Nato.Sound
{
    public class MasterSound : MonoBehaviour
    {
        public static MasterSound Instance;

        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource sfxAudioSourceLoop;
        [SerializeField] private AudioSource[] sfxAudioSources;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup bgmAudioMixerGroup;
        [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;

        private static float currentMusicVolume = 0.0f;
        private static float currentSfxVolume = 0.0f;

        private static IEnumerator sfxLoopCoroutine;

        public enum AUDIO_OUTPUT
        {
            MUSIC,
            EFFECT
        };

        public static readonly string AUDIO_SAVE_KEY = "audio_configuration";

        private const string MUSIC_LOWPASS_PARAMETER = "MUSIC_LOWPASS";
        private const string MUSIC_VOLUME_PARAMETER = "MUSIC_VOLUME";
        private const string EFFECT_VOLUME_PARAMETER = "EFFECT_VOLUME";

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);


            bgmAudioSource = transform.Find("BGM_AudioSource").GetComponent<AudioSource>();
            sfxAudioSourceLoop = transform.Find("SFX_AudioSourceLoop").GetComponent<AudioSource>();
            sfxAudioSourceLoop.loop = true;
            sfxAudioSources = transform.Find("SFX_Group").GetComponentsInChildren<AudioSource>();

            bgmAudioSource.outputAudioMixerGroup = bgmAudioMixerGroup;

            foreach (AudioSource sfxAudioSource in sfxAudioSources)
                sfxAudioSource.outputAudioMixerGroup = sfxAudioMixerGroup;

            SettingAudioVolume();
        }

        private void SettingAudioVolume()
        {
            float musicVolume = GetSavedVolume(AUDIO_OUTPUT.MUSIC);
            SetVolume(musicVolume, AUDIO_OUTPUT.MUSIC);

            float effectVolume = GetSavedVolume(AUDIO_OUTPUT.EFFECT);
            SetVolume(effectVolume, AUDIO_OUTPUT.EFFECT);
        }

        public static float GetSavedVolume(AUDIO_OUTPUT output)
        {
            switch (output)
            {
                case AUDIO_OUTPUT.MUSIC:
                    return currentMusicVolume;
                case AUDIO_OUTPUT.EFFECT:
                    return currentSfxVolume;
            }
            return 0;
        }

        public static void SaveVolume(float volume, AUDIO_OUTPUT output)
        {

            switch (output)
            {
                case AUDIO_OUTPUT.MUSIC:
                    currentMusicVolume = volume;
                    break;
                case AUDIO_OUTPUT.EFFECT:
                    currentSfxVolume = volume;
                    break;
            }

            ConfigurationSaveObject configurationSave = JsonSave.Load<ConfigurationSaveObject>(JsonSave.DEFAULT_CONFIGURATION_SAVEFILE);

            configurationSave.MusicVolume = currentMusicVolume;
            configurationSave.EffectVolume = currentSfxVolume;
            JsonSave.Save(JsonSave.DEFAULT_CONFIGURATION_SAVEFILE, configurationSave);
        }

        public static void UnderLowpassMusicEffect()
        {
            Instance.audioMixer.SetFloat(MUSIC_LOWPASS_PARAMETER, 1000);
        }

        public static void NormalLowpassMusicEffect()
        {
            Instance.audioMixer.SetFloat(MUSIC_LOWPASS_PARAMETER, 22000);
        }

        public static void SetVolume(float volume, AUDIO_OUTPUT output)
        {
            switch (output)
            {
                case AUDIO_OUTPUT.MUSIC:
                    Instance.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER, volume);
                    break;
                case AUDIO_OUTPUT.EFFECT:
                    Instance.audioMixer.SetFloat(EFFECT_VOLUME_PARAMETER, volume);
                    break;
            }
        }


        public static void PlaySFX(AudioSO sfxAudio)
        {
            SetVolume(GetSavedVolume(AUDIO_OUTPUT.EFFECT), AUDIO_OUTPUT.EFFECT);
            if (sfxAudio == null || sfxAudio.Clip == null)
                return;

            foreach (AudioSource sfxAudioSource in Instance.sfxAudioSources)
            {
                if (!sfxAudioSource.isPlaying)
                {
                    float audioVolume = 1 * sfxAudio.Volume;
                    sfxAudioSource.volume = audioVolume;
                    sfxAudioSource.pitch = Random.Range(sfxAudio.Pitch.x, sfxAudio.Pitch.y);
                    sfxAudioSource.PlayOneShot(sfxAudio.Clip);
                    break;
                }
            }
        }

        public static void PlaySFXLoop(AudioSO sfxAudio, float duration, float repeatDelay = 0)
        {
            if (sfxAudio == null || sfxAudio.Clip == null)
                return;
            if (repeatDelay == 0)
                repeatDelay = sfxAudio.Clip.length;

            sfxLoopCoroutine = Instance.SFXLoopCoroutine(sfxAudio, duration, repeatDelay);
            Instance.StartCoroutine(sfxLoopCoroutine);
        }

        private IEnumerator SFXLoopCoroutine(AudioSO sfxAudio, float duration, float repeatDelay)
        {
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                SetVolume(GetSavedVolume(AUDIO_OUTPUT.EFFECT), AUDIO_OUTPUT.EFFECT);
                float audioVolume = 1 * sfxAudio.Volume;
                Instance.sfxAudioSourceLoop.volume = audioVolume;
                Instance.sfxAudioSourceLoop.pitch = Random.Range(sfxAudio.Pitch.x, sfxAudio.Pitch.y);
                Instance.sfxAudioSourceLoop.PlayOneShot(sfxAudio.Clip);

                yield return new WaitForSeconds(repeatDelay);
                elapsedTime += repeatDelay;
            }
        }

        public static void PlayMusic(AudioSO bgmAudio)
        {
            SetVolume(GetSavedVolume(AUDIO_OUTPUT.MUSIC), AUDIO_OUTPUT.MUSIC);
            if (bgmAudio == null || bgmAudio.Clip == null)
                return;
            float audioVolume = 1 * bgmAudio.Volume;
            AudioSource audioSource = Instance.bgmAudioSource;
            audioSource.volume = audioVolume;
            audioSource.clip = bgmAudio.Clip;
            audioSource.loop = true;
            audioSource.Play();
        }

        public static void PlayMusic(AudioSO bgmAudio, bool loop)
        {
            SetVolume(GetSavedVolume(AUDIO_OUTPUT.MUSIC), AUDIO_OUTPUT.MUSIC);
            if (bgmAudio == null || bgmAudio.Clip == null)
                return;
            float audioVolume = 1 * bgmAudio.Volume;
            AudioSource audioSource = Instance.bgmAudioSource;
            audioSource.volume = audioVolume;
            audioSource.clip = bgmAudio.Clip;
            audioSource.loop = loop;
            audioSource.Play();
        }

        public static void PlayLastMusic()
        {
            SetVolume(GetSavedVolume(AUDIO_OUTPUT.MUSIC), AUDIO_OUTPUT.MUSIC);
 
            AudioSource audioSource = Instance.bgmAudioSource;
            if (audioSource == null || audioSource.clip == null)
                return;

            audioSource.loop = true;
            audioSource.Play();
        }


        public static void CrossfadeTo(AudioSO nextClip, float durationFade = 0.5f, float delay = 0.5f)
        {
            Instance.StartCoroutine(Instance.FadeOutIn(nextClip, durationFade, delay));
        }

        public static void BGM_FadeIn(float durationFade = 0.5f, float delay = 0.5f, System.Action callback = null)
        {
            Instance.StartCoroutine(Instance.FadeIn(durationFade, delay, callback));
        }

        public static void BGM_FadeOut(float durationFade = 0.5f, float delay = 0.5f, System.Action callback = null)
        {
            Instance.StartCoroutine(Instance.FadeOut(durationFade, delay, callback));
        }

        private IEnumerator FadeIn(float durationFade, float delay, System.Action callback)
        {
            yield return StartCoroutine(FadeVolume(-80f, durationFade));
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private IEnumerator FadeOut(float durationFade, float delay, System.Action callback)
        {
            float targetVolume = GetSavedVolume(AUDIO_OUTPUT.MUSIC);
            yield return StartCoroutine(FadeVolume(targetVolume, durationFade));
            callback?.Invoke();
        }

        private IEnumerator FadeOutIn(AudioSO nextClip, float durationFade, float delay)
        {
            yield return StartCoroutine(FadeVolume(-80f, durationFade));
            yield return new WaitForSeconds(delay);

            StopMusic();
            PlayMusic(nextClip);
            SetVolume(-80f, AUDIO_OUTPUT.MUSIC);
            float targetVolume = GetSavedVolume(AUDIO_OUTPUT.MUSIC);
            yield return StartCoroutine(FadeVolume(targetVolume, durationFade));
        }

        private IEnumerator FadeVolume(float targetVolume, float duration)
        {
            float currentTime = 0;
            Instance.audioMixer.GetFloat(MUSIC_VOLUME_PARAMETER, out float currentVolume);
            float startVolume = currentVolume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                SetVolume(newVolume, AUDIO_OUTPUT.MUSIC);
                yield return null;
            }
            SetVolume(targetVolume, AUDIO_OUTPUT.MUSIC);
        }

        public static void StopSFX()
        {
            if(sfxLoopCoroutine != null)
                Instance.StopCoroutine(sfxLoopCoroutine);
            foreach (AudioSource sfxAudioSource in Instance.sfxAudioSources)
            {
                sfxAudioSource.Stop();
            }
        }

        public static void StopSFX(AudioSO audio)
        {
            if (sfxLoopCoroutine != null)
                Instance.StopCoroutine(sfxLoopCoroutine);
            foreach (AudioSource sfxAudioSource in Instance.sfxAudioSources)
            {
                if(sfxAudioSource.clip == audio.Clip)
                    sfxAudioSource.Stop();
            }
        }

        public static void StopMusic()
        {
            Instance.StopAllCoroutines();
            Instance.bgmAudioSource.Stop();
        }

        public static void PauseMusic()
        {
            Instance.bgmAudioSource.Pause();
        }

        public static void UnpauseMusic()
        {
            Instance.bgmAudioSource.UnPause();
        }

        public void StopAll()
        {
            StopSFX();
            StopMusic();
        }
    }
}