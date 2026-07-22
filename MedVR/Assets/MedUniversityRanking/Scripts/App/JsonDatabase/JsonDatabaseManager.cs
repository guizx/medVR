using Nato.Singleton;
using UnityEngine;

namespace MedGames
{
    public class JsonDatabaseManager : Singleton<JsonDatabaseManager>
    {
        [field: SerializeField] public UniversityDatabase UniversityDatabase;

        protected override void Awake()
        {
            base.Awake();
            UniversityDatabase.LoadDatabase();
        }
    }
}