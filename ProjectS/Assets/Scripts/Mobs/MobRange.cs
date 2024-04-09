using UnityEngine;

public class MobRange : MonoBehaviour
{
    private enum RangeType
    {
        action,
        search,
        attack
    }

    [SerializeField] private RangeType rangeType;

    private void OnTriggerEnter(Collider other)
    {

        if (CheckForSpecificCollider(RangeType.search)) //Checking if something enter collision with search collider
        {
            if (other.gameObject.GetComponent<PlayerStats>())
                transform.parent.GetComponent<MobBehaviour>().isPlayerInRange = true;

            // if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is item or resource 
            //     transform.parent.GetComponent<MobBehaviour>().itemsInRange.Add(other.gameObject); //add it to the list

        }

        if(CheckForSpecificCollider(RangeType.attack))
        {
            if (transform.parent.parent.GetComponent<MobController>().currentTarget == other.transform)
                PlayerStats.instance.TakeDmg(transform.parent.parent.GetComponent<MobStats>().GetDmg());
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (CheckForSpecificCollider(RangeType.action))
            if (transform.parent.GetComponent<MobController>().currentTarget == other.transform)// Check if player reached the target 
                StartCoroutine(transform.parent.GetComponent<MobBehaviour>().CompleteAction()); //complete the action
    }

    private void OnTriggerExit(Collider other)
    {
        if (CheckForSpecificCollider(RangeType.search)) //Checking if something exit collision with search collider
        {
            if (other.gameObject.GetComponent<PlayerStats>())
                transform.parent.GetComponent<MobBehaviour>().isPlayerInRange = false;

          //  if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is an item or a res
          //      transform.parent.GetComponent<MobBehaviour>().itemsInRange.Remove(other.gameObject); // Eliminate it from the list
        }
    }


    bool CheckForSpecificCollider(RangeType rangeTypeToCheck) => (rangeType == rangeTypeToCheck) ? true : false;

}
