using UnityEngine;

public class CameraController : MonoBehaviour
{
    //private Transform player;

    //void Awake() => player = GameObject.FindGameObjectWithTag("Player").transform;

    void LateUpdate() => Move();

    private void Move() => transform.position = new Vector3(PlayerStats.instance.transform.position.x,
                                                            PlayerStats.instance.transform.position.y + 30,
                                                            PlayerStats.instance.transform.position.z - 14);




}
