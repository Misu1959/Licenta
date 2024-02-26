using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangeManagement : MonoBehaviour
{
    private enum RangeType
    {
        darkness,
        action,
        search
    }
    [SerializeField] private RangeType rangeType;

    private void OnTriggerEnter(Collider other)
    {

        if (CheckForSpecificCollider(RangeType.darkness)) //Checking if something enter collision with darkness collider
        {
            if (other.gameObject.GetComponent<Light>()) // If other object has light source
                PlayerStats.instance.SetInLight(1);// Add 1 to numbers of light the player is in
        }

        if (CheckForSpecificCollider(RangeType.search)) //Checking if something enter collision with search collider
        {
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is item or resource 
                PlayerActionManagement.instance.itemsInRange.Add(other.gameObject); //add it to the list

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if(CheckForSpecificCollider(RangeType.action))
            if (PlayerActionManagement.instance.currentTarget == other.gameObject)// Check if player reached the target 
                PlayerActionManagement.instance.PerformAction(); //perform the action

    }

    private void OnTriggerExit(Collider other)
    {
        if (CheckForSpecificCollider(RangeType.darkness)) //Checking if something exit collision with darkness collider
        {
            if (other.gameObject.GetComponent<Light>()) // If player get's out of the light 
                PlayerStats.instance.SetInLight(-1); // Remove one from the number of lights player is in
        }

        if (CheckForSpecificCollider(RangeType.search)) //Checking if something exit collision with search collider
        {
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is an item or a res
                PlayerActionManagement.instance.itemsInRange.Remove(other.gameObject); // Eliminate it from the list
            // Since we know that it entered in collision with search collider we don't need to check if it's in the list
        }
    }

    bool CheckForSpecificCollider(RangeType rangeTypeToCheck) => (rangeType == rangeTypeToCheck) ? true : false;

}
