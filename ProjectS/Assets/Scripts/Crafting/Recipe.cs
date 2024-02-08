using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Recipe : ScriptableObject
{
    [System.Serializable]
    public struct Requiremets
    {
        public ItemData.Name name;
        public int quantity;
    };


    public Sprite recipeUI;
    public GameObject prefabItem;
    public bool isLearned;


    public Requiremets[] requirements;


}
