using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public bool CanPerformAction();

    public void PerformAction(PrototypeHero prototypeHero);
}
