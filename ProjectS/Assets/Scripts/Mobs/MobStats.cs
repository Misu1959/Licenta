using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MobStats : MonoBehaviour
{
    public Animator animator { get; private set; }

    [Header("Stats")]

    public new ObjectName name;

    [SerializeField] private float maxHp;
    public float hp { get; private set; }

    [SerializeField] private float dmg;

    [SerializeField] private float speed;

    public float GetSpeed() => speed;

    private void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        hp = maxHp;
    }

    private void Regenerate()
    {

    }

    public void Heal(int amount)
    {

    }

    public void TakeDmg(int amount)
    {

    }



}
