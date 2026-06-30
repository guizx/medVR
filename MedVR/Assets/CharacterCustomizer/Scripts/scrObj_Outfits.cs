using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    public class scrObj_Outfits : ScriptableObject
    {
        public virtual bool GetRandomOutfit(CharacterCustomization script, out List<string> apparelOptions, out List<int> apparelMaterials)
        {
            apparelOptions = null;
            apparelMaterials = null;
            return true;
        }

        public void GetRandomApparel(scrObj_Apparel apparelObj, List<string> choices, out string apparelOption, out int apparelMaterial, out HashSet<int> hiddenSlots)
        {
            hiddenSlots = new HashSet<int>();

            //If apparel obj is null, return empty
            if (apparelObj == null)
            {
                apparelOption = "";
                apparelMaterial = 0;
                return;
            }

            //Get apparel data of choices
            var filteredApparel = apparelObj.Items.Where(item => choices.Contains(item.Name)).ToList();

            //If choices is empty, return empty
            if (filteredApparel.Count < 1)
            {
                apparelOption = "";
                apparelMaterial = 0;
                return;
            }

            //Pick random apparel
            var randomChoice = filteredApparel[Random.Range(0, filteredApparel.Count)];

            //Assign apparel name, random material option and hidden slots
            apparelOption = randomChoice.Name;
            apparelMaterial = Random.Range(0, randomChoice.Materials.Count);
            hiddenSlots.UnionWith(randomChoice.HidesTheseSlots);
            return;
        }
    }
}