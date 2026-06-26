using UnityEngine;

[System.Serializable]
public class PatientPhrase
{
    public AudioClip PhraseSFX;
    [TextArea] public string Phrase;
    public SymptomOption[] Options;
}
