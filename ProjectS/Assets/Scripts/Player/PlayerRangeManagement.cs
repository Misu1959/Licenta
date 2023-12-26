using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangeManagement : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (CheckForSpecificCollider(PlayerActionManagement.instance.darknessCollider))
        {
            if (other.gameObject.GetComponent<Light2D>())
                PlayerStats.instance.SetInLight(1);
        }
        if (CheckForSpecificCollider(PlayerActionManagement.instance.actionCollider))
        {
            if (other.gameObject.layer == 10)
                PlayerStats.instance.SetResearchLevel(1);
        }
        if (CheckForSpecificCollider(PlayerActionManagement.instance.searchCollider))
        {
            if (other.GetComponent<Item>() || other.GetComponent<Resource>())
                PlayerActionManagement.instance.itemsInRange.Add(other.gameObject);

        }

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (CheckForSpecificCollider(PlayerActionManagement.instance.playerBody))
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

        if (CheckForSpecificCollider(PlayerActionManagement.instance.actionCollider))
            if (other.gameObject == PlayerActionManagement.instance.target)
            {
                if (PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.pick)
                {
                    InventoryManager.instance.AddItemToSlot(other.gameObject);
                    PlayerActionManagement.instance.itemsInRange.Remove(other.gameObject);
                    PlayerActionManagement.instance.CompleteAction();
                }
                else if (PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.drop)
                {
                    PlayerActionManagement.instance.target.GetComponent<Item>().SetTransparent(false);
                    PlayerActionManagement.instance.CompleteAction();
                }
                else if(PlayerActionManagement.instance.currentAction >= PlayerActionManagement.Action.gather && PlayerActionManagement.instance.currentAction <= PlayerActionManagement.Action.mine)
                {
                    if (!other.gameObject.GetComponent<Resource>().isBeingGathered)
                        other.gameObject.GetComponent<Resource>().SetIsBeingGathered(true);
                }
                else if (PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.addFuel)
                {
                    other.gameObject.GetComponent<Fire>().AddFuel(InventoryManager.instance.selectedItem);
                    PlayerActionManagement.instance.CompleteAction();
                }
                else if (PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.cook)
                {
                    if (!InventoryManager.instance.selectedItem.GetComponent<Food>().isCooking)
                        InventoryManager.instance.selectedItem.GetComponent<Food>().SetIsCooking(true);
                }
                else if (PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.eat)
                {
                    other.GetComponent<Food>().Consume();
                    PlayerActionManagement.instance.CompleteAction();
                }
            }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CheckForSpecificCollider(PlayerActionManagement.instance.playerBody))
        {
            Color objColor = other.GetComponent<SpriteRenderer>().color;
            other.GetComponent<SpriteRenderer>().color = new Color(objColor.r, objColor.g, objColor.b, 1);
        }

        if (CheckForSpecificCollider(PlayerActionManagement.instance.darknessCollider))
        {
            if (other.gameObject.GetComponent<Light2D>())
                PlayerStats.instance.SetInLight(-1);
        }

        if (CheckForSpecificCollider(PlayerActionManagement.instance.actionCollider))
        {
            if (other.GetComponent<Construction>())
            {

                //It has bugs
                if (other.gameObject.layer == 10)
                    PlayerStats.instance.SetResearchLevel(0);

            }
        }

        if (CheckForSpecificCollider(PlayerActionManagement.instance.searchCollider))
        {
            if (other.GetComponent<Item>() || other.GetComponent<Resource>())
                PlayerActionManagement.instance.itemsInRange.Remove(other.gameObject);

        }
    }

    bool CheckForSpecificCollider(GameObject collider)
    {
        if (this.gameObject == collider)
            return true;

        return false;
    }
}
