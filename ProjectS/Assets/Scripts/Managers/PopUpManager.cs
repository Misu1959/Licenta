using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpManager : MonoBehaviour
{
    public enum PopUpPriorityLevel
    {
        none,
        low,
        medium,
        high,
        maximum
    }

    public static PopUpManager instance;
    
    private PopUpPriorityLevel popUpPriorityLevel;
    private PopUpPriorityLevel previousPopUpPriorityLevel;
    [SerializeField] private GameObject mousePopUp;
    [SerializeField] private GameObject popUpPrefab;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }

    private void Update() => MoveMousePopUp();
    void SetPopUp(Transform location,GameObject popUp)
    {
        Vector3 offset = new Vector3(Random.Range(-.35f, .35f),1, Random.Range(.6f, .8f));
        popUp.transform.position = location.position + offset;
        Destroy(popUp, .75f);
    }

    public void ShowPopUp(Transform location, string textInput)
    {
        GameObject popUp = Instantiate(popUpPrefab);
        SetPopUp(location, popUp);

        popUp.GetComponent<TextMeshPro>().text = textInput;

    }

    public void ShowPopUpDarkness(int treshhold)
    {
        GameObject popUp = Instantiate(popUpPrefab);
        SetPopUp(PlayerStats.instance.transform, popUp);

        if (treshhold == 1)
            popUp.GetComponent<TextMeshPro>().text = "What was that!";
        else if (treshhold == 2)
            popUp.GetComponent<TextMeshPro>().text = "Who's there?";
        else if (treshhold == 3)
            popUp.GetComponent<TextMeshPro>().text = "Ouch";

    }



    public void ShowMousePopUp(string popUpText, PopUpPriorityLevel priority)
    {
        if (priority >= popUpPriorityLevel)
        {
            popUpPriorityLevel = priority;

            if (mousePopUp.GetComponent<TextMeshProUGUI>().text != popUpText)
                mousePopUp.GetComponent<TextMeshProUGUI>().text = popUpText;
        }
        else
        {
            if(previousPopUpPriorityLevel < popUpPriorityLevel &&
                priority < popUpPriorityLevel)
                popUpPriorityLevel = priority;


        }
        previousPopUpPriorityLevel = priority;
    }

    private void MoveMousePopUp()
    {
        mousePopUp.transform.position = (Vector2)Input.mousePosition + new Vector2(0,50);
        ShowMousePopUp("", PopUpPriorityLevel.none);
    }

}
