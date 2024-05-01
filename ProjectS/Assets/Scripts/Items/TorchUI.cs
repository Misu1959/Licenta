using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchUI : MonoBehaviour
{
    private Timer torchTimer;
    private bool isEquipped;
    public void SetisEquiped(bool _isEquipped)
    {
        isEquipped = _isEquipped;
        PlayerStats.instance.transform.GetChild(4).gameObject.SetActive(isEquipped);

        int val = isEquipped == false ? -1 : 1;
        PlayerStats.instance.SetInLight(val);

    }

    private void Start() => torchTimer = new Timer(1);

    void Update() => ConsumeTorch();

    private void ConsumeTorch()
    {
        if (!isEquipped)
        {
            torchTimer.RestartTimer();
            return;
        }

        torchTimer.StartTimer();
        torchTimer.Tick();

        if (!torchTimer.IsElapsed()) return;
        torchTimer.RestartTimer();

        GetComponent<EquipmentUI>().UseTool();
    }

    private void OnDestroy() => SetisEquiped(false);
}
