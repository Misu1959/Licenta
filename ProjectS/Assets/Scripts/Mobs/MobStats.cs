using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobStats : MonoBehaviour
{
    public Animator animator { get; private set; }

    private Transform spawner;

    [Header("Stats")]

    public new ObjectName name;

    [SerializeField] private float maxHp;
    public float hp { get; private set; }

    [SerializeField] private float dmg;

    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;

    public float GetSpeed() => speed;
    public float GetRunSpeed() => runSpeed;
    public Transform GetSpawner() => spawner;
    public void SetSpawner(Transform _spawner) => spawner = _spawner;


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
