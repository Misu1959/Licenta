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

    private void Start() => SetButtonsFunctionality();

    #region Buttons setup

    void SetButtonsFunctionality()
    {
        PlayerInputActions inputActions = new PlayerInputActions();
        inputActions.Menu.Enable();
        inputActions.Menu.BackToMenu.performed += BackToMainMenu;

        buttonContinue.interactable = (PlayerPrefs.GetInt("LastWorld") == 0) ? false : true;
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
    }
    void SetControlsMenuButtons()
    {
        buttonOpenControlsMenu.onClick.AddListener(() => OpenControlsMenu());
        buttonBackFromControlsMenu.onClick.AddListener(() => BackToMainMenu());
        buttonResetControls.onClick.AddListener(() => ResetControlsSettings());
    }






    #endregion

    #region Main menu

    void ContinueGame() => SceneManager.LoadScene(PlayerPrefs.GetInt("LastWorld"));

    void OpenWorldSelectionPanel()
    {
        buttonOpenWorldSelectionPanel.GetComponent<Animator>().SetBool("ShowPanel", true);
        SetMainButtonsInteractable(false);

        for (int i = 0; i < 3; i++)
            buttonDeleteWorld[i].gameObject.SetActive(!(PlayerPrefs.GetInt("World " + (i + 1) + " day") == 0));
        
    }

    void SelectWorld(int worldToSelect) 
    {
        SceneManager.LoadScene(worldToSelect);
        return;

        if (PlayerPrefs.GetInt("World " + worldToSelect + " day") == 0)
        {
            OpenWorldSettingsMenu();
            PlayerPrefs.SetInt("SelectedWorld", worldToSelect);
        }
        else
            SceneManager.LoadScene(worldToSelect);
    }
    void DeleteWorld(int worldToDelete)
    {
        buttonDeleteWorld[worldToDelete - 1].gameObject.SetActive(false);
        
        Debug.Log("Delete "+ worldToDelete);
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
        buttonContinue.interactable = ((PlayerPrefs.GetInt("LastWorld") == 0) || activeOrNot == false) ? false : true;

        buttonOpenWorldSelectionPanel.interactable  = activeOrNot;
        buttonOpenControlsMenu.interactable         = activeOrNot;
        buttonOpenQuitGamePanel.interactable        = activeOrNot;
    }

    #endregion

    #region World settings

    void OpenWorldSettingsMenu() => worldSettingsMenu.SetActive(true);


    void StartGame() => SceneManager.LoadScene(PlayerPrefs.GetInt("SelectedWorld"));

    void ApplySettings()
    {

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
