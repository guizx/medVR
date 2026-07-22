using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nato
{
    public class Initializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            GameObject essentials = Object.Instantiate(Resources.Load("Essentials")) as GameObject;
            Object.DontDestroyOnLoad(essentials);
        }
    }
}
