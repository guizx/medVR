using UnityEngine;

[CreateAssetMenu(fileName = "NovoAutor", menuName = "Dialogo/Autor")]
public class SpeakerData : ScriptableObject {
    public string Name;
    public Color SpeakerColor = Color.white;
}