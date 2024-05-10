using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance { get; private set; }
    
    private bool canInteract;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    private void Start() => canInteract = true; 

    public void SetInteractionStatus(bool status)
    {
        canInteract = status;
        CraftingManager.instance.ActivateCraftingManager(canInteract);
    }

    public bool CanPlayerInteractWithUI() => canInteract;
    public bool CanPlayerInteractWithWorld(bool doPlayerNeedSelectedItem)
    {
        if(!doPlayerNeedSelectedItem)
            return canInteract & !MyMethods.CheckIfMouseIsOverUI() & !InventoryManager.instance.selectedItemSlot.CheckIfItHasItem();
        else
            return canInteract & !MyMethods.CheckIfMouseIsOverUI();

    }

}
