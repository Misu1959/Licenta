using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Recipe : ScriptableObject
{
    [System.Serializable]
    public struct Requiremets
    {
        public ObjectName name;
        public int quantity;
    };


    public Sprite recipeUI;
    public ObjectName objectName;
    public bool isLearned;


    public Requiremets[] requirements;


}
