using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyMethods : MonoBehaviour
{
    public static bool CheckIfMouseIsOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        foreach (RaycastResult objRes in raycastResultsList)
            if (objRes.gameObject.GetComponent<CanvasRenderer>())
                return true;

        return false;
    }

    public static Fire CheckIfMouseIsOverFire()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        foreach (RaycastResult objRes in raycastResultsList)
            if (objRes.gameObject.GetComponent<Fire>())
                return objRes.gameObject.GetComponent<Fire>();

        return null;
    }
}
