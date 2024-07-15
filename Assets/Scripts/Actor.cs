using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    float _currentHealth;
    float _currentMagic;


    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value;
            HealthChanged?.Invoke(this);
        }
    }
    public float Health;

    public float Attack;
    public Dictionary<string, float> AtkMods = new Dictionary<string, float>();

    public float Defence;
    public Dictionary<string, float> DefMods = new Dictionary<string, float>();


    public float CurrentMagic
    {
        get => _currentMagic;
        set
        {
            _currentMagic = value;
            MPChanged?.Invoke(this);
        }
    }
    public float Magic;

    public Action<Actor> HealthChanged;
    public Action<Actor> MPChanged;

    public abstract void Die();

    public void Awake()
    {
        CurrentHealth = Health;
        CurrentMagic = Magic;
    }

    public virtual void Damage(float attack)
    {
        float defWMods = Defence;
        float defTotalMods = DefMods.Sum(x => x.Value);
        defWMods *=(1 + defTotalMods);

        float atk = Mathf.Pow(attack, 2);
        float def = 25 * (defWMods + 0.01f);

        float damage = (float)Math.Round(atk / def, 2);


        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
            Die();
    }

    public float ModifyAttack()
    {
        return Attack * (1 + AtkMods.Sum(x => x.Value));
    }
}
