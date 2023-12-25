using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager instance;

    [SerializeField] private GameObject popUpPrefab;

    private void Start()
    {
        instance = this;
    }

    void SetPopUp(GameObject popUp)
    {
        Vector2 offset = new Vector2(Random.Range(-.35f, .35f), Random.Range(.6f, .8f));
        popUp.transform.position = (Vector2)PlayerStats.instance.gameObject.transform.position + offset;
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

    public void ShowPopUpActionCanceled()
    {
        GameObject popUp = Instantiate(popUpPrefab);
        SetPopUp(popUp);

        popUp.GetComponent<TextMeshPro>().text = "Action canceled!";

    }
}
