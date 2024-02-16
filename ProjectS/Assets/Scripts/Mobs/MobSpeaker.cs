using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobSpeaker : MonoBehaviour , IPointerDownHandler
{
    static System.Random rnd = new System.Random();

    [SerializeField] private List<string> voiceLines;
    private int previousVoiceLine = -1;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        SayVoiceLine();
    }

    private void SayVoiceLine()
    {
        int nr = rnd.Next(voiceLines.Count);

        if (nr != previousVoiceLine)
        {
            previousVoiceLine = nr;
            PopUpManager.instance.ShowPopUp(this.transform, voiceLines[nr]);
        }
        else
            SayVoiceLine();
    }

}
