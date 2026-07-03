using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDisease", menuName = "Game/Disease Data")]
public class DiseaseData : ScriptableObject
{
    public enum EmergencyCodeType
    {
        Azul,
        Verde,
        Amarelo,
        Vermelho
    }

    public string DiseaseName;
    public string[] DiseaseTriage;
    public EmergencyCodeType EmergencyCode;
    public List<PatientPhrase> Phrases;

    public string GetEmergencyCodeType() => EmergencyCode.ToString();
}