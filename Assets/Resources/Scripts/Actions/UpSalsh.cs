using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpSalsh : MonoBehaviour
{
    public bool CanPerformAction()
    {
        return true;
    }

    public void PerformAction(PrototypeHero prototypeHero)
    {
        if (prototypeHero.m_grounded)
        {
            prototypeHero.m_animator.SetTrigger("UpAttack");

            // Reset timer
            prototypeHero.m_timeSinceAttack = 0.0f;

            // Disable movement 
            prototypeHero.m_disableMovementTimer = 0.35f;

        }
        else
        {
            Debug.Log("Air attack up");
            prototypeHero.m_animator.SetTrigger("AirAttackUp");

            // Reset timer
            prototypeHero.m_timeSinceAttack = 0.0f;
        }
    }
}
