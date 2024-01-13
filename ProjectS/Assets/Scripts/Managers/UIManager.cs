using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject clock;

    [SerializeField] private GameObject hpUI;
    [SerializeField] private Sprite[] hpStages;

    [SerializeField] private GameObject hungerUI;
    [SerializeField] private Sprite[] hungerStages;

    [SerializeField] private GameObject escapeScreen;
    [SerializeField] private GameObject deathScreen;

    [SerializeField] private GameObject escapeButton;

    private void Start()
    {
        instance = this;
        escapeButton.GetComponent<Button>().onClick.AddListener(() => ShowEscapeScreen());

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowEscapeScreen();
    }

    public void SetClock(float time)
    {
        clock.transform.parent.GetComponent<Image>().fillAmount = time;
    }
    public void ShowTime(float timeSpeed)
    {
        clock.transform.GetChild(0).eulerAngles -= new Vector3(0, 0, timeSpeed);
    }
    public void ShowDayCount(int currentDay)
    {
        clock.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Day\n" + currentDay.ToString();

    }

    public void ShowHp(float maxHp,float hp)
    {
        GameObject hpFiller = hpUI.transform.GetChild(0).gameObject; 
        GameObject hpImage  = hpUI.transform.GetChild(1).gameObject;
        GameObject hpText   = hpUI.transform.GetChild(3).gameObject;

        hpFiller.GetComponent<Image>().fillAmount = hp / maxHp;

        int img;
        if (hp <= .25f * maxHp)
            img = 0;
        else if (hp <= .5f * maxHp)
            img = 1;
        else if (hp <= .5f * maxHp)
            img = 2;
        else
            img = 3;

        if (hpImage.GetComponent<Image>().sprite != hpStages[img])
            hpImage.GetComponent<Image>().sprite = hpStages[img];
            
        hpText.GetComponent<TextMeshProUGUI>().text = hp.ToString();
    }

    public void ShowHunger(float maxHunger,float hunger)
    {
        GameObject hungerFiller = hungerUI.transform.GetChild(0).gameObject;
        GameObject hungerImage = hungerUI.transform.GetChild(1).gameObject;
        GameObject hungerText = hungerUI.transform.GetChild(3).gameObject;

        hungerFiller.GetComponent<Image>().fillAmount = hunger / maxHunger;

        int img;
        if (hunger <= .25f * maxHunger)
            img = 0;
        else if (hunger <= .5f * maxHunger)
            img = 1;
        else if (hunger <= .5f * maxHunger)
            img = 2;
        else
            img = 3;

        if (hungerImage.GetComponent<Image>().sprite != hungerStages[img])
            hungerImage.GetComponent<Image>().sprite = hungerStages[img];

        hungerText.GetComponent<TextMeshProUGUI>().text = hunger.ToString();
    }

    private void ShowEscapeScreen()
    {
        escapeScreen.SetActive(!escapeScreen.activeInHierarchy);

        Time.timeScale = escapeScreen.activeInHierarchy ? 0.0f : 1.0f;
        escapeScreen.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        escapeScreen.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(0));
        
        //CraftingManager.instance.toolTip.gameObject.SetActive(false);
        InventoryManager.instance.SetBackToSlot();

        InteractionManager.SetInteractionStatus(!escapeScreen.activeInHierarchy);
    }

    public void ShowDeathScreen(string causeOfDeath)
    {
        ShowDeathText(causeOfDeath);
        deathScreen.SetActive(true);

        PlayerPrefs.SetInt("prevWorld", 0);
        Time.timeScale = 0;
        //CraftingManager.instance.toolTip.gameObject.SetActive(false);
        
        Button deathScreenButton = deathScreen.transform.GetChild(1).GetComponent<Button>();
        deathScreenButton.onClick.AddListener(() => SceneManager.LoadScene(0));

        InteractionManager.SetInteractionStatus(false);
    }


    private void ShowDeathText(string causeOfDeath)
    {
        TextMeshProUGUI deathText = deathScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        deathText.text = "You survived " + (PlayerPrefs.GetInt("currentDay") - 1) + " full days\n and died " + causeOfDeath;
    }

}
