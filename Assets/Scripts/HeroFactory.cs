using Databox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFactory : MonoBehaviour
{
    public GameObject BaseHeroPrefab;

    public DataboxObject HeroDB;

    public static Action<PrototypeHero> HeroInitilized;

    public string HeroToMake
    {
        set
        {
            _heroToMake = value;
            _heroLoaded |= HeroLoaded.StringLoaded;
            CheckAllLoaded();
        }
    }

    string _heroToMake;

    [Flags]
    enum HeroLoaded
    {
        DatabaseLoaded = 1,
        StringLoaded = 1<<1,
        All = DatabaseLoaded | StringLoaded
    }

    HeroLoaded _heroLoaded = 0;

    private void Start()
    {
        HeroDB.LoadDatabase();
    }

    void OnEnable()
    {
        HeroDB.OnDatabaseLoaded += DataReady;
    }

    void OnDisable()
    {
        HeroDB.OnDatabaseLoaded -= DataReady;
    }

    public void MakeHero(string heroToMake)
    {
        GameObject go = Instantiate(BaseHeroPrefab, transform.position, transform.rotation);
        PrototypeHero chacterController = go.GetComponent<PrototypeHero>();
        GameObject sprite = HeroDB.GetData<ResourceType>("HeroDB", heroToMake, "Sprite").Load() as GameObject;
        Instantiate(sprite, go.transform);

        string baseAction = HeroDB.GetData<StringType>("HeroDB", heroToMake, "Base_Action").Value;
        chacterController.BaseAction = GameManager.Instance.ActionManager.GetAction(baseAction);

        chacterController.m_runSpeed = HeroDB.GetData<FloatType>("HeroDB", heroToMake, "Run_Speed").Value;
        chacterController.m_walkSpeed = HeroDB.GetData<FloatType>("HeroDB", heroToMake, "Walk_Speed").Value;
        chacterController.m_jumpForce = HeroDB.GetData<FloatType>("HeroDB", heroToMake, "Jump_Force").Value;
        chacterController.m_dodgeForce = HeroDB.GetData<FloatType>("HeroDB", heroToMake, "Dodge_Force").Value;
        chacterController.m_parryKnockbackForce = HeroDB.GetData<FloatType>("HeroDB", heroToMake, "Parry_Knockback").Value;

        chacterController.Initilize();
        HeroInitilized?.Invoke(chacterController);
    }

    void DataReady()
    {
        _heroLoaded |= HeroLoaded.DatabaseLoaded;
        CheckAllLoaded();
    }

    void CheckAllLoaded()
    {
        if (_heroLoaded.HasFlag(HeroLoaded.All))
        {
            MakeHero(_heroToMake);
        }
    }
}
