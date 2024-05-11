using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [SerializeField] private InputActionAsset inputAsset;
    PlayerInputActions inputActions;

    public InputActionAsset GetInputAsset() => inputAsset;
    public PlayerInputActions GetInputActions() => inputActions;

    private void Awake()
    {
        instance = this;
        inputActions = new PlayerInputActions(inputAsset);
        DisableAllMaps();
    }

    private void DisableAllMaps()
    {
        foreach (InputActionMap map in inputAsset.actionMaps)
            map.Disable();
    }

    public void SetActionMap(InputActionMap mapToSetActive)
    {
        foreach (InputActionMap map in inputAsset.actionMaps)
            map.Disable();

        mapToSetActive.Enable();
    }

}
