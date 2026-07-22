using UnityEditor;
using UnityEngine;

namespace Nato.Sound
{
    [CustomEditor(typeof(AudioSO))]
    public class AudioSOEditor : Editor
    {
        private AudioSource audioSource;

        private void OnEnable()
        {
            if (audioSource == null)
            {
                GameObject go = new GameObject("AudioPreview", typeof(AudioSource));
                go.hideFlags = HideFlags.HideAndDontSave;
                audioSource = go.GetComponent<AudioSource>();
            }
        }

        private void OnDisable()
        {
            if (audioSource != null)
            {
                DestroyImmediate(audioSource.gameObject);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AudioSO audioSO = (AudioSO)target;

            if (GUILayout.Button("Preview Audio"))
            {
                PlayAudioPreview(audioSO);
            }

            if (GUILayout.Button("Stop Preview"))
            {
                StopAudioPreview();
            }
        }

        private void PlayAudioPreview(AudioSO audio)
        {
            if (audioSource != null && audio != null && audio.Clip != null)
            {
                StopAudioPreview();
                audioSource.clip = audio.Clip;
                float volumeAudio = (1f * audio.Volume);
                audioSource.volume = volumeAudio;
                audioSource.pitch = Random.Range(audio.Pitch.x, audio.Pitch.y);
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        private void StopAudioPreview()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
    }
}
