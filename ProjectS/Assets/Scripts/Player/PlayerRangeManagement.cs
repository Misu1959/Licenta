using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangeManagement : MonoBehaviour
{
    private static GameObject playerBody;
    private static GameObject darknessCollider;
    private static GameObject actionCollider;
    private static GameObject searchCollider;


    IEnumerator Start()
    {
        yield return null; // Wait a frame so that PlayerStats can initialize it's instance

        playerBody       = PlayerStats.instance.gameObject;
        darknessCollider = PlayerStats.instance.gameObject.transform.GetChild(0).gameObject; // Set darkness collider
        actionCollider   = PlayerStats.instance.gameObject.transform.GetChild(1).gameObject; // Set action collider
        searchCollider   = PlayerStats.instance.gameObject.transform.GetChild(2).gameObject; // Set search collider
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (CheckForSpecificCollider(darknessCollider)) //Checking if something enter collision with darkness collider
        {
            if (other.gameObject.GetComponent<Light2D>()) // If other object has light source
                PlayerStats.instance.SetInLight(1);// Add 1 to numbers of light the player is in
        }
        if (CheckForSpecificCollider(actionCollider))  
        {
            if (other.gameObject.GetComponent<Construction>()) // If other object is construction 
                // Check if it's researchStation
                PlayerStats.instance.SetResearchLevel(1); // Set player research level

            

        }
        if (CheckForSpecificCollider(searchCollider)) //Checking if something enter collision with search collider
        {
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is item or resource 
                PlayerActionManagement.instance.itemsInRange.Add(other.gameObject); //add it to the list

        }

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (CheckForSpecificCollider(playerBody))
            if (other.GetComponent<SpriteRenderer>().sortingOrder > GetComponent<SpriteRenderer>().sortingOrder)
            {
                Color objColor = other.GetComponent<SpriteRenderer>().color;
                other.GetComponent<SpriteRenderer>().color = new Color(objColor.r, objColor.g, objColor.b, .5f);
            }
            else if (other.GetComponent<SpriteRenderer>().sortingOrder <= GetComponent<SpriteRenderer>().sortingOrder)
            {
                Color objColor = other.GetComponent<SpriteRenderer>().color;
                other.GetComponent<SpriteRenderer>().color = new Color(objColor.r, objColor.g, objColor.b, 1);
            }

        if(CheckForSpecificCollider(actionCollider))
            if (PlayerActionManagement.instance.currentTarget == other.gameObject)// Check if player reached the target 
                PlayerActionManagement.instance.PerformAction(); //perform the action

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CheckForSpecificCollider(playerBody))
        {
            Color objColor = other.GetComponent<SpriteRenderer>().color;
            other.GetComponent<SpriteRenderer>().color = new Color(objColor.r, objColor.g, objColor.b, 1);
        }

        if (CheckForSpecificCollider(darknessCollider)) //Checking if something exit collision with darkness collider
        {
            if (other.gameObject.GetComponent<Light2D>()) // If player get's out of the light 
                PlayerStats.instance.SetInLight(-1); // Remove one from the number of lights player is in
        }

        if (CheckForSpecificCollider(actionCollider)) //Checking if something exit collision with action collider
        {
            if (other.GetComponent<Construction>()) // If the other object is a construction 
            {
                //It has bugs

                // If the other object is a resarchStation
                if (other.gameObject.layer == 10)
                    PlayerStats.instance.SetResearchLevel(0); // Set player research level

            }
        }

        if (CheckForSpecificCollider(searchCollider)) //Checking if something exit collision with search collider
        {
            if (other.GetComponent<Item_Base>() || other.GetComponent<Resource>()) // Check if other object is an item or a res
                PlayerActionManagement.instance.itemsInRange.Remove(other.gameObject); // Eliminate it from the list
            // Since we know that it entered in collision with search collider we don't need to check if it's in the list
        }
    }

    bool CheckForSpecificCollider(GameObject collider)
    {
        return (this.gameObject == collider) ? true : false;
    }
}
