using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe : MonoBehaviour
{
    public  GameObject prefabItem;
    public  bool       isLearned;

    [System.Serializable]
    public struct Requiremets
    {
        public string type;
        public int quantity;
    };
    public Requiremets[] requirements;

    public bool CheckIfCanBeCrafted()
    {
        // If player it's not near a station and recipe isn't learned
        if(PlayerStats.instance.researchLevel==0)
            if (!isLearned)
                return false;

        return CheckIfHaveRosources();
    }

    public bool CheckIfHaveRosources()
    {
        for (int i = 0; i < requirements.Length; i++)
            if (InventoryManager.instance.AmountOwnedOfType(requirements[i].type) < requirements[i].quantity)
                return false;

        return true;
    }

}
