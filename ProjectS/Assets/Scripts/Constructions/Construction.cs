using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Construction : MonoBehaviour
{
    private Color color;

    [SerializeField] private float maxTimeToBuild;
    private float timeToBuild;

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
        if (!PlayerActionManagement.instance.isBuilding) // If player is not building return
            return;

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(CheckIfCanBePlaced() & MyMethods.CheckIfMouseIsOverUI())
        {
            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, .5f);
            
            if (Input.GetMouseButtonDown(0))
                PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.build);
        }
        else 
            GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0, .5f);


        if (Input.GetMouseButtonDown(1))
        {
            CraftingManager.instance.ActivateCraftingButtons(true);
            Destroy(this.gameObject);
            return;
        }

    }

    public void Build()
    {
        if (!IsBuilt())
        {
            timeToBuild = maxTimeToBuild;
            return;
        }

        timeToBuild -= Time.deltaTime;
        if(timeToBuild<=0)
        {
            CraftingManager.instance.ActivateCraftingButtons(true);

            GetComponent<SpriteRenderer>().color = color;
            GetComponent<Collider2D>().isTrigger = false;

            if (GetComponent<Fire>())
                GetComponent<Fire>().enabled = true;

            CraftingRecipe currentRecipe = CraftingManager.instance.currentRecipe.GetComponent<CraftingRecipe>();
            for (int i = 0; i < currentRecipe.requirements.Length; i++)
                InventoryManager.instance.SpendResources(currentRecipe.requirements[i].type, currentRecipe.requirements[i].quantity);

        }
    }

    bool CheckIfCanBePlaced()
    {
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();

        Physics2D.OverlapCollider(GetComponent<Collider2D>(),filter,results);

        for (int i = 0; i < results.Count; i++)
            if (results[i].gameObject.layer == 2)
                results.RemoveAt(i--);

        return results.Count > 0 ? false : true;
    }

    protected bool IsBuilt()
    {
        if (PlayerActionManagement.instance.currentTarget == this.gameObject &&
            PlayerActionManagement.instance.currentAction == PlayerActionManagement.Action.build &&
            PlayerActionManagement.instance.isPerformingAction)
            return true;
        else
            return false;
    }

}
