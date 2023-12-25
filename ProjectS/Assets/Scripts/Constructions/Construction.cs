using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Construction : MonoBehaviour
{
    private Color color;

    private bool isPlaced;
    private bool canBePlaced;

    private void Start()
    {
        color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, .5f);
    }

    void Update()
    {
        Place();
    }

    void Place()
    {
        if (isPlaced)
            return;

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!CheckIfCanBePlaced())
        {
            if (canBePlaced)
            {
                GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, .5f);
                canBePlaced = false;
            }
        }
        else
        {
            if (!canBePlaced)
            {
                GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, .5f);
                canBePlaced = true;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (CheckIfItIsOverUI())
                {
                    GetComponent<SpriteRenderer>().color = color;
                    GetComponent<Collider2D>().isTrigger = false;

                    if (GetComponent<Fire>())
                        GetComponent<Fire>().enabled = true;

                    CraftingRecipe currentRecipe = CraftingManager.instance.currentRecipe.GetComponent<CraftingRecipe>();
                    for (int i = 0; i < currentRecipe.requirements.Length; i++)
                        InventoryManager.instance.SpendResources(currentRecipe.requirements[i].type, currentRecipe.requirements[i].quantity);

                    isPlaced = true;
                    return;
                }
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(this.gameObject);
            return;
        }

    }

    bool CheckIfCanBePlaced()
    {
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();

        Physics2D.OverlapCollider(GetComponent<Collider2D>(),filter,results);
        
        for (int i = 0; i < results.Count; i++)
            if (results[i].gameObject.layer == 3)
                results.RemoveAt(i--);
        
        if (results.Count == 0)
            return true;

        return false;
    }

    bool CheckIfItIsOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        for(int i=0;i<raycastResultsList.Count;i++)
            if (raycastResultsList[i].gameObject==this.gameObject)
            {
                raycastResultsList.RemoveAt(i);
                i--;
            }

        return !(raycastResultsList.Count > 0);
    }
}
