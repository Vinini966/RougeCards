using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionEquipManager : MonoBehaviour
{
    //Change to card
    IAction[] EquipedActions = new IAction[3];
    PrototypeHero _player;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.InputManger.ActionMap.Player.Action1.performed += Action1Performed;
        GameManager.Instance.InputManger.ActionMap.Player.Action2.performed += Action2Performed;
        GameManager.Instance.InputManger.ActionMap.Player.Action3.performed += Action3Performed;
        HeroFactory.HeroInitilized += GetPlayer;
    }

    void GetPlayer(PrototypeHero actor)
    {
        _player = actor;
    }

    private void Action3Performed(InputAction.CallbackContext obj)
    {
        
        if (EquipedActions[2] != null && _player.m_timeSinceAttack > 0.2f && EquipedActions[2].CanPerformAction())
            EquipedActions[2].PerformAction(_player);
    }

    private void Action2Performed(InputAction.CallbackContext obj)
    {
        if (EquipedActions[1] != null && _player.m_timeSinceAttack > 0.2f && EquipedActions[1].CanPerformAction())
            EquipedActions[1].PerformAction(_player);
    }

    private void Action1Performed(InputAction.CallbackContext obj)
    {
        if (EquipedActions[0] != null && _player.m_timeSinceAttack > 0.2f && EquipedActions[0].CanPerformAction())
            EquipedActions[0].PerformAction(_player);
    }

    public void EquipAction(int slot, string action)
    {
        EquipedActions[slot] = GameManager.Instance.ActionDatabaseManager.GetAction(action);
    }
}
