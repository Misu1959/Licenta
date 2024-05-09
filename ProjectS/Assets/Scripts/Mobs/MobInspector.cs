using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MobStats))]
public class MobInspector : BaseInspector
{
    public void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;


        if (CanPlayerInteractWithMob())
            PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.low);
        else
            PopUpManager.instance.ShowMousePopUp("Wheel - inspect", PopUpManager.PopUpPriorityLevel.low);
    }

    private bool CanPlayerInteractWithMob() => (PlayerStats.instance.GetActualDamage() == 0) ? false : true;
}
