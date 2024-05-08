using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [Header("* Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject worldSettingsMenu;

    [Header("* Buttons")]
    [SerializeField] private Button buttonContinue;

    [Header("- World Selection panel buttons")]
    [SerializeField] private Button buttonOpenWorldSelectionPanel;
    [SerializeField] private Button buttonCloseWorldSelectionPanel;
    
    [SerializeField] private Button[] buttonSelectWorld;
    [SerializeField] private Button[] buttonDeleteWorld;

    [Header("- World settings menu buttons")]
    [SerializeField] private Button buttonBackFromWorldSettingsMenu;
    [SerializeField] private Button buttonResetWorldSettings;
    [SerializeField] private Button buttonStartGame;


    [Header("- Controls menu buttons")]
    [SerializeField] private Button buttonOpenControlsMenu;
    [SerializeField] private Button buttonBackFromControlsMenu;
    
    [SerializeField] private Button buttonOpenResetControlsCheck;
    [SerializeField] private Button buttonCloseResetControlsCheck;
    [SerializeField] private Button buttonResetControls;
    [SerializeField] private InputActionAsset input;

    [Header("- Quit game panel buttons")]

    [SerializeField] private Button buttonOpenQuitGamePanel;
    [SerializeField] private Button buttonCloseQuitGamePanel;
    [SerializeField] private Button buttonQuitGame;


    PlayerInputActions inputActions;
    private void Start() => SetButtonsFunctionality();

    private void OnEnable()
    {
        inputActions = new PlayerInputActions();
        inputActions.Menu.BackToMenu.performed += BackToMainMenu;
        inputActions.Menu.Enable();

    }

    private void OnDisable()
    {
        inputActions.Menu.Disable();
    }



    #region Buttons setup
    void SetButtonsFunctionality()
    {

        buttonContinue.interactable = (PlayerPrefs.GetInt(SaveData.LAST_WORLD) == 0) ? false : true;
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
            buttonSelectWorld[i].onClick.AddListener(() => SelectWorld(x));
            buttonDeleteWorld[i].onClick.AddListener(() => DeleteWorld(x));
        }

    }

    void SetQuitGameButtons()
    {
        buttonOpenQuitGamePanel.onClick.AddListener(() => OpenQuitPanel());
        buttonCloseQuitGamePanel.onClick.AddListener(() => BackToMainMenu());
        buttonQuitGame.onClick.AddListener(() => QuitGame());
    }

    void SetWorldSettingsMenuButtons()
    {
        buttonBackFromWorldSettingsMenu.onClick.AddListener(() => BackToMainMenu());
        buttonResetWorldSettings.onClick.AddListener(() => WorldSettingsManager.ResetAllSettings());
        buttonStartGame.onClick.AddListener(() => StartGame());
    }
    void SetControlsMenuButtons()
    {
        buttonOpenControlsMenu.onClick.AddListener(() => OpenControlsMenu());
        buttonBackFromControlsMenu.onClick.AddListener(() => BackToMainMenu());
        buttonResetControls.onClick.AddListener(() => ResetControlsSettings());
    }






    #endregion

    #region Main menu

    void ContinueGame() => SceneManager.LoadScene(PlayerPrefs.GetInt(SaveData.LAST_WORLD));

    void OpenWorldSelectionPanel()
    {
        buttonOpenWorldSelectionPanel.GetComponent<Animator>().SetBool("ShowPanel", true);
        SetMainButtonsInteractable(false);

        for (int i = 0; i < 3; i++)
            buttonDeleteWorld[i].gameObject.SetActive(!(PlayerPrefs.GetInt(SaveData.NEW_WORLD + (i + 1)) == 0));

    }

    void SelectWorld(int worldToSelect) 
    {
        PlayerPrefs.SetInt(SaveData.SELECTED_WORLD, worldToSelect);

        if (PlayerPrefs.GetInt(SaveData.NEW_WORLD + worldToSelect) == 0)
            OpenWorldSettingsMenu();
        else
            SceneManager.LoadScene(worldToSelect);
    }
    void DeleteWorld(int worldToDelete)
    {
        buttonDeleteWorld[worldToDelete - 1].gameObject.SetActive(false);
        PlayerPrefs.SetInt(SaveData.NEW_WORLD + worldToDelete, 0);


        // Open confirmation panel
        // Clear all world related playerprefs
    }

    void OpenQuitPanel()
    {
        buttonOpenQuitGamePanel.GetComponent<Animator>().SetBool("ShowPanel", true);
        SetMainButtonsInteractable(false);
    }
    void QuitGame() => Application.Quit();

    void BackToMainMenu(InputAction.CallbackContext context)
    {
        SetMainButtonsInteractable(true);

        buttonOpenWorldSelectionPanel.GetComponent<Animator>().SetBool("ShowPanel", false);
        buttonOpenQuitGamePanel.GetComponent<Animator>().SetBool("ShowPanel", false);

        worldSettingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    void BackToMainMenu()
    {
        SetMainButtonsInteractable(true);

        buttonOpenWorldSelectionPanel.GetComponent<Animator>().SetBool("ShowPanel", false);
        buttonOpenQuitGamePanel.GetComponent<Animator>().SetBool("ShowPanel", false);

        worldSettingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }


    void SetMainButtonsInteractable(bool activeOrNot)
    {
        buttonContinue.interactable = ((PlayerPrefs.GetInt(SaveData.LAST_WORLD) == 0) || activeOrNot == false) ? false : true;

        buttonOpenWorldSelectionPanel.interactable  = activeOrNot;
        buttonOpenControlsMenu.interactable         = activeOrNot;
        buttonOpenQuitGamePanel.interactable        = activeOrNot;
    }

    #endregion

    #region World settings

    void OpenWorldSettingsMenu() => worldSettingsMenu.SetActive(true);


    void StartGame()
    {
        WorldSettingsManager.SaveAllSetings();
        SceneManager.LoadScene(PlayerPrefs.GetInt(SaveData.SELECTED_WORLD));
    }


    #endregion

    #region Controls menu

    void OpenControlsMenu() => controlsMenu.SetActive(true);


    private void ResetControlsSettings()
    {
        foreach(InputActionMap map in input.actionMaps)
            map.RemoveAllBindingOverrides();

        PlayerPrefs.DeleteKey("rebinds");
    }

    #endregion



}
