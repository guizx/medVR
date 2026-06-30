using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CC
{
    [CreateAssetMenu(fileName = "Standard Randomizer", menuName = "ScriptableObjects/Randomizer Standard")]
    public class scrObj_Randomizer_Standard : scrObj_Randomizer
    {
        public List<string> hairBlacklistCaucasian;
        public List<string> hairBlacklistAfrican;
        public List<string> hairBlacklistAsian;

        public override IEnumerator randomizeAll(CharacterCustomization script)
        {
            //Reset options
            if (script.LoadAsync && script.GetPresetData(script.CharacterName, out var ogPreset))
            {
                yield return script.ApplyCharacterVarsAsync(ogPreset);
            }
            else script.LoadFromPreset(script.CharacterName);

            if (script.LoadAsync) yield return null;

            var ethnicity = (Ethnicity)Random.Range(0, 3);
            var ageGroup = AgeGroup.Adult;

            for (int i = 0; i < script.HairTables.Count; i++)
            {
                var hair = getRandomHair(script.HairTables[i].Hairstyles.Select(h => h.Name).ToList(), ethnicity);
                script.setHairByName(hair, i);
                if (script.LoadAsync) yield return null;
            }

            var hairColor = getRandomHairColor(ethnicity, ageGroup);
            script.setColorProperty(new CC_Property { propertyName = "_Hair_Tint", colorValue = hairColor }, true);

            var eyeColor = getRandomEyeColor(ethnicity);
            script.setColorProperty(new CC_Property { propertyName = "_Eye_Color", colorValue = eyeColor, materialIndex = -1, meshTag = "Head" }, true);

            if (script.LoadAsync) yield return null;

            //Randomize mod shapes
            var modShapes = new List<string> { "mod_brow_height", "mod_brow_depth", "mod_jaw_height", "mod_jaw_width", "mod_cheeks_size", "mod_cheekbone_size", "mod_nose_height", "mod_nose_width", "mod_nose_out", "mod_nose_size", "mod_mouth_size", "mod_mouth_depth", "mod_mouth_height", "mod_eyes_depth", "mod_eyes_height", "mod_eyes_narrow", "mod_chin_size" };
            for (int i = 0; i < modShapes.Count; i++)
            {
                float val = GenerateNormalRandom(0.2f);
                script.setBlendshapeByName(modShapes[i], val);
            }

            //Random freckles
            float frecklesRand = Mathf.Abs(GenerateNormalRandom(0.5f));
            script.setFloatProperty(new CC_Property { propertyName = "_Freckles_Strength", floatValue = frecklesRand, materialIndex = 0, meshTag = "Head" }, true);

            //Random skin tint
            Color skinColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            float skinRand = Mathf.Abs(GenerateNormalRandom(0.1f));
            skinColor.a = skinRand;
            script.setColorProperty(new CC_Property { propertyName = "_Skin_Tint", colorValue = skinColor, materialIndex = 0 }, true);

            //Random lipstick
            Color lipsColor = new Color(0.8f, 0.2f, 0.2f);
            float lipsRand = Mathf.Abs(GenerateNormalRandom(0.1f));
            lipsColor.a = lipsRand;
            script.setColorProperty(new CC_Property { propertyName = "_Lips_Color", colorValue = lipsColor, materialIndex = 0, meshTag = "Head" }, true);

            if (script.LoadAsync) yield return null;

            //Random face shape
            var faceShapes = new List<string> { "", "shp_head_01", "shp_head_02", "shp_head_03", "shp_head_04", "shp_head_05", "shp_head_06", "shp_head_07", "shp_head_08" };
            foreach (var shape in faceShapes)
            {
                script.setBlendshapeByName(shape, 0);
            }

            string secondaryShape = faceShapes[Random.Range(0, faceShapes.Count)];
            faceShapes.Remove(secondaryShape);
            switch (ethnicity)
            {
                case Ethnicity.Caucasian:
                    faceShapes.Remove("shp_head_01");
                    faceShapes.Remove("shp_head_04");
                    faceShapes.Remove("shp_head_06");
                    faceShapes.Remove("shp_head_07");
                    break;

                case Ethnicity.African:
                    faceShapes.RemoveAt(0);
                    faceShapes.Remove("shp_head_02");
                    faceShapes.Remove("shp_head_03");
                    faceShapes.Remove("shp_head_04");
                    faceShapes.Remove("shp_head_05");
                    faceShapes.Remove("shp_head_06");
                    faceShapes.Remove("shp_head_08");
                    break;

                case Ethnicity.Asian:
                    faceShapes.RemoveAt(0);
                    faceShapes.Remove("shp_head_01");
                    faceShapes.Remove("shp_head_02");
                    faceShapes.Remove("shp_head_03");
                    faceShapes.Remove("shp_head_05");
                    faceShapes.Remove("shp_head_07");
                    faceShapes.Remove("shp_head_08");
                    break;

                case Ethnicity.Other:
                default:
                    break;
            }
            string mainShape = faceShapes[Random.Range(0, faceShapes.Count)];
            float secondaryRand = Mathf.Abs(GenerateNormalRandom(0.33f));
            script.setBlendshapeByName(secondaryShape, secondaryRand);
            script.setBlendshapeByName(mainShape, 1 - secondaryRand);

            if (script.LoadAsync) yield return null;

            //Set random skin texture
            var headTextures = new List<string> { "T_Skin_Head_01", "T_Skin_Head_02", "T_Skin_Head_03" };
            var bodyTextures = new List<string> { "T_Skin_Body_01", "T_Skin_Body_02", "T_Skin_Body_03" };

            int selectedTexture = 0;
            float rand = Random.Range(0f, 1f);

            switch (ethnicity)
            {
                case Ethnicity.Caucasian:
                    break;

                case Ethnicity.African:
                    selectedTexture = (rand > 0.5f) ? 1 : 2;
                    break;

                case Ethnicity.Other:
                    selectedTexture = (rand > 0.5f) ? 0 : 2;
                    break;

                case Ethnicity.Asian:
                    selectedTexture = (rand > 0.25f) ? 0 : 2;
                    break;

                default:
                    break;
            }

            script.setTextureProperty(new CC_Property { propertyName = "_Color_Map", stringValue = headTextures[selectedTexture], meshTag = "Head", materialIndex = 0 }, true);
            script.setTextureProperty(new CC_Property { propertyName = "_Color_Map", stringValue = bodyTextures[selectedTexture], meshTag = "Body", materialIndex = 0 }, true);

            //Set random height and weight
            script.setBlendshapeByName("BodyCustomization_Height", GenerateNormalRandom(0.33f), true);
            script.setBlendshapeByName("BodyCustomization_Weight", GenerateNormalRandom(0.33f));

            if (script.LoadAsync) yield return null;

            //Set default outfit
            script.setApparelByName("UpperBody_Default", 0, 0);
            if (script.LoadAsync) yield return null;
            script.setApparelByName("LowerBody_Default", 1, 0);
            if (script.LoadAsync) yield return null;
            script.setApparelByName("Footwear_Default", 2, 0);
            if (script.LoadAsync) yield return null;
            script.setApparelByName("Headwear_Default", 3, 0);
        }

        private enum Ethnicity
        {
            Caucasian, African, Asian, Other
        }

        private enum AgeGroup
        {
            Young, Adult, Elderly
        }

        private enum EyeColor
        {
            LightBrown, MediumBrown, DarkBrown, Amber, Hazel, Green, LightBlue, DarkBlue
        }

        private enum HairColor
        {
            LightBrown, MediumBrown, DarkBrown, Blonde, Black, LightGray, DarkGray
        }

        private Color getRandomHairColor(Ethnicity ethnicity, AgeGroup ageGroup)
        {
            var availableColors = new List<HairColor>();
            var elderlyOnly = new List<HairColor>() { HairColor.LightGray, HairColor.DarkGray };
            var youngOnly = new List<HairColor>() { HairColor.LightBrown, HairColor.MediumBrown, HairColor.Blonde };

            switch (ethnicity)
            {
                case Ethnicity.Caucasian:
                    availableColors = new List<HairColor>() { HairColor.LightBrown, HairColor.MediumBrown, HairColor.MediumBrown, HairColor.DarkBrown, HairColor.DarkBrown, HairColor.Black, HairColor.Blonde, HairColor.LightGray, HairColor.DarkGray };
                    if (ageGroup == AgeGroup.Elderly) availableColors = availableColors.Except(youngOnly).ToList();
                    else availableColors = availableColors.Except(elderlyOnly).ToList();
                    return getHairColor(availableColors[Random.Range(0, availableColors.Count)]);

                case Ethnicity.Asian:
                    availableColors = new List<HairColor>() { HairColor.DarkBrown, HairColor.DarkBrown, HairColor.Black, HairColor.Black, HairColor.LightGray, HairColor.DarkGray };
                    if (ageGroup == AgeGroup.Elderly) availableColors = availableColors.Except(youngOnly).ToList();
                    else availableColors = availableColors.Except(elderlyOnly).ToList();
                    return getHairColor(availableColors[Random.Range(0, availableColors.Count)]);

                case Ethnicity.African:
                case Ethnicity.Other:
                    availableColors = new List<HairColor>() { HairColor.DarkBrown, HairColor.DarkBrown, HairColor.DarkBrown, HairColor.Black, HairColor.Black, HairColor.LightGray, HairColor.DarkGray };
                    if (ageGroup == AgeGroup.Elderly) availableColors = availableColors.Except(youngOnly).ToList();
                    else availableColors = availableColors.Except(elderlyOnly).ToList();
                    return getHairColor(availableColors[Random.Range(0, availableColors.Count)]);

                default:
                    return getHairColor(HairColor.DarkBrown);
            }
        }

        private string getRandomHair(List<string> options, Ethnicity ethnicity)
        {
            List<string> sanitizedOptions = new List<string>();
            if (ethnicity == Ethnicity.Caucasian) sanitizedOptions = options.Except(hairBlacklistCaucasian).ToList();
            if (ethnicity == Ethnicity.African) sanitizedOptions = options.Except(hairBlacklistAfrican).ToList();
            if (ethnicity == Ethnicity.Asian) sanitizedOptions = options.Except(hairBlacklistAsian).ToList();
            if (ethnicity == Ethnicity.Other) sanitizedOptions = options;

            if (options.Count <= 0) return "";

            return sanitizedOptions[Random.Range(0, sanitizedOptions.Count)];
        }

        private Color getRandomEyeColor(Ethnicity ethnicity)
        {
            var availableColors = new List<EyeColor>();

            switch (ethnicity)
            {
                case Ethnicity.Caucasian:
                    availableColors = new List<EyeColor>() { EyeColor.LightBrown, EyeColor.MediumBrown, EyeColor.Amber, EyeColor.Hazel, EyeColor.Green, EyeColor.LightBlue, EyeColor.DarkBlue };
                    return getEyeColor(availableColors[Random.Range(0, availableColors.Count)]);

                case Ethnicity.Asian:
                    availableColors = new List<EyeColor>() { EyeColor.DarkBrown, EyeColor.MediumBrown };
                    return getEyeColor(availableColors[Random.Range(0, availableColors.Count)]);

                case Ethnicity.African:
                case Ethnicity.Other:
                    availableColors = new List<EyeColor>() { EyeColor.DarkBrown, EyeColor.MediumBrown, EyeColor.Amber, EyeColor.Hazel };
                    return getEyeColor(availableColors[Random.Range(0, availableColors.Count)]);

                default:
                    return getEyeColor(EyeColor.MediumBrown);
            }
        }

        private static Color getEyeColor(EyeColor eyeColor)
        {
            Color color;
            switch (eyeColor)
            {
                case EyeColor.LightBrown:
                    ColorUtility.TryParseHtmlString("#875E40", out color);
                    break;

                case EyeColor.MediumBrown:
                    ColorUtility.TryParseHtmlString("#604531", out color);
                    break;

                case EyeColor.DarkBrown:
                    ColorUtility.TryParseHtmlString("#3A2B1F", out color);
                    break;

                case EyeColor.Amber:
                    ColorUtility.TryParseHtmlString("#87763C", out color);
                    break;

                case EyeColor.Hazel:
                    ColorUtility.TryParseHtmlString("#9C9662", out color);
                    break;

                case EyeColor.Green:
                    ColorUtility.TryParseHtmlString("#677851", out color);
                    break;

                case EyeColor.LightBlue:
                    ColorUtility.TryParseHtmlString("#698AA3", out color);
                    break;

                case EyeColor.DarkBlue:
                    ColorUtility.TryParseHtmlString("#4E6373", out color);
                    break;

                default:
                    return Color.black;
            }

            return color;
        }

        private static Color getHairColor(HairColor hairColor)
        {
            Color color;
            switch (hairColor)
            {
                case HairColor.LightBrown:
                    ColorUtility.TryParseHtmlString("#947259", out color);
                    break;

                case HairColor.MediumBrown:
                    ColorUtility.TryParseHtmlString("#604938", out color);
                    break;

                case HairColor.DarkBrown:
                    ColorUtility.TryParseHtmlString("#3A2D22", out color);
                    break;

                case HairColor.Blonde:
                    ColorUtility.TryParseHtmlString("#A18D64", out color);
                    break;

                case HairColor.LightGray:
                    ColorUtility.TryParseHtmlString("#A1A1A1", out color);
                    break;

                case HairColor.DarkGray:
                    ColorUtility.TryParseHtmlString("#4D4D4D", out color);
                    break;

                case HairColor.Black:
                    ColorUtility.TryParseHtmlString("#1A1A1A", out color);
                    break;

                default:
                    return Color.black;
            }
            return color;
        }
    }
}