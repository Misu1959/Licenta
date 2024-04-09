using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyMethods
{
    public static bool CheckIfMouseIsOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        return raycastResultsList.Where(r => r.gameObject.layer == 5).Count() > 0;

    }

    public static bool CheckIfMouseIsOverItem()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        return raycastResultsList.Count == 1 ? false : true;


    }

    public static int LayerToLayerMask(int layerNr) => (int)Mathf.Pow(2, layerNr);
}
