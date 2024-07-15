using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManger : MonoBehaviour
{
    public StandardInputActions ActionMap;

    // Start is called before the first frame update
    void Awake()
    {
        ActionMap = new StandardInputActions();
    }

    public void DisableAllControls()
    {
        DisablePlayerControls();
        DisableUIControls();
    }

    public void EnablePlayerControls(bool onlyEnable = false)
    {
        if (!onlyEnable)
            DisableAllControls();

        ActionMap.Player.Enable();
    }

    public void DisablePlayerControls()
    {
        ActionMap.Player.Disable();
    }

    public void EnableUIControls(bool onlyEnable = false)
    {
        if (!onlyEnable)
            DisableAllControls();

        ActionMap.UI.Enable();
    }

    public void DisableUIControls()
    {
        ActionMap.UI.Disable();
    }


    public void OnControlsChanged()
    {
        Debug.Log("Test");
    }
}
