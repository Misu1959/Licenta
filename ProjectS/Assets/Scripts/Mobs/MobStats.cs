using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobStats : MonoBehaviour,IPointerDownHandler
{

    public Rigidbody  rigidBody     { get; private set; }
    public Animator   animator      { get; private set; }

    public Transform spawner { get; private set; }

    [Header("Stats")]

    public ObjectName objectName;

    
    [SerializeField] TimeManager.DayState sleepPeriod;
    public TimeManager.DayState GetSleepPeriod() => sleepPeriod;



    [SerializeField] private int maxHp;
    public int hp { get; private set; }


    [SerializeField] private int dmg;
    [SerializeField] private int walkSpeed;
    [SerializeField] private int runSpeed;

    public int GetDmg() => dmg;
    public int GetWalkSpeed() => walkSpeed;
    public int GetRunSpeed() => runSpeed;


    private void Awake()
    {
        rigidBody   = GetComponent<Rigidbody>();
        animator    = transform.GetChild(0).GetComponent<Animator>();
    }
    public void SetMobData(Transform _spawner)
    {
        spawner = _spawner;
        spawner.GetComponent<MobSpawner>().AddMobToList(this);
  
        hp = maxHp;
    }
    public void SetMobData(Transform _spawner, int _hp) 
    {
        spawner = _spawner;
        spawner.GetComponent<MobSpawner>().AddMobToList(this);

        hp = _hp; 
    
//        sleepPeriod = ItemsManager
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!InteractionManager.instance.CanPlayerInteractWithWorld(false)) return;

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
