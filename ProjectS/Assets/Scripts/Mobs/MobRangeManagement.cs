using UnityEngine;

public class MobRangeManagement : MonoBehaviour
{
    private enum RangeType
    {
        action,
        search
    }

    [SerializeField] private RangeType rangeType;

    private void OnTriggerEnter(Collider other)
    {

        if (CheckForSpecificCollider(RangeType.search)) //Checking if something enter collision with search collider
        {
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is item or resource 
                transform.parent.GetComponent<MobActionManagement>().itemsInRange.Add(other.gameObject); //add it to the list

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (CheckForSpecificCollider(RangeType.action))
            if (transform.parent.GetComponent<MobActionManagement>().currentTarget == other.gameObject)// Check if player reached the target 
                transform.parent.GetComponent<MobActionManagement>().PerformAction(); //perform the action
    }

    private void OnTriggerExit(Collider other)
    {
        if (CheckForSpecificCollider(RangeType.search)) //Checking if something exit collision with search collider
        {
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is an item or a res
                transform.parent.GetComponent<MobActionManagement>().itemsInRange.Remove(other.gameObject); // Eliminate it from the list
        }
    }

    bool CheckForSpecificCollider(RangeType rangeTypeToCheck) => (rangeType == rangeTypeToCheck) ? true : false;

}
