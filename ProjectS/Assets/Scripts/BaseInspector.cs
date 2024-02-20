using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseInspector : MonoBehaviour, IPointerDownHandler
{
    private static System.Random rnd = new System.Random();
    private int previousInspection = -1;

    [TextArea(1, 3)]
    [SerializeField] protected string hoverText;
    [SerializeField] protected List<string> inspectionTexts;

    public void OnPointerDown(PointerEventData eventData) => Inspect();
    private void Inspect()
    {
        if (!Input.GetMouseButtonDown(2)) return;

        int nr = rnd.Next(inspectionTexts.Count);
        if (nr != previousInspection || inspectionTexts.Count == 1)
        {
            previousInspection = nr;
            PopUpManager.instance.ShowPopUp(PlayerStats.instance.transform, inspectionTexts[nr]);
        }
        else
            Inspect();
    }

}