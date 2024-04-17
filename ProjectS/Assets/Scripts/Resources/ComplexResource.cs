using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexResource : Resource
{
    [SerializeField] EquipmentActionType howToGather;
    [SerializeField] private float maxHp;
    private float hp;

    protected override void Start()
    {
        base.Start();
        hp = maxHp;
    }

    public override void SetToGather()
    {
        if (howToGather == EquipmentActionType.chop)
            PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.chop);
        else if (howToGather == EquipmentActionType.mine)
            PlayerBehaviour.instance.SetTargetAndAction(this.transform, PlayerBehaviour.Action.mine);
    }

    public override void GatherItemOfType()
    {

        // If player isn't harvesting it or if it's not grown
        if (!PlayerBehaviour.instance.IsGathering(this.transform))
        {
            gatherTimer.RestartTimer();
            return;
        }

        gatherTimer.StartTimer();
        gatherTimer.Tick();
        if (!gatherTimer.IsElapsed())
            return;

        TakeDmg();
        EquipmentManager.instance.GetHandItem()?.GetComponent<EquipmentUI>().UseTool();

        if(!EquipmentManager.instance.GetHandItem()) // If there is no equipment stop action
            PlayerBehaviour.instance.CompleteAction();

        if (!Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0)) // If player don't hold space or LMB
            PlayerBehaviour.instance.CompleteAction();

    }

    void TakeDmg()
    {
        hp--;
        SetAnim();

        if (hp <= 0)
            DestroyResource();
    }

    void DestroyResource()
    {
        GetComponent<LootManagement>().DropLoot();

        PlayerBehaviour.instance.CompleteAction(); // Complete the action
        Destroy(this.gameObject); // Destroy this object

    }

    public override bool CheckIfCanBeGathered() => EquipmentManager.instance.GetHandItem()?.GetEquipmentData().actionType != howToGather ? false : true;

    public override void SetAnim()
    {
        animator.SetTrigger("PlayerInput");
        if (hp <= 0)
            animator.SetInteger("Stage", (int)GrowthStages.dead);
        else if (hp < .34f * maxHp)
            animator.SetInteger("Stage", (int)GrowthStages.barrelyFull);
        else if (hp < .66f * maxHp)
            animator.SetInteger("Stage", (int)GrowthStages.almostFull);
        
    }
}
