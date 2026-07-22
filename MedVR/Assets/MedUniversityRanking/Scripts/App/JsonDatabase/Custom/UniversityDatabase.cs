using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace MedGames
{
    public class UniversityDatabase : MonoBehaviour
    {
        [SerializeField] private string fileName = "universities.json";

        public List<UniversityDTO> Universities = new List<UniversityDTO>();
        [SerializeField] private string currentPath;

        public UniversityDTO UniversitySelected { get; private set; }
        public string UF { get; private set; }

        public void SetCurrentUF(string uf)
        {
            UF = uf;
        }

        public void SetUniversitySelected(UniversityDTO university)
        {
            UniversitySelected = university;
        }

        // public List<UniversityDTO> LoadDatabase()
        // {
        //     string json = JsonLoader.LoadJsonFile(fileName, out currentPath);
        //     if (!string.IsNullOrEmpty(json))
        //         Universities = JsonUtilityWrapper.FromJson<UniversityDTO>(json);

        //     Debug.Log($"Load university database: {Universities.Count}");
        //     return Universities;
        // }

        public List<UniversityDTO> LoadDatabase()
        {
            StartCoroutine(LoadDataInternal());
            if (Universities == null || Universities.Count == 0)
            {
                Debug.LogWarning("LoadDatabase chamado, mas a lista ainda está vazia. O carregamento assíncrono pode não ter terminado.");
            }
            return Universities;
        }

        private IEnumerator LoadDataInternal()
        {
            string pathInPersistent = Path.Combine(Application.persistentDataPath, fileName);
            string pathInStreaming = Path.Combine(Application.streamingAssetsPath, fileName);
            string json = "";

            if (File.Exists(pathInPersistent))
            {
                json = File.ReadAllText(pathInPersistent);
                Debug.Log("Carregando de: " + pathInPersistent);
            }
            else
            {
                if (pathInStreaming.Contains("://") || pathInStreaming.Contains(":///"))
                {
                    using (UnityWebRequest www = UnityWebRequest.Get(pathInStreaming))
                    {
                        yield return www.SendWebRequest();
                        if (www.result == UnityWebRequest.Result.Success)
                            json = www.downloadHandler.text;
                        else
                            Debug.LogError("Erro ao carregar do StreamingAssets: " + www.error);
                    }
                }
                else if (File.Exists(pathInStreaming))
                {
                    json = File.ReadAllText(pathInStreaming);
                }
            }

            if (!string.IsNullOrEmpty(json))
            {
                Universities = JsonUtilityWrapper.FromJson<UniversityDTO>(json);
                Debug.Log("Database carregada. Total: " + Universities.Count);
            }
        }

        // public void SaveDatabase()
        // {
        //     string json = JsonUtilityWrapper.ToJson(Universities, true);
        //     File.WriteAllText(JsonLoader.GetStreamingPath(fileName), json);
        // }


        public void SaveDatabase()
        {
            // SEMPRE salvar no persistentDataPath
            string path = Path.Combine(Application.persistentDataPath, fileName);

            string json = JsonUtilityWrapper.ToJson(Universities, true);

            try
            {
                File.WriteAllText(path, json);
                Debug.Log($"Dados salvos com sucesso em: {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao salvar banco de dados: {e.Message}");
            }
        }

        public void AddUniversity(UniversityDTO uni)
        {
            Universities.Add(uni);
            SaveDatabase();
        }

        public void AddScoreAndSave(UniversityDTO currentUniversity)
        {
            currentUniversity.pontos++;
            foreach (var university in Universities)
            {
                if (university.id == currentUniversity.id)
                {
                    university.pontos = currentUniversity.pontos;
                    break;
                }
            }
            SaveDatabase();
        }

        public void AddScoreAndSave(UniversityDTO currentUniversity, int points)
        {
            currentUniversity.pontos += points;
            foreach (var university in Universities)
            {
                if (university.id == currentUniversity.id)
                {
                    university.pontos = currentUniversity.pontos;
                    break;
                }
            }
            SaveDatabase();
        }

        public void ResetAllScores()
        {
            foreach (var university in Universities)
                university.pontos = 0;

            SaveDatabase();
        }

        public List<UniversityDTO> GetUniversitiesByUF(string uf)
        {
            string ufNormalized = uf.Trim().ToUpper();

            return Universities
                .Where(u => u.uf != null && u.uf.Trim().ToUpper() == ufNormalized)
                .ToList();
        }
    }
}