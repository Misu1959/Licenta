using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Construction : MonoBehaviour
{
    private Color color;

    [SerializeField] private float timeToBuild;
    private Timer timer;


    private void Start()
    {
        timer = new Timer(timeToBuild);

        color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, .5f);
    }

    void Update()
    {
        Place();
        Build();
    }

    void Place()
    {
        if (!PlayerActionManagement.instance.IsPlacing(this.gameObject)) return; // If player is not placing return

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        PopUpManager.instance.ShowMousePopUp("RMB - cancel");


        if (CheckIfCanBePlaced() & !MyMethods.CheckIfMouseIsOverUI())
        {
            PopUpManager.instance.ShowMousePopUp("LMB - place\nRMB - cancel");

            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, .5f);

            if (Input.GetMouseButtonDown(0))
            {
                PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.build);
                PopUpManager.instance.ShowMousePopUp();
            }
        }
        else
            GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 128);


        if (Input.GetMouseButtonDown(1))
        {
            PlayerActionManagement.instance.CancelAction();
            PopUpManager.instance.ShowMousePopUp();
            Destroy(this.gameObject);
            return;
        }

    }

    public void Build()
    {
        if (!PlayerActionManagement.instance.IsBuilding(this.gameObject))
        {
            timer.RestartTimer();
            return;
        }

        timer.StartTimer();
        timer.Tick();
        if (!timer.IsElapsed()) return;


        GetComponent<SpriteRenderer>().color = color;
        GetComponent<Collider2D>().isTrigger = false;
        transform.SetParent(SaveLoadManager.instance.constructions.transform);


        if (GetComponent<Fireplace>())
            GetComponent<Fireplace>().enabled = true;

        foreach(Recipe.Requiremets req in CraftingManager.instance.GetCurrentCraftingRecipe().GetRecipe().requirements)
            InventoryManager.instance.SpendResources(req.name, req.quantity);

        PlayerActionManagement.instance.CompleteAction();
    }

    bool CheckIfCanBePlaced()
    {
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();

        Physics2D.OverlapCollider(GetComponent<Collider2D>(),filter,results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 2 || results[i].gameObject == this.gameObject)
                continue;
            else
                return false;

        }
        return true;
    }

}
