using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    #region Stats
    [SerializeField] private float maxHunger;
    public float hunger { get; private set; }
    
    private float hungerTimer = 1;
    private float starveTimer = 0;

    [SerializeField] private float maxHp;
    public float hp { get; private set; }

    [SerializeField] private float maxDmg;
    public float dmg { get; private set; }
    
    [SerializeField] private float maxSpeed;
    public float speed { get; private set; }

    #endregion
    public int isInLight { get; private set; }
    public int researchLevel { get; private set; }

    void Start()
    {
        instance = this;

        hunger = maxHunger;
        hp = maxHp;
        //if (PlayerPrefs.GetInt("prevWorld") <= 1)
            StartCoroutine(SetStats(maxHp,maxDmg,maxSpeed,maxHunger,Vector2.zero));
    
    }

    private void Update()
    {
        hungerTimer -= Time.deltaTime;
        if(hungerTimer<=0)
        {
            // Every second lose 1 hunger
            hunger = Mathf.Clamp(hunger - 1, 0, maxHunger);
            UIManager.instance.ShowHunger(maxHunger, hunger);
            hungerTimer = 1;
        }

        if (hunger==0)
        {
            // If starving every second lose 2 hp
            starveTimer -= Time.deltaTime;
            if(starveTimer<=0)
            {
                TakeDmg(2);
                starveTimer = 1;
            }

        }
    }

    public IEnumerator SetStats(float _hp,float _dmg,float _speed,float _hunger, Vector2 pos)
    {
        yield return null;
        hp      = _hp;
        dmg     = _dmg;
        speed   = _speed;
        hunger  = _hunger;
        transform.position = pos;

        UIManager.instance.ShowHp(maxHp, hp);
        UIManager.instance.ShowHunger(maxHunger, hunger);
    }

    public void TakeDmg(float dmgAmount)
    {
        hp = Mathf.Clamp(hp - dmgAmount, 0, maxHp);
        UIManager.instance.ShowHp(maxHp, hp);


        if (hp <= 0)
            Die();
    }

    public void Heal(float healAmount)
    {
        hp = Mathf.Clamp(hp + healAmount, 0, maxHp);
        UIManager.instance.ShowHp(maxHp, hp);
    }

    public void Eat(float hungerAmount, float healAmount)
    {
        hunger = Mathf.Clamp(hunger + hungerAmount, 0, maxHunger);
        hp = Mathf.Clamp(hp + healAmount, 0, maxHp);

        if (hp <= 0)
            Die();

        UIManager.instance.ShowHunger(maxHunger, hunger);
        UIManager.instance.ShowHp(maxHp, hp);
    }

    void Die()
    {
        UIManager.instance.ShowDeathScreen();
        Time.timeScale = 0;
        PlayerPrefs.SetInt("prevWorld", 0);
    }

    public void SetResearchLevel(int newResearchLevel)
    {
        researchLevel = newResearchLevel;
        CraftingManager.instance.SetTooltipCraftButton();
    }

    public void SetInLight(int _isIntLight)
    {
        isInLight += _isIntLight;
    }

}
