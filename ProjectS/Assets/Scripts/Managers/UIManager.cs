using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Tooltip("not to be placed on the clock object")]
    [SerializeField] private Transform clock;

    [Tooltip("To be placed on the hp background")]
    [SerializeField] private Transform hpUI;
    [SerializeField] private Sprite[] hpStages;

    [Tooltip("To be placed on the hunger background")]
    [SerializeField] private Transform hungerUI;
    [SerializeField] private Sprite[] hungerStages;

    [SerializeField] private Transform escapeScreen;
    [SerializeField] private Transform deathScreen;

    [SerializeField] private Transform escapeButton;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        InputManager.instance.GetInputActions().Player.Pause.performed += ShowEscapeScreen;
        SetEscapeMenu();
    }

    private void OnDestroy() => InputManager.instance.GetInputActions().Player.Pause.performed -= ShowEscapeScreen;

    private void SetEscapeMenu()
    {
        escapeButton.GetComponent<Button>().onClick.AddListener(() => ShowEscapeScreen());

        escapeScreen.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ShowEscapeScreen());
        escapeScreen.GetChild(2).GetComponent<Button>().onClick.AddListener(() => WorldGenerator.instance.SaveWorld());
        escapeScreen.GetChild(3).GetComponent<Button>().onClick.AddListener(() => GoBackToMainMenu());

    }

    public void SetClock(float dayLength,float dawnLength,float currentTime)
    {
        clock.parent.GetChild(0).GetComponent<Image>().fillAmount = dawnLength + dayLength;
        clock.parent.GetChild(1).GetComponent<Image>().fillAmount = dayLength;

        clock.GetChild(0).eulerAngles = new Vector3(0, 0, -currentTime);
    }
    public void ShowTime(float timeSpeed) => clock.GetChild(0).eulerAngles -= new Vector3(0, 0, timeSpeed);
    public void ShowDayCount(int currentDay) => clock.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Day\n" + currentDay.ToString();

    public void ShowHp(float maxHp,float hp)
    {
        GameObject hpFiller = hpUI.GetChild(0).gameObject; 
        GameObject hpImage  = hpUI.GetChild(1).gameObject;
        GameObject hpText   = hpUI.GetChild(3).gameObject;

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
        GameObject hungerFiller = hungerUI.GetChild(0).gameObject;
        GameObject hungerImage = hungerUI.GetChild(1).gameObject;
        GameObject hungerText = hungerUI.GetChild(3).gameObject;

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

    private void ShowEscapeScreen(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ShowEscapeScreen();
    }

    private void ShowEscapeScreen()
    {
        string selectedWorld = "World " + SaveLoadManager.Get_Selected_world();
        string currentDay = " day " + TimeManager.instance.currentDay;
        escapeScreen.GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedWorld + currentDay;



        escapeScreen.gameObject.SetActive(!escapeScreen.gameObject.activeInHierarchy);
        Time.timeScale = escapeScreen.gameObject.activeInHierarchy ? 0.0f : 1.0f;

        InventoryManager.instance.SetBackToSlot();
        InteractionManager.instance.SetInteractionStatus(!escapeScreen.gameObject.activeInHierarchy);
    }

    public void ShowDeathScreen(string causeOfDeath)
    {
        ShowDeathText(causeOfDeath);
        deathScreen.gameObject.SetActive(true);


        PlayerPrefs.SetInt("prevWorld", 0);
        Time.timeScale = 0;
        //CraftingManager.instance.toolTip.gameObject.SetActive(false);
        
        Button deathScreenButton = deathScreen.GetChild(2).GetComponent<Button>();
        deathScreenButton.onClick.AddListener(() => SceneManager.LoadScene(0));

        InteractionManager.instance.SetInteractionStatus(false);
    }


    private void ShowDeathText(string causeOfDeath)
    {
        TextMeshProUGUI deathText = deathScreen.GetChild(1).GetComponent<TextMeshProUGUI>();

        deathText.text = "You survived " + (PlayerPrefs.GetInt("currentDay") - 1) + " full days\n and died " + causeOfDeath;
    }

    private void GoBackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
