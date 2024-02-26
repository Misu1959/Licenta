using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Construction : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color color;

    private int canBePlaced;

    [SerializeField] private Timer buildTimer;


    private void Start()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 2)
            canBePlaced++;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 2)
            canBePlaced--;
    }

    void Update()
    {
        Place();
        Build();
    }

    void Place()
    {
        if (!PlayerActionManagement.instance.IsPlacing(this.gameObject)) return; // If player is not placing return

        PopUpManager.instance.ShowMousePopUp("RMB - cancel", PopUpManager.PopUpPriorityLevel.low);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1000))
            transform.position = new Vector3(hitData.point.x, 0, hitData.point.z);

        if (canBePlaced ==  0 && !MyMethods.CheckIfMouseIsOverUI())
        {
            PopUpManager.instance.ShowMousePopUp("LMB - place\nRMB - cancel", PopUpManager.PopUpPriorityLevel.low);

            spriteRenderer.color = new Color(color.r, color.g, color.b, .5f);

            if (Input.GetMouseButtonDown(0))
                PlayerActionManagement.instance.SetTargetAndAction(this.gameObject, PlayerActionManagement.Action.build);
        }
        else
            spriteRenderer.color = new Color32(255, 0, 0, 128);


        if (Input.GetMouseButtonDown(1))
        {
            PlayerActionManagement.instance.CancelAction();
            Destroy(this.gameObject);
            return;
        }

    }

    public void Build()
    {
        if (!PlayerActionManagement.instance.IsBuilding(this.gameObject))
        {
            buildTimer.RestartTimer();
            return;
        }

        buildTimer.StartTimer();
        buildTimer.Tick();
        if (!buildTimer.IsElapsed()) return;


        spriteRenderer.color = color;
        GetComponent<Collider>().isTrigger = false;
        transform.SetParent(WorldManager.instance.constructions.transform);


        if (GetComponent<Fireplace>())
            GetComponent<Fireplace>().enabled = true;
        
        if (GetComponent<ComplexMobSpawner>())
            GetComponent<ComplexMobSpawner>().enabled = true;
        
        foreach (Recipe.Requiremets req in CraftingManager.instance.GetCurrentCraftingRecipe().GetRecipe().requirements)
            InventoryManager.instance.SpendResources(req.name, req.quantity);

        PlayerActionManagement.instance.CompleteAction();
    }

}
