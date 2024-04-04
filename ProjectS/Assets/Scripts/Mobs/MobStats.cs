using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobStats : MonoBehaviour
{

    public Rigidbody  rigidBody     { get; private set; }
    public Animator   animator      { get; private set; }

    private Transform spawner;


    [Header("Stats")]

    public new ObjectName name;

    [SerializeField] TimeManager.DayState sleepPeriod;


    [SerializeField] private float maxHp;
    public float hp { get; private set; }

    [SerializeField] private float dmg;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;


    public TimeManager.DayState GetSleepPeriod() => sleepPeriod;
    public float GetWalkSpeed() => walkSpeed;
    public float GetRunSpeed() => runSpeed;

    public Transform GetSpawner() => spawner;
    public void SetSpawner(Transform _spawner) => spawner = _spawner;


    private void Start()
    {
        rigidBody   = GetComponent<Rigidbody>();
        animator    = transform.GetChild(0).GetComponent<Animator>();
        
        hp = maxHp;
    }

    private void Regenerate(int percent)
    {
        int amount = (int)((float)percent / 100 * maxHp);
        hp = Mathf.Clamp(hp + amount, 0, maxHp);
    }

    public void Heal(int amount)
    {
        hp = Mathf.Clamp(hp + amount, 0, maxHp);
    }

    public void TakeDmg(int amount)
    {
        //run or fight depending on agresivity
        animator.SetTrigger("Hit");

        hp = Mathf.Clamp(hp - amount, 0, maxHp);
        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Destroy(this.gameObject, 1);
    }
}
