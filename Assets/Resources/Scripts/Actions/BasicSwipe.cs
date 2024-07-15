using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSwipe : IAction
{
    public bool CanPerformAction()
    {
        return true;
    }

    public void PerformAction(PrototypeHero prototypeHero)
    {

        if (prototypeHero.m_grounded)
        {
            
            //if (upPressed)
            //{
            //    prototypeHero.m_animator.SetTrigger("UpAttack");

            //    // Reset timer
            //    prototypeHero.m_timeSinceAttack = 0.0f;

            //    // Disable movement 
            //    prototypeHero.m_disableMovementTimer = 0.35f;
            //}
            //else
            //{
                
            //}

            // Reset timer
            prototypeHero.m_timeSinceAttack = 0.0f;

            prototypeHero.m_currentAttack++;

            // Loop back to one after second attack
            if (prototypeHero.m_currentAttack > 2)
                prototypeHero.m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (prototypeHero.m_timeSinceAttack > 1.0f)
                prototypeHero.m_currentAttack = 1;

            // Call one of the two attack animations "Attack1" or "Attack2"
            prototypeHero.m_animator.SetTrigger("Attack" + prototypeHero.m_currentAttack);

            // Disable movement 
            prototypeHero.m_disableMovementTimer = 0.35f;

        }
        else
        {
            //if (upPressed)
            //{
            //    Debug.Log("Air attack up");
            //    prototypeHero.m_animator.SetTrigger("AirAttackUp");

            //    // Reset timer
            //    prototypeHero.m_timeSinceAttack = 0.0f;
            //}
            //else if (downPressed)
            //{
            //    prototypeHero.m_animator.SetTrigger("AttackAirSlam");
            //    prototypeHero.m_body2d.velocity = new Vector2(0.0f, -prototypeHero.m_jumpForce);
            //    prototypeHero.m_disableMovementTimer = 0.8f;

            //    //Reset timer
            //    prototypeHero.m_timeSinceAttack = 0.0f;
            //}
            //else
            //{
                
            //}

            prototypeHero.m_animator.SetTrigger("AirAttack");

            //Reset timer
            prototypeHero.m_timeSinceAttack = 0.0f;
        }
    }
}
