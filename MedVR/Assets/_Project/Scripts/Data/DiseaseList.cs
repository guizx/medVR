using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiseaseList", menuName = "Game/Disease List")]
public class DiseaseList : ScriptableObject
{
    public List<DiseaseData> AllDiseases;
}