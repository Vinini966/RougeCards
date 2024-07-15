using Databox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDatabaseManager : MonoBehaviour
{
    public DataboxObject ActionDB;

    bool _ready;

    // Start is called before the first frame update
    void Awake()
    {
        ActionDB.LoadDatabase();
    }

    void OnEnable()
    {
        ActionDB.OnDatabaseLoaded += DataReady;
    }

    void OnDisable()
    {
        ActionDB.OnDatabaseLoaded -= DataReady;
    }

    private void DataReady()
    {
        _ready = true;
    }

    public IAction GetAction(string actionName)
    {
        if (!_ready)
            new Exception("Action Database is not loaded.");

        return ActionDB.GetData<ResourceType>("ActiveActionsDB", actionName, "Action").ToIAction();
    }
}
