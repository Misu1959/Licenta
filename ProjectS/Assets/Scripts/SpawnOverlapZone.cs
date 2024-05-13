using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOverlapZone : MonoBehaviour
{

    IEnumerator Start()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(1);
        GetComponent<Rigidbody>().isKinematic = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<SpawnOverlapZone>())
            Destroy(this.gameObject);
    }
}
