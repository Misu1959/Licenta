using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobStats : MonoBehaviour,IPointerDownHandler
{

    public Rigidbody  rigidBody     { get; private set; }
    public Animator   animator      { get; private set; }


    private Transform spawner;

    [Header("Stats")]

    public new ObjectName name;

    [SerializeField] TimeManager.DayState sleepPeriod;

    [SerializeField] private int maxHp;
    public int hp { get; private set; }

    [SerializeField] private int dmg;


    [SerializeField] private int walkSpeed;
    [SerializeField] private int runSpeed;

    public TimeManager.DayState GetSleepPeriod() => sleepPeriod;
    public Transform GetSpawner() => spawner;
    public void SetSpawner(Transform _spawner) => spawner = _spawner;


    public int GetDmg() => dmg;
    public int GetWalkSpeed() => walkSpeed;
    public int GetRunSpeed() => runSpeed;

    private void Start()
    {
        rigidBody   = GetComponent<Rigidbody>();
        animator    = transform.GetChild(0).GetComponent<Animator>();

        hp = maxHp;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.CanPlayerInteractWithWorld(false)) return;

        if (Input.GetMouseButton(0))
            if (PlayerStats.instance.GetActualDamage() > 0)
                PlayerBehaviour.instance.SetTargetAndAction(transform, PlayerBehaviour.Action.attack);
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
        GetComponent<MobBehaviour>().FightOrFlight();

        animator.SetTrigger("Hit");

        hp = Mathf.Clamp(hp - amount, 0, maxHp);
        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        GetComponent<LootManagement>().DropLoot();

        animator.SetTrigger("Die");
        Destroy(this.gameObject, 1);
    }


}
