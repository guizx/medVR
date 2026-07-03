using UnityEngine;

[CreateAssetMenu(fileName = "NewPatient", menuName = "Game/Patient Data")]

public class PatientData : ScriptableObject
{
    public enum GenreType
    {
        Masculino,
        Feminino
    }

    public string Name;
    public int Age;
    public GenreType Genre;
    public GameObject ModelPrefab;
}
