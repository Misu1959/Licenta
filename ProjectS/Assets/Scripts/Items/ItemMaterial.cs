using UnityEngine;

public class ItemMaterial : Item
{
    [SerializeField]
    private ItemData data;
    public override ItemData GetItemData() {   return data;    }

    public override void SetItemData(ItemData newData) { data = new ItemData(newData); }

    public override void OnRightMouseButtonPressed(){}

    public override void OnMouseOver()
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false))
        {
            PopUpManager.instance.ShowMousePopUp();
            return;
        }

        if (IsOnTheGround())
            PopUpManager.instance.ShowMousePopUp("LMB - Pick");
    }

}