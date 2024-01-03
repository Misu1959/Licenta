using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static UnityEditor.Progress;
using Unity.VisualScripting;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    public GameObject items;
    public GameObject resources;
    public GameObject constructions;
    public GameObject mobs;
    void Start()
    {
        instance = this;

        if (PlayerPrefs.GetInt("prevWorld") == 0)
        {
            PlayerPrefs.SetInt("prevWorld", 1);
            Invoke("SaveWorld", 1); // Should save when everything in the world is initialized
        }
        else
            StartCoroutine(LoadWorld());
    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Stats saved");
            SaveWorld();
        }
    }

    #region Save
    public void SaveWorld()
    {
        PlayerPrefs.SetInt("prevWorld", PlayerPrefs.GetInt("prevWorld") + 1);

        SavePlayer();
        SaveItems();
        SaveResources();
        SaveConstructions();
        SaveMobs();
    }

    void SavePlayer()
    {
        PlayerPrefs.SetFloat("playerHp", PlayerStats.instance.hp);
        PlayerPrefs.SetFloat("playerDmg", PlayerStats.instance.dmg);
        PlayerPrefs.SetFloat("playerHunger", PlayerStats.instance.hunger);
        PlayerPrefs.SetFloat("playerSpeed", PlayerStats.instance.speed);

        PlayerPrefs.SetFloat("playerPosX", PlayerStats.instance.gameObject.transform.position.x);
        PlayerPrefs.SetFloat("playerPosY", PlayerStats.instance.gameObject.transform.position.y);

        SaveInventory();
        // Save equipment
    }

    void SaveInventory()
    {
        int nrOfItemsInInventory = 0;
        for (int i = 0; i < 15; i++)
            if (InventoryManager.instance.inventory.GetChild(i).childCount > 0)
            {
                Item item = InventoryManager.instance.inventory.GetChild(i).GetChild(0).GetComponent<Item>();
                if (item.GetComponent<ItemUI>())
                    PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " typeOfItem", 1);
                else if (item.GetComponent<FoodUI>())
                {
                    PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " typeOfItem", 2);
                    PlayerPrefs.SetFloat("itemUI " + nrOfItemsInInventory + " foodHpAmount",item.GetComponent<Food>().hpAmount);
                    PlayerPrefs.SetFloat("itemUI " + nrOfItemsInInventory + " foodHungerAmount",item.GetComponent<Food>().hungerAmount);
                }
                if (item.GetComponent<EquipmentUI>())
                {
                    PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " typeOfItem", 3);
                    PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " equipmentNumber", item.GetComponent<Equipment>().equipmentNumber);
                    PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " equipmentActionNumber", item.GetComponent<Equipment>().actionNumber);
                    PlayerPrefs.SetFloat("itemUI " + nrOfItemsInInventory + " equipmentDurability", item.GetComponent<Equipment>().durability);
                }



                PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " parentSlot", i);
                PlayerPrefs.SetString("itemUI " + nrOfItemsInInventory + " type", item.type);

                PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " currentStack", item.currentStack);
                PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " maxStack", item.maxStack);
                
                PlayerPrefs.SetInt("itemUI " + nrOfItemsInInventory + " fuelValue", item.fuelValue);

                nrOfItemsInInventory++;
            }

        if(InventoryManager.instance.selectedItem)
        {
            PlayerPrefs.SetInt("selectedItem", 1);

            Item selectedItem = InventoryManager.instance.selectedItem;

            if (selectedItem.GetComponent<ItemUI>())
                PlayerPrefs.SetInt("selectedItem typeOfItem", 1);
            else if (selectedItem.GetComponent<FoodUI>())
            {
                PlayerPrefs.SetInt("selectedItem typeOfItem", 2);
                PlayerPrefs.SetFloat("selectedItem foodHpAmount", selectedItem.GetComponent<Food>().hpAmount);
                PlayerPrefs.SetFloat("selectedItem hungerHpAmount", selectedItem.GetComponent<Food>().hungerAmount);
            }
            if (selectedItem.GetComponent<EquipmentUI>())
            {
                PlayerPrefs.SetInt("selectedItem typeOfItem", 3);
                PlayerPrefs.SetInt("selectedItem equipmentNumber", selectedItem.GetComponent<Equipment>().equipmentNumber);
                PlayerPrefs.SetInt("selectedItem equipmentActionNumber", selectedItem.GetComponent<Equipment>().actionNumber);
                PlayerPrefs.SetFloat("selectedItem equipmentDurability", selectedItem.GetComponent<Equipment>().durability);
            }


            PlayerPrefs.SetString("selectedItem type", InventoryManager.instance.selectedItem.type);

            PlayerPrefs.SetInt("selectedItem maxStack", InventoryManager.instance.selectedItem.maxStack);
            PlayerPrefs.SetInt("selectedItem currentStack", InventoryManager.instance.selectedItem.currentStack);

            PlayerPrefs.SetInt("selectedItem fuelValue", InventoryManager.instance.selectedItem.fuelValue);
        }
        else
            PlayerPrefs.SetInt("selectedItem", 0);

        PlayerPrefs.SetInt("nrOfItemsInInventory", nrOfItemsInInventory);
    }

    void SaveItems()
    {
        PlayerPrefs.SetInt("nrOfItems", items.transform.childCount);
        for(int i=0;i<items.transform.childCount;i++) 
        {
            Item item= items.transform.GetChild(i).GetComponent<Item>();

            PlayerPrefs.SetString("item " + i + " type", item.type);
            PlayerPrefs.SetInt("item " + i + " currentStack", item.currentStack);

            PlayerPrefs.SetFloat("item " + i + " posX", item.transform.position.x);
            PlayerPrefs.SetFloat("item " + i + " posY", item.transform.position.y);
        }
    }

    void SaveResources()
    {
        PlayerPrefs.SetInt("nrOfResources", resources.transform.childCount);
        for (int i = 0; i < resources.transform.childCount; i++)
        {
            Resource resource = resources.transform.GetChild(i).GetComponent<Resource>();

            PlayerPrefs.SetString("resource " + i + " type", resource.type);

            //PlayerPrefs.SetFloat("resource " + i + " timeToGrow", resource.timeToGrow);

            PlayerPrefs.SetFloat("resource " + i + " posX", resource.transform.position.x);
            PlayerPrefs.SetFloat("resource " + i + " posY", resource.transform.position.y);

        }
    }
    void SaveConstructions()
    {

    }

    void SaveMobs()
    {

    }

    #endregion

    #region Load

    IEnumerator LoadWorld()
    {
        yield return null;

        LoadPlayer();
        LoadItems();
        LoadResources();
        LoadConstructions();
        LoadMobs();
    }

    void LoadPlayer()
    {
        float _hp       = PlayerPrefs.GetFloat("playerHp");
        float _dmg      = PlayerPrefs.GetFloat("playerDmg");
        float _speed    = PlayerPrefs.GetFloat("playerSpeed");
        float _hunger   = PlayerPrefs.GetFloat("playerHunger");
        Vector2 _pos = new Vector2(PlayerPrefs.GetFloat("playerPosX"), PlayerPrefs.GetFloat("playerPosY"));

        StartCoroutine(PlayerStats.instance.SetStats(_hp,_dmg,_speed,_hunger,_pos));
        LoadInventory();
    }

    void LoadInventory()
    {

        for (int i = 0; i < PlayerPrefs.GetInt("nrOfItemsInInventory"); i++)
        {
            GameObject itemUI = Instantiate(ItemsManager.instance.itemUI);
            if (PlayerPrefs.GetInt("itemUI " + i + " typeOfItem") == 1)
                itemUI.AddComponent<ItemUI>();
            else if (PlayerPrefs.GetInt("itemUI " + i + " typeOfItem") == 2)
            {
                itemUI.AddComponent<FoodUI>();

                itemUI.GetComponent<Food>().hpAmount     = PlayerPrefs.GetFloat("itemUI " + i + " foodHpAmount");
                itemUI.GetComponent<Food>().hungerAmount = PlayerPrefs.GetFloat("itemUI " + i + " foodHungerAmount");

            }
            else if (PlayerPrefs.GetInt("itemUI " + i + " typeOfItem") == 3)
            {
                itemUI.AddComponent<EquipmentUI>();

                itemUI.GetComponent<Equipment>().equipmentNumber = PlayerPrefs.GetInt("itemUI " + i + " equipmentNumber");
                itemUI.GetComponent<Equipment>().actionNumber = PlayerPrefs.GetInt("itemUI " + i + " equipmentActionNumber");
                itemUI.GetComponent<Equipment>().SetDurability(PlayerPrefs.GetFloat("itemUI " + i + " equipmentDurability"));
            }


            itemUI.GetComponent<Item>().SetType(PlayerPrefs.GetString("itemUI " + i + " type"));
            itemUI.GetComponent<Item>().AddToStack(PlayerPrefs.GetInt("itemUI " + i + " currentStack"));

            itemUI.GetComponent<Item>().maxStack = PlayerPrefs.GetInt("itemUI " + i + " maxStack");
            itemUI.GetComponent<Item>().fuelValue = PlayerPrefs.GetInt("itemUI " + i + " fuelValue");

            itemUI.GetComponent<Item>().transform.SetParent(InventoryManager.instance.inventory.GetChild(PlayerPrefs.GetInt("itemUI " + i + " parentSlot")));
            itemUI.GetComponent<Item>().transform.localPosition = Vector2.zero;

            itemUI.GetComponent<Item>().uiImg = ItemsManager.instance.SearchItemsList(itemUI.GetComponent<Item>().type).GetComponent<Item>().uiImg;
            itemUI.GetComponent<Item>().GetComponent<Image>().sprite = itemUI.GetComponent<Item>().uiImg;
        }

        if (PlayerPrefs.GetInt("selectedItem") == 1)
        {
            GameObject selectedItem = Instantiate(ItemsManager.instance.itemUI);

            if (PlayerPrefs.GetInt("selectedItem typeOfItem") == 1)
                selectedItem.AddComponent<ItemUI>();
            else if (PlayerPrefs.GetInt("selectedItem typeOfItem") == 2)
            {
                selectedItem.AddComponent<FoodUI>();

                selectedItem.GetComponent<Food>().hpAmount = PlayerPrefs.GetFloat("selectedItem foodHpAmount");
                selectedItem.GetComponent<Food>().hungerAmount = PlayerPrefs.GetFloat("selectedItem foodHungerAmount");

            }
            else if (PlayerPrefs.GetInt("selectedItem typeOfItem") == 3)
            {
                selectedItem.AddComponent<EquipmentUI>();

                selectedItem.GetComponent<Equipment>().equipmentNumber = PlayerPrefs.GetInt("selectedItem equipmentNumber");
                selectedItem.GetComponent<Equipment>().actionNumber = PlayerPrefs.GetInt("selectedItem equipmentActionNumber");
                selectedItem.GetComponent<Equipment>().SetDurability(PlayerPrefs.GetFloat("selectedItem equipmentDurability"));
            }

            selectedItem.GetComponent<Item>().SetType(PlayerPrefs.GetString("selectedItem type"));
            selectedItem.GetComponent<Item>().AddToStack(PlayerPrefs.GetInt("selectedItem currentStack"));

            selectedItem.GetComponent<Item>().maxStack = PlayerPrefs.GetInt("selectedItem maxStack");
            selectedItem.GetComponent<Item>().fuelValue = PlayerPrefs.GetInt("selectedItem fuelValue");

            selectedItem.GetComponent<Item>().transform.SetParent(InventoryManager.instance.inventory);
            InventoryManager.instance.selectedItem = selectedItem.GetComponent<Item>();

            selectedItem.GetComponent<Image>().color = ItemsManager.instance.SearchItemsList(selectedItem.GetComponent<Item>().type).GetComponent<SpriteRenderer>().color;
        }
    }


    void LoadItems()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("nrOfItems"); i++)
        {
            Item item = Instantiate(ItemsManager.instance.SearchItemsList(PlayerPrefs.GetString("item " + i + " type"))).GetComponent<Item>();

            item.SetType(PlayerPrefs.GetString("item " + i + " type"));
            item.AddToStack(PlayerPrefs.GetInt("item " + i + " currentStack"));

            item.transform.SetParent(items.transform);
            item.transform.position = new Vector2(PlayerPrefs.GetFloat("item " + i + " posX"), PlayerPrefs.GetFloat("item " + i + " posY"));
        }
    }

    void LoadResources()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("nrOfResources"); i++)
        {
            GameObject resource = Instantiate(ItemsManager.instance.SearchResourcesList(PlayerPrefs.GetString("resource " + i + " type")));
            resource.GetComponent<Resource>().SetType(PlayerPrefs.GetString("resource " + i + " type"));

            //resource.GetComponent<Resource>().timeToGrow = PlayerPrefs.GetFloat("resource " + i + " timeToGrow");

            resource.transform.SetParent(resources.transform);
            resource.transform.position = new Vector2(PlayerPrefs.GetFloat("resource " + i + " posX"), PlayerPrefs.GetFloat("resource " + i + " posY"));
        }

    }

    void LoadConstructions()
    {

    }

    void LoadMobs()
    {
        
    }

    #endregion
}
