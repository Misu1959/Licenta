using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    public Animator animator { get; private set; }
    public Rigidbody rigidBody { get; private set; }

    public PlayerInputActions inputActions { get; private set; }

    [SerializeField] private InputActionAsset asset;

    #region Stats
    [SerializeField] private int maxHunger;
    public int hunger { get; private set; }

    private Timer hungerTimer;
    private Timer starveTimer;

    [SerializeField] private int maxHp;
    public int hp { get; private set; }
    
    [SerializeField] private int maxSpeed;
    public int speed { get; private set; }

    #endregion
    
    public int isInLight { get; private set; }
    public int researchLevel { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputActions(asset);
        inputActions.Enable();
    }

    void Start()
    {
        instance = this;
        animator = transform.GetChild(0).GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        hungerTimer = new Timer(1);
        hungerTimer.StartTimer();
        
        starveTimer = new Timer(1);

        hunger = maxHunger;
        hp = maxHp;
        //if (PlayerPrefs.GetInt("prevWorld") <= 1)
            StartCoroutine(SetStats(maxHp,maxSpeed,maxHunger,Vector2.zero));
    
    }

    private void Update()
    {
        
        hungerTimer.Tick();
        if(hungerTimer.IsElapsed())
        {
            // Every second lose 1 hunger
            hunger = Mathf.Clamp(hunger - 1, 0, maxHunger);
            UIManager.instance.ShowHunger(maxHunger, hunger);
            hungerTimer.StartTimer();
        }

        if (hunger == 0)
        {
            // If starving every second lose 2 hp
            starveTimer.StartTimer();
            starveTimer.Tick();
            if (starveTimer.IsElapsed())
            {
                Starve(2);
                starveTimer.StartTimer();
            }
        }
        else
            starveTimer.RestartTimer();
    }

    public IEnumerator SetStats(int _hp, int _speed, int _hunger, Vector2 pos)
    {
        yield return null;
        hp      = _hp;
        speed   = _speed;
        hunger  = _hunger;
        transform.position = pos;

        UIManager.instance.ShowHp(maxHp, hp);
        UIManager.instance.ShowHunger(maxHunger, hunger);
    }

    public void TakeDmg(int dmgAmount)
    {
        hp = Mathf.Clamp(hp - dmgAmount, 0, maxHp);
        UIManager.instance.ShowHp(maxHp, hp);

        if (PlayerController.instance.canMove && PlayerBehaviour.instance.currentTarget != null)
            PlayerBehaviour.instance.CancelAction(actionInterrupted: true);

        PlayerController.instance.SetCanMove();
        animator.SetTrigger("Hit");
        


        if (hp <= 0)
            Die("by taking damage!");
    }

    private void Starve(int dmgAmount)
    {
        hp = Mathf.Clamp(hp - dmgAmount, 0, maxHp);
        UIManager.instance.ShowHp(maxHp, hp);


        if (hp <= 0)
            Die("by starvation!");
    }

    public void Heal(int healAmount)
    {
        hp = Mathf.Clamp(hp + healAmount, 0, maxHp);
        UIManager.instance.ShowHp(maxHp, hp);
    }

    public void Eat(Item_Base food)
    {
        hunger  = (int)Mathf.Clamp(hunger + food.GetFoodData().hungerAmount, 0, maxHunger);
        hp      = (int)Mathf.Clamp(hp + food.GetFoodData().hpAmount, 0, maxHp);

        food.TakeFromStack(1);

        if (hp <= 0)
            Die("by eating bad food!");

        UIManager.instance.ShowHunger(maxHunger, hunger);
        UIManager.instance.ShowHp(maxHp, hp);
    }

    void Die(string causeOfDeath)   
    {
        animator.SetTrigger("Die");
        UIManager.instance.ShowDeathScreen(causeOfDeath);   
    }


    public int GetActualDamage()
    {
        if (!EquipmentManager.instance.GetHandItem()) return 0;

        if (EquipmentManager.instance.GetHandItem().GetEquipmentData().actionType == EquipmentActionType.fight)
            return EquipmentManager.instance.GetHandItem().GetEquipmentData().dmg;
        else
            return EquipmentManager.instance.GetHandItem().GetEquipmentData().dmg / 2;

    }




    public void SetResearchLevel(int newResearchLevel)  =>   researchLevel = newResearchLevel;   

    public void SetInLight(int _isIntLight) => isInLight += _isIntLight;  
}
