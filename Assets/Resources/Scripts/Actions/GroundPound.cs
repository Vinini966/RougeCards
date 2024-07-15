using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPound : IAction
{
    public bool CanPerformAction()
    {
        return true;
    }

    public void PerformAction(PrototypeHero prototypeHero)
    {

        if (!prototypeHero.m_grounded)
        {
            prototypeHero.m_animator.SetTrigger("AttackAirSlam");
            prototypeHero.m_body2d.velocity = new Vector2(0.0f, -prototypeHero.m_jumpForce);
            prototypeHero.m_disableMovementTimer = 0.8f;

            //Reset timer
            prototypeHero.m_timeSinceAttack = 0.0f;
        }
    }
}
