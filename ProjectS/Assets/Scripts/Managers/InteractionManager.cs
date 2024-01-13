using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static bool canInteract { get; private set; }


    private void Start()
    {
        canInteract = true;
    }

    public static void SetInteractionStatus(bool status)
    {
        canInteract = status;

        //CraftingManager.instance.ActivateCraftingButtons(status);
    }
}
