using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRange : MonoBehaviour
{
    private enum RangeType
    {
        darkness,
        action,
        search,
        attack
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
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>() || other.GetComponent<MobStats>()) // Check if other object is item or resource 
                PlayerBehaviour.instance.itemsInRange.Add(other.transform); //add it to the list

        }

        if (CheckForSpecificCollider(RangeType.attack))
        {
            if (other.transform == PlayerBehaviour.instance.currentTarget)
                if (other.gameObject.GetComponent<MobStats>())
                {
                    EquipmentManager.instance.GetHandItem()?.GetComponent<EquipmentUI>().UseTool();
                    other.gameObject.GetComponent<MobStats>().TakeDmg(PlayerStats.instance.GetActualDamage());
                }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if(CheckForSpecificCollider(RangeType.action))
            if (PlayerBehaviour.instance.currentTarget == other.transform)// Check if player reached the target 
                PlayerBehaviour.instance.PerformAction(); //perform the action

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
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>() || other.GetComponent<MobStats>()) // Check if other object is an item or a res
                PlayerBehaviour.instance.itemsInRange.Remove(other.transform); // Eliminate it from the list
            // Since we know that it entered in collision with search collider we don't need to check if it's in the list
        }
    }

    bool CheckForSpecificCollider(RangeType rangeTypeToCheck) => (rangeType == rangeTypeToCheck) ? true : false;

}
