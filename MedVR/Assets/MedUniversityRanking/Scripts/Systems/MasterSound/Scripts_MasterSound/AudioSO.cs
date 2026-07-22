using System.ComponentModel;
using UnityEngine;

namespace Nato.Sound
{
    [CreateAssetMenu(fileName = "NewAudio", menuName = "Nato/MasterSound/Audio", order = 1)]
    public class AudioSO : ScriptableObject
    {
        public AudioClip Clip => Audios[Random.Range(0, Audios.Length)];
        public AudioClip[] Audios;

        [Range(0, 1)] public float Volume = 1;
        public Vector2 Pitch = new Vector2(1, 1);
        public bool Loop = false;
        public string Name;
        public string Author;
        public License License;
    }

    public enum LicenseEnum
    {
        CC0,
        CCBY
    }
}