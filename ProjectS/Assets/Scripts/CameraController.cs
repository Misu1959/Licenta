using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class CameraController : MonoBehaviour
{
    private Transform player;


    void Start() { player = GameObject.FindGameObjectWithTag("Player").transform;    }

    void LateUpdate() { Move(); }

    private void Move()
    {
        transform.position = new Vector3(player.position.x, player.position.y + 30, player.transform.position.z - 14);
    }




}
