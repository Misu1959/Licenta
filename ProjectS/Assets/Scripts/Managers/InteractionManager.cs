using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private static bool canInteract;

    private void Start() {  canInteract = true; }

    public static void SetInteractionStatus(bool status)
    {
        canInteract = status;
        CraftingManager.instance.ActivateCraftingManager(canInteract);
        PopUpManager.instance.ShowMousePopUp();
    }

    public static bool CanPlayerInteractWithUI()
    {
        return canInteract;
    }

    public static bool CanPlayerInteractWithWorld(bool doPlayerNeedSelectedItem)
    {
        if(!doPlayerNeedSelectedItem)
            return canInteract & !MyMethods.CheckIfMouseIsOverUI() & !InventoryManager.instance.selectedItemSlot.CheckIfItHasItem();
        else
            return canInteract & !MyMethods.CheckIfMouseIsOverUI();

    }

}
