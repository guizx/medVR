using CC;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CC
{
    public class CreatePrefab : MonoBehaviour
    {
    #if UNITY_EDITOR
        public string prefabSaveFolder = "CharacterCustomizer/CustomPrefabs";
        public bool resetPose = true;
        public string suffix = "_Prefab";

        public void createPrefab(string name, GameObject orig, string prefabSource)
        {
            string prefabFolder = Path.Combine("Assets/" + prefabSaveFolder + "/" + name);
            string matFolder = prefabFolder + "/Materials";

            if (!Directory.Exists(prefabFolder)) Directory.CreateDirectory(prefabFolder);
            if (!Directory.Exists(matFolder)) Directory.CreateDirectory(matFolder);

            //Delete old materials
            string[] matFiles = Directory.GetFiles(matFolder, "*.mat", SearchOption.TopDirectoryOnly);
            foreach (var file in matFiles)
            {
                // Convert system path to Unity asset path
                string assetPath = file.Replace("\\", "/");
                AssetDatabase.DeleteAsset(assetPath);
            }

            string prefabPath = prefabFolder + "/" + name + suffix + ".prefab";

            //Duplicate the GO for modifying
            GameObject temp = Instantiate(orig);
            temp.name = name;

            //All renderers in the clone
            Renderer[] renderers = temp.GetComponentsInChildren<Renderer>(true);

            // Map "material key" -> duplicated asset
            Dictionary<string, Material> materialMap = new Dictionary<string, Material>();

            foreach (var rend in renderers)
            {
                var src = rend.sharedMaterials;
                if (src == null || src.Length == 0) continue;

                var dst = new Material[src.Length];

                for (int i = 0; i < src.Length; i++)
                {
                    var m = src[i];
                    if (m == null)
                    {
                        dst[i] = null;
                        continue;
                    }

                    string key = buildMaterialKey(m);

                    if (!materialMap.TryGetValue(key, out Material dup))
                    {
                        //Name file from the base (non-instance) name
                        string baseName = getBaseName(m.name);
                        string fileName = sanitizeFileName(baseName) + "_Copy.mat";
                        string matPath = AssetDatabase.GenerateUniqueAssetPath(matFolder + "/" + fileName);

                        dup = new Material(m); //copies all properties/keywords/shader
                        AssetDatabase.CreateAsset(dup, matPath);
                        materialMap[key] = dup;
                    }

                    dst[i] = dup;
                }

                rend.sharedMaterials = dst;
            }

            //Remove unneeded scripts
            DestroyImmediate(temp.GetComponentInChildren<CharacterCustomization>(true));
            DestroyImmediate(temp.GetComponentInChildren<ModifyBone_Manager>(true));
            foreach (var modifyBone in temp.GetComponentsInChildren<ModifyBone>(true))
            {
                DestroyImmediate(modifyBone);
            }
            foreach (var copyPose in temp.GetComponentsInChildren<CopyPose>(true))
            {
                DestroyImmediate(copyPose);
            }
            foreach (var blendshapeManager in temp.GetComponentsInChildren<BlendshapeManager>(true))
            {
                DestroyImmediate(blendshapeManager);
            }
            foreach (var headColliders in temp.GetComponentsInChildren<HeadColliders>(true))
            {
                foreach (var GO in headColliders.colliderObjects)
                {
                    if (GO != null) DestroyImmediate(GO);
                }
            }
            DestroyImmediate(temp.GetComponentInChildren<HeadColliders>(true));

            //Disable customization mode in physics manager
            var physicsManager = temp.GetComponentInChildren<PhysicsManager>();
            if (physicsManager != null) { physicsManager.customizing = false; };

            // Reset bones back to prefab's original transforms
            if (resetPose && !string.IsNullOrEmpty(prefabSource)) resetBones(temp, prefabSource);

            // Save the prefab from the modified clone
            PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);
            DestroyImmediate(temp);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Prefab created (overwritten if existed): {prefabPath}");
        }

        private static string buildMaterialKey(Material m)
        {
            string path = AssetDatabase.GetAssetPath(m);
            string shaderName = m.shader ? m.shader.name : "<no-shader>";

            if (!string.IsNullOrEmpty(path))
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                return $"GUID:{guid}|SH:{shaderName}";
            }

            // Non-asset instance (e.g., created by renderer.material). Group by base name + shader.
            string baseName = getBaseName(m.name);
            return $"NAME:{baseName}|SH:{shaderName}";
        }

        private static string getBaseName(string name)
        {
            const string inst = " (Instance)";
            return name.EndsWith(inst) ? name.Substring(0, name.Length - inst.Length) : name;
        }

        private static string sanitizeFileName(string s)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                s = s.Replace(c.ToString(), "_");
            return s;
        }

        private void resetBones(GameObject go, string prefabSource)
        {
            if (string.IsNullOrEmpty(prefabSource))
            {
                Debug.LogWarning("Prefab name is null or empty, cannot reset bones.");
                return;
            }

            // Load the original prefab from Resources
            GameObject prefab = Resources.Load<GameObject>(prefabSource);
            if (prefab == null)
            {
                Debug.LogWarning($"Could not load prefab '{prefabSource}' from Resources.");
                return;
            }

            // Instantiate prefab temporarily (hidden in hierarchy)
            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            prefabInstance.hideFlags = HideFlags.HideAndDontSave;

            // Copy transforms recursively by name
            copyTransforms(prefabInstance.transform, go.transform);

            // Destroy the temporary prefab instance
            DestroyImmediate(prefabInstance);
        }


        private static void copyTransforms(Transform source, Transform target)
        {
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;

            // Match children by name
            foreach (Transform srcChild in source)
            {
                var tgtChild = target.Find(srcChild.name);
                if (tgtChild != null)
                    copyTransforms(srcChild, tgtChild);
            }
        }
    #endif
    }
}
