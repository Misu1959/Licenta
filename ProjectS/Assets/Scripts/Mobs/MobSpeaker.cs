using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobSpeaker : MonoBehaviour
{
    static System.Random rnd = new System.Random();

    [SerializeField] private List<string> voiceLines;
    private int previousVoiceLine = -1;

    private Timer voiceLineTimer;

    private void Start() => voiceLineTimer = new Timer(30);

    private void Update() => SayVoicelineOnTime();

    private void SayVoicelineOnTime()
    {
        voiceLineTimer.StartTimer();
        voiceLineTimer.Tick();

        if (!voiceLineTimer.IsElapsed()) return;

        SayVoiceline();
    }

    private void SayVoiceline()
    {
        if (voiceLines.Count == 0)  // Remove for final version
        {
            Destroy(this);
            return; 
        }

        int nr = rnd.Next(voiceLines.Count);

        if (nr != previousVoiceLine)
        {
            previousVoiceLine = nr;
            PopUpManager.instance.ShowPopUp(this.transform, voiceLines[nr]);
        }
        else
            SayVoiceline();
    }

}
