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

    public IEnumerator CraftRecipe()
    {
        GameObject craftedItem = Instantiate(prefabItem);

        if (isLearned == false)
            isLearned = true;

        InventoryManager.instance.SetBackToSlot();

        if (!craftedItem.GetComponent<Construction>()) // If the crafted thing is not a construction
        {
            foreach(Requiremets req in requirements)
                InventoryManager.instance.SpendResources(req.type, req.quantity);

            yield return null;
            if (craftedItem.GetComponent<Equipment>())
            {
                craftedItem.GetComponent<Item>().SetType(prefabItem.name);
                craftedItem.GetComponent<Equipment>().SetDurability(-1);
                InventoryManager.instance.AddItemToSlot(craftedItem.GetComponent<Item>());
            }
        }
        else // If the crafted thing is a construction
            PlayerActionManagement.instance.SetTargetAndAction(null, PlayerActionManagement.Action.place); // Set player action to placement mode

        CraftingManager.instance.SetRecipesList(); // Close crafting manager
    }






}
