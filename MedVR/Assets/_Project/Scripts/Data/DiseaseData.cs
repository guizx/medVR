using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDisease", menuName = "Game/Disease Data")]
public class DiseaseData : ScriptableObject
{
    public string DiseaseName;
    public List<PatientPhrase> Phrases;
}