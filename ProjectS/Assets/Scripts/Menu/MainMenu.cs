using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance { get; private set; }

    [Header("* Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject worldSettingsMenu;
    [SerializeField] private CheckActionPanel checkActionPanel;

    [Header("* Buttons")]
    [SerializeField] private Button buttonContinue;

    [Header("- World Selection panel buttons")]
    [SerializeField] private Button buttonOpenWorldSelectionPanel;
    [SerializeField] private Button buttonCloseWorldSelectionPanel;
    
    [SerializeField] private Button[] buttonSelectWorld;
    [SerializeField] private Button[] buttonDeleteWorld;

    [Header("- World settings menu buttons")]
    [SerializeField] private Button buttonBackFromWorldSettingsMenuCheck;
    [SerializeField] private Button buttonResetWorldSettingsCheck;
    [SerializeField] private Button buttonStartGame;


    [Header("- Controls menu buttons")]
    [SerializeField] private Button buttonOpenControlsMenu;
    [SerializeField] private Button buttonBackFromControlsMenu;
    
    [SerializeField] private Button buttonResetControlsCheck;

    [Header("- Quit game panel buttons")]

    [SerializeField] private Button buttonQuitGameCheck;



    private void Awake() => instance = this;
    private void Start()
    {

        InputManager.instance.GetInputActions().Menu.BackToMenu.performed += BackToMainMenu;

        InputManager.instance.SetActionMap(InputManager.instance.GetInputActions().Menu);

        SetButtonsFunctionality();
    }

    private void OnDestroy() => InputManager.instance.GetInputActions().Menu.BackToMenu.performed -= BackToMainMenu;


    #region Buttons setup
    void SetButtonsFunctionality()
    {

        buttonContinue.interactable = (SaveLoadManager.Get_Last_world() == 0) ? false : true;
        buttonContinue.onClick.AddListener(() => ContinueGame());

        SetWorldSelectionButtons();
        SetQuitGameButtons();

        SetControlsMenuButtons();
        SetWorldSettingsMenuButtons();
    }

    void SetWorldSelectionButtons()
    {
        buttonOpenWorldSelectionPanel.onClick.AddListener(() => OpenWorldSelectionPanel());
        buttonCloseWorldSelectionPanel.onClick.AddListener(() => BackToMainMenu());


        for (int i = 0; i < 3; i++)
        {
            int x = i + 1;
            int j = i;
            
            buttonSelectWorld[j].onClick.AddListener(() => SelectWorld(x));
            buttonDeleteWorld[j].onClick.AddListener(() => checkActionPanel.OpenPanel(buttonSelectWorld[j].transform, "Are you sure you want to\ndelete this world ?", DeleteWorld, x));

        }

    }

    void SetQuitGameButtons() => buttonQuitGameCheck.onClick.AddListener(() => checkActionPanel.OpenPanel(buttonQuitGameCheck.transform, "Are you sure you want to quit ?", QuitGame));

    void SetWorldSettingsMenuButtons()
    {
        buttonStartGame.onClick.AddListener(() => StartGame());
        buttonBackFromWorldSettingsMenuCheck.onClick.AddListener(() => BackFromWorldSettingsMenu());
        buttonResetWorldSettingsCheck.onClick.AddListener(() => checkActionPanel.OpenPanel(buttonResetWorldSettingsCheck.transform, "Are you sure you want to reset the settings ?", WorldSettingsManager.ResetAllSettings));

        SetInteractable_ResetSettingsButton(0);
    }
    void SetControlsMenuButtons()
    {
        buttonOpenControlsMenu.onClick.AddListener(() => OpenControlsMenu());
        buttonBackFromControlsMenu.onClick.AddListener(() => BackToMainMenu());

        buttonResetControlsCheck.onClick.AddListener(() => checkActionPanel.OpenPanel(buttonResetControlsCheck.transform, "Are you sure you want to reset the controls?", ResetControlsSettings));

        SetInteractable_ResetControlsButton(0);
    }

    #endregion

    #region Main menu

    void ContinueGame() => SceneManager.LoadScene(SaveLoadManager.Get_Last_world());

    void OpenWorldSelectionPanel()
    {
        buttonOpenWorldSelectionPanel.GetComponent<Animator>().SetBool("ShowPanel", true);
        SetMainButtonsInteractable(false);

        for (int i = 0; i < 3; i++)
        {
            int x = i + 1;
            bool oldWorld = (SaveLoadManager.Get_Old_World(x) == 0) ? false : true;

            if (oldWorld)
            {
                string selectedWorld = "World " + SaveLoadManager.Get_Selected_world();
                string currentDay = " Day " + SaveLoadManager.Get_World_Day(x);
                //buttonSelectWorld[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedWorld +" - "+ currentDay;
            }
            buttonDeleteWorld[i].gameObject.SetActive(oldWorld);
        
        }
    }

    void SelectWorld(int worldToSelect) 
    {
        SaveLoadManager.Set_Selected_world(worldToSelect);

        if (SaveLoadManager.Get_Old_World_Current() == 0)
            OpenWorldSettingsMenu();
        else
            SceneManager.LoadScene(worldToSelect);
    }
    void DeleteWorld(int worldToDelete)
    {
       /// buttonSelectWorld[worldToDelete - 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "World " + worldToDelete + " - New game";
        buttonDeleteWorld[worldToDelete - 1].gameObject.SetActive(false);
        SaveLoadManager.Set_Old_World(worldToDelete, 0);

        if (SaveLoadManager.Get_Last_world() == worldToDelete)
            SaveLoadManager.Set_Last_world(0);

    }

    void QuitGame() => Application.Quit();

    void BackToMainMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        BackToMainMenu();
    }

    void BackToMainMenu()
    {
        if (checkActionPanel.IsOn())
            checkActionPanel.ClosePanel();
        else
        {
            SetMainButtonsInteractable(true);

            worldSettingsMenu.SetActive(false);
            controlsMenu.SetActive(false);

            if (WorldSettingsManager.GetNrOfChanges() != 0)
                WorldSettingsManager.ResetAllSettings();

            buttonOpenWorldSelectionPanel.GetComponent<Animator>().SetBool("ShowPanel", false);
        }
    }


    void SetMainButtonsInteractable(bool activeOrNot)
    {
        buttonContinue.interactable = ((SaveLoadManager.Get_Last_world() == 0) || activeOrNot == false) ? false : true;

        buttonOpenWorldSelectionPanel.interactable  = activeOrNot;
        buttonOpenControlsMenu.interactable         = activeOrNot;
        buttonQuitGameCheck.interactable            = activeOrNot;
    }

    #endregion

    #region World settings

    void OpenWorldSettingsMenu() => worldSettingsMenu.SetActive(true);


    void StartGame()
    {
        WorldSettingsManager.SaveAllSetings();

        SaveLoadManager.Set_Last_world(SaveLoadManager.Get_Selected_world());
        SceneManager.LoadScene(SaveLoadManager.Get_Selected_world());
    }

    private void BackFromWorldSettingsMenu()
    {
        if(WorldSettingsManager.GetNrOfChanges() == 0)
            BackToMainMenu();
        else
            checkActionPanel.OpenPanel(buttonBackFromWorldSettingsMenuCheck.transform, "Are you sure you want to go back ?", ConfirmClose);
    }

    private void ConfirmClose()
    {
        checkActionPanel.gameObject.SetActive(false);
        BackToMainMenu();
    }

    public void SetInteractable_ResetSettingsButton(int nr) => buttonResetWorldSettingsCheck.interactable = (nr == 0) ? false : true;

    #endregion

    #region Controls menu

    void OpenControlsMenu() => controlsMenu.SetActive(true);


    private void ResetControlsSettings()
    {
        foreach(InputActionMap map in InputManager.instance.GetInputAsset().actionMaps)
            map.RemoveAllBindingOverrides();

        SaveLoadManager.Delete_Rebinds();
        SetInteractable_ResetControlsButton(0);
        SaveLoadManager.Set_Nr_Rebinds_Changes(0);

    }

    public void SetInteractable_ResetControlsButton(int nr) => buttonResetControlsCheck.interactable = (nr == 0) ? false : true;

    #endregion



}
