using System;
using System.IO;
using UnityEngine;

namespace Nato.SaveLoad
{
    public class JsonSave : MonoBehaviour
    {
        private static string SAVE_FOLDER = $"{Application.streamingAssetsPath}/";
        private const string SAVE_EXTENSION = ".json";

        public const string DEFAULT_CONFIGURATION_SAVEFILE = "configurations";
        public const string DEFAULT_ACHIEVEMENTS_SAVEFILE = "achievements";
        public const string DEFAULT_REBINDS_SAVEFILE = "rebinds";
        public const string DEFAULT_GAME_SAVEFILE = "save";

        static JsonSave()
        {
            // Cria a pasta de salvamento se não existir
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }

        /// <summary>
        /// Salva os dados no formato JSON.
        /// </summary>
        public static void Save<T>(string dataName, T data)
        {
            string path = $"{SAVE_FOLDER}{dataName}{SAVE_EXTENSION}";

            try
            {
                string jsonData = JsonUtility.ToJson(data, true); // Formata o JSON para melhor leitura
                File.WriteAllText(path, jsonData);
                Debug.Log($"Data saved to {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save data: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se existe um arquivo de salvamento.
        /// </summary>
        public static bool HasSave(string dataName)
        {
            string path = $"{SAVE_FOLDER}{dataName}{SAVE_EXTENSION}";
            return File.Exists(path);
        }

        /// <summary>
        /// Carrega os dados no formato JSON.
        /// </summary>
        public static T Load<T>(string dataName) where T : new()
        {
            string path = $"{SAVE_FOLDER}{dataName}{SAVE_EXTENSION}";

            if (File.Exists(path))
            {
                try
                {
                    string jsonData = File.ReadAllText(path);
                    T data = JsonUtility.FromJson<T>(jsonData);
                    Debug.Log($"Data loaded from {path}");
                    return data;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load data: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"No save found with name: {dataName} on path {path}. Returning a new instance.");
            }

            // Retorna uma nova instância se não houver salvamento ou em caso de erro
            return new T();
        }

        /// <summary>
        /// Exclui um arquivo de salvamento.
        /// </summary>
        public static void DeleteSave(string dataName, Action callback = null)
        {
            string path = $"{SAVE_FOLDER}{dataName}{SAVE_EXTENSION}";

            if (File.Exists(path))
            {
                File.Delete(path);
                callback?.Invoke();
                Debug.Log($"Save deleted: {path}");
            }
            else
            {
                Debug.LogWarning($"No save found to delete: {path}");
            }
        }
    }
}
