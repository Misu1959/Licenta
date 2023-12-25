using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangeManagement : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (CheckForSpecificCollider(PlayerGatherManager.instance.darknessCollider))
        {
            if (other.gameObject.GetComponent<Light2D>())
                PlayerStats.instance.SetInLight(1);
        }
        if (CheckForSpecificCollider(PlayerGatherManager.instance.actionCollider))
        {
            if (other.gameObject.layer == 10)
                PlayerStats.instance.SetResearchLevel(1);
        }
        if (CheckForSpecificCollider(PlayerGatherManager.instance.searchCollider))
        {
            if (other.GetComponent<Item>() || other.GetComponent<Resource>())
                PlayerGatherManager.instance.itemsInRange.Add(other.gameObject);

        }

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (CheckForSpecificCollider(PlayerGatherManager.instance.playerBody))
            if (other.GetComponent<SpriteRenderer>().sortingOrder > GetComponent<SpriteRenderer>().sortingOrder)
            {
                Color objColor = other.GetComponent<SpriteRenderer>().color;
                other.GetComponent<SpriteRenderer>().color = new Color(objColor.r, objColor.g, objColor.b, .5f);
            }
            else if (other.GetComponent<SpriteRenderer>().sortingOrder <= GetComponent<SpriteRenderer>().sortingOrder)
            {
                Color objColor = other.GetComponent<SpriteRenderer>().color;
                other.GetComponent<SpriteRenderer>().color = new Color(objColor.r, objColor.g, objColor.b, 1);
            }

        if (CheckForSpecificCollider(PlayerGatherManager.instance.actionCollider))
            if (other.gameObject == PlayerGatherManager.instance.target)
            {
                if (PlayerGatherManager.instance.actionType == 1)
                {
                    InventoryManager.instance.AddItemToSlot(other.gameObject);
                    PlayerGatherManager.instance.itemsInRange.Remove(other.gameObject);
                    PlayerGatherManager.instance.SetTarget(null, 0);
                }
                else if (PlayerGatherManager.instance.actionType == 2)
                {
                    PlayerGatherManager.instance.target.GetComponent<Item>().SetTransparent(false);
                    PlayerGatherManager.instance.SetTarget(null, 0);
                }
                else if(PlayerGatherManager.instance.actionType >= 11 && PlayerGatherManager.instance.actionType <= 13)
                {
                    if (!other.gameObject.GetComponent<Resource>().isGathering)
                    {
                        other.gameObject.GetComponent<Resource>().SetIsGathering(true);
                    }
                }
                else if (PlayerGatherManager.instance.actionType == 31)
                {
                    other.gameObject.GetComponent<Fire>().AddFuel(InventoryManager.instance.selectedItem);
                    PlayerGatherManager.instance.SetTarget(null, 0);
                }
                else if (PlayerGatherManager.instance.actionType == 32)
                {
                    if (!InventoryManager.instance.selectedItem.GetComponent<Food>().isCooking)
                    {
                        PlayerGatherManager.instance.target = null;
                        InventoryManager.instance.selectedItem.GetComponent<Food>().SetIsCooking(true);
                    }
                }
                else if (PlayerGatherManager.instance.actionType == 33)
                {
                    other.GetComponent<Food>().Consume();
                    PlayerGatherManager.instance.SetTarget(null, 0);
                }
            }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CheckForSpecificCollider(PlayerGatherManager.instance.playerBody))
        {
            Color objColor = other.GetComponent<SpriteRenderer>().color;
            other.GetComponent<SpriteRenderer>().color = new Color(objColor.r, objColor.g, objColor.b, 1);
        }

        if (CheckForSpecificCollider(PlayerGatherManager.instance.darknessCollider))
        {
            if (other.gameObject.GetComponent<Light2D>())
                PlayerStats.instance.SetInLight(-1);
        }

        if (CheckForSpecificCollider(PlayerGatherManager.instance.actionCollider))
        {
            if (other.GetComponent<Construction>())
            {

                //It has bugs
                if (other.gameObject.layer == 10)
                    PlayerStats.instance.SetResearchLevel(0);

            }
        }

        if (CheckForSpecificCollider(PlayerGatherManager.instance.searchCollider))
        {
            if (other.GetComponent<Item>() || other.GetComponent<Resource>())
                PlayerGatherManager.instance.itemsInRange.Remove(other.gameObject);

        }
    }

    bool CheckForSpecificCollider(GameObject collider)
    {
        if (this.gameObject == collider)
            return true;

        return false;
    }
}
