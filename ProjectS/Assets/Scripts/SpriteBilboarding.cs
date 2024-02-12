using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBilboarding : MonoBehaviour
{
    private Camera mainCam;
    void Start() => mainCam = Camera.main;

    void LateUpdate() => transform.rotation = mainCam.transform.rotation;
}
