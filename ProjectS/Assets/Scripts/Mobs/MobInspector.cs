using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MobStats))]
public class MobInspector : BaseInspector
{
    private MobStats mob;

    private void Start() => mob = GetComponent<MobStats>();

    public void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false))
            return;

        PopUpManager.instance.ShowMousePopUp(hoverText, PopUpManager.PopUpPriorityLevel.low);
    }
}
