using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private Button buttonContinue;
    [SerializeField] private Button buttonNewGame;
    [SerializeField] private Button buttonSettings;
    [SerializeField] private Button buttonExit;

    private void Start()
    {
        SetButtonsFunctionality();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (buttonNewGame.GetComponent<Animator>().GetBool("confirmationMenuIsOpen"))
                DontConfirm();
    }


    void SetButtonsFunctionality()
    {
        if (PlayerPrefs.GetInt("prevWorld") == 0)
            buttonContinue.interactable = false;
        else
        {
            buttonContinue.interactable = true;
            buttonContinue.onClick.AddListener(() => ContinueGame());
        }

        buttonNewGame.onClick.AddListener(()  => NewGame());
        buttonSettings.onClick.AddListener(() => OpenSettings());
        buttonExit.onClick.AddListener(()     => ExitGame());
    
    

    }

    void ContinueGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void NewGame()
    {
        if(PlayerPrefs.GetInt("prevWorld") == 0)
        {
            Confirm();
            return;
        }
        buttonNewGame.GetComponent<Animator>().SetBool("confirmationMenuIsOpen", true);
        SetMainButtonsInteractable(false);

        buttonNewGame.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        buttonNewGame.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => DontConfirm());

        buttonNewGame.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        buttonNewGame.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => Confirm());

    }

    void DontConfirm()
    {
        buttonNewGame.GetComponent<Animator>().SetBool("confirmationMenuIsOpen", false);
        SetMainButtonsInteractable(true);
    }

    void Confirm()
    {
        PlayerPrefs.SetInt("prevWorld", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    void OpenSettings()
    {
        settingsMenu.SetActive(true);
        mainButtons.SetActive(false);

        settingsMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        settingsMenu.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => CloseSettings());
    }

    void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainButtons.SetActive(true);
    }

    void ExitGame()
    {
        Application.Quit();
    }


    void SetMainButtonsInteractable(bool activeOrNot)
    {
        buttonContinue.interactable = activeOrNot;
        buttonNewGame.interactable  = activeOrNot;
        buttonSettings.interactable = activeOrNot;
        buttonExit.interactable     = activeOrNot;
    }

}
