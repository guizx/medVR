using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    public class scrObj_Randomizer : ScriptableObject
    {
        public virtual IEnumerator randomizeAll(CharacterCustomization script)
        {
            yield break;
        }

        public static float GenerateNormalRandom(float stdDev, float scale = 1, float bias = 0)
        {
            float u1 = 1.0f - Random.Range(0.0f, 1.0f);
            float u2 = 1.0f - Random.Range(0.0f, 1.0f);
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
            float randNormal = Mathf.Clamp(stdDev * randStdNormal, -1, 1);

            return randNormal * scale + bias;
        }
    }
}