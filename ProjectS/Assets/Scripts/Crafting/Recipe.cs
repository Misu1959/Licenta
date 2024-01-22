using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Recipe : ScriptableObject
{
    public Sprite recipeUI;
    public GameObject prefabItem;
    public bool isLearned;

    [System.Serializable]
    public struct Requiremets
    {
        public Item.Name name;
        public int quantity;
    };
    public Requiremets[] requirements;


}
