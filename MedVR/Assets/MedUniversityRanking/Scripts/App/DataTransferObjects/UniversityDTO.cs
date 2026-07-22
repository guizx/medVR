using UnityEngine;

namespace MedGames
{
    [System.Serializable]
    public class UniversityRanking
    {
        public int ID;
        public string IES;
        public string UF;
        public string Municipio;
        public int Pontos;
        public string Campus;
        public string Sigla;
        public string CompleteName;

    }

    [System.Serializable]
    public class UniversityDTO
    {
        public string id;
        public string uf;
        public string municipio;
        public string ies;
        public string campus;
        public string sigla;
        public int pontos;

        public string GetCompleteName(bool removeAccents)
        {
            string completeName = $"{sigla} - {ies}";
            if (removeAccents)
            {
                return StringUtils.RemoveAccents(completeName);
            }
            return completeName;
        }
    }
}