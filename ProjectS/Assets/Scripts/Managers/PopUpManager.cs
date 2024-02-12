using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PopUpManager : MonoBehaviour
{
    public enum PopUpPriorityLevel
    {
        low,
        medium,
        high,
        maximum
    }

    public static PopUpManager instance;
    
    private PopUpPriorityLevel popUpPriorityLevel;
    [SerializeField] private GameObject mousePopUp;
    [SerializeField] private GameObject popUpPrefab;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        MoveMousePopUp();
    }

    void SetPopUp(GameObject popUp)
    {
        Vector3 offset = new Vector3(Random.Range(-.35f, .35f),1, Random.Range(.6f, .8f));
        popUp.transform.position = PlayerStats.instance.gameObject.transform.position + offset;
        Destroy(popUp, .75f);
    }

    public void ShowPopUpDarkness(int treshhold)
    {
        GameObject popUp = Instantiate(popUpPrefab);
        SetPopUp(popUp);

        if (treshhold == 1)
            popUp.GetComponent<TextMeshPro>().text = "What was that!";
        else if (treshhold == 2)
            popUp.GetComponent<TextMeshPro>().text = "Who's there?";
        else
            popUp.GetComponent<TextMeshPro>().text = "Ouch";

    }

    public void ShowPopUpAction(string textInput = "")
    {
        GameObject popUp = Instantiate(popUpPrefab);
        SetPopUp(popUp);

        popUp.GetComponent<TextMeshPro>().text = textInput;

    }


    public void ShowMousePopUp(string popUpText = "", PopUpPriorityLevel priority = PopUpPriorityLevel.low)
    {
        if (popUpText == "")
            popUpPriorityLevel = PopUpPriorityLevel.low;
        else if (popUpPriorityLevel <= priority)
            popUpPriorityLevel = priority;

        if (popUpPriorityLevel <= priority)
            if (mousePopUp.GetComponent<TextMeshProUGUI>().text != popUpText)
                mousePopUp.GetComponent<TextMeshProUGUI>().text = popUpText;

    }

    private void MoveMousePopUp()
    {
        mousePopUp.transform.position = (Vector2)Input.mousePosition + new Vector2(0,50);
    }

}
