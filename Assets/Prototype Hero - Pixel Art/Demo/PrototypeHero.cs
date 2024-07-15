using UnityEngine;
using System.Collections;
using System;

public class PrototypeHero : Actor {

    public float StickDeadZone;

    public float      m_runSpeed = 4.5f;
    public float      m_walkSpeed = 2.0f;
    public float      m_jumpForce = 7.5f;
    public float      m_dodgeForce = 8.0f;
    public float      m_parryKnockbackForce = 4.0f; 
    public bool       m_noBlood = false;
    public bool       m_hideSword = false;

    public IAction BaseAction;

    public Animator            m_animator;
    public Rigidbody2D         m_body2d;
    private SpriteRenderer      m_SR;
    private Sensor_Prototype    m_groundSensor;
    private Sensor_Prototype    m_wallSensorR1;
    private Sensor_Prototype    m_wallSensorR2;
    private Sensor_Prototype    m_wallSensorL1;
    private Sensor_Prototype    m_wallSensorL2;
    public bool                m_grounded = false;
    private bool                m_moving = false;
    private bool                m_dead = false;
    private bool                m_dodging = false;
    private bool                m_wallSlide = false;
    private bool                m_ledgeGrab = false;
    private bool                m_ledgeClimb = false;
    private bool                m_crouching = false;
    private Vector3             m_climbPosition;
    private int                 m_facingDirection = 1;
    public float               m_disableMovementTimer = 0.0f;
    private float               m_parryTimer = 0.0f;
    private float               m_respawnTimer = 0.0f;
    private Vector3             m_respawnPosition = Vector3.zero;
    public int                 m_currentAttack = 0;
    public float               m_timeSinceAttack = 0.0f;
    private float               m_gravity;
    public float                m_maxSpeed = 4.5f;

    bool _hasSlid = false;

    // Use this for initialization
    public void Initilize()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
        m_gravity = m_body2d.gravityScale;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

        GameManager.Instance.InputManger.EnablePlayerControls();
    }

    // Update is called once per frame
    void Update ()
    {
        // Decrease death respawn timer 
        m_respawnTimer -= Time.deltaTime;

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Decrease timer that checks if we are in parry stance
        m_parryTimer -= Time.deltaTime;

        // Decrease timer that disables input movement. Used when attacking
        m_disableMovementTimer -= Time.deltaTime;

        // Respawn Hero if dead
        if (m_dead && m_respawnTimer < 0.0f)
            RespawnHero();

        if (m_dead)
            return;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = 0.0f;

        // GetAxisRaw returns either -1, 0 or 1
        float inputRaw = GameManager.Instance.InputManger.ActionMap.Player.Move.ReadValue<Vector2>().x;

        bool upPressed = GameManager.Instance.InputManger.ActionMap.Player.Move.ReadValue<Vector2>().y > 0.5f;
        bool downPressed = GameManager.Instance.InputManger.ActionMap.Player.Move.ReadValue<Vector2>().y < -0.5f;

        if (m_disableMovementTimer < 0.0f)
            inputX = inputRaw;

        // Check if character is currently moving
        if (Mathf.Abs(inputRaw) > StickDeadZone && Mathf.Sign(inputRaw) == m_facingDirection)
            m_moving = true;
        else
            m_moving = false;

        // Swap direction of sprite depending on move direction
        if (inputRaw > 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb)
        {
            m_SR.flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputRaw < 0 && !m_dodging && !m_wallSlide && !m_ledgeGrab && !m_ledgeClimb)
        {
            m_SR.flipX = true;
            m_facingDirection = -1;
        }
     
        // SlowDownSpeed helps decelerate the characters when stopping
        float SlowDownSpeed = m_moving ? 1.0f : 0.5f;
        // Set movement
        if(!m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_parryTimer < 0.0f)
            m_body2d.velocity = new Vector2(inputX * m_maxSpeed * SlowDownSpeed, m_body2d.velocity.y);

        // Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // Set Animation layer for hiding sword
        int boolInt = m_hideSword ? 1 : 0;
        m_animator.SetLayerWeight(1, boolInt);

        // Check if all sensors are setup properly
        if (m_wallSensorR1 && m_wallSensorR2 && m_wallSensorL1 && m_wallSensorL2)
        {
            bool prevWallSlide = m_wallSlide;
            //Wall Slide
            // True if either both right sensors are colliding and character is facing right
            // OR if both left sensors are colliding and character is facing left
            m_wallSlide = (m_wallSensorR1.State() && m_wallSensorR2.State() && m_facingDirection == 1) || (m_wallSensorL1.State() && m_wallSensorL2.State() && m_facingDirection == -1);
            if (m_grounded)
                m_wallSlide = false;
            m_animator.SetBool("WallSlide", m_wallSlide);
            //Play wall slide sound
            if(prevWallSlide && !m_wallSlide)
                AudioManager_PrototypeHero.instance.StopSound("WallSlide");


            //Grab Ledge
            // True if either bottom right sensor is colliding and top right sensor is not colliding 
            // OR if bottom left sensor is colliding and top left sensor is not colliding 
            bool shouldGrab = !m_ledgeClimb && !m_ledgeGrab && ((m_wallSensorR1.State() && !m_wallSensorR2.State()) || (m_wallSensorL1.State() && !m_wallSensorL2.State()));
            if(shouldGrab)
            {
                Vector3 rayStart;
                if (m_facingDirection == 1)
                    rayStart = m_wallSensorR2.transform.position + new Vector3(0.2f, 0.0f, 0.0f);
                else
                    rayStart = m_wallSensorL2.transform.position - new Vector3(0.2f, 0.0f, 0.0f);

                var hit = Physics2D.Raycast(rayStart, Vector2.down, 1.0f);

                GrabableLedge ledge = null;
                if(hit)
                    ledge = hit.transform.GetComponent<GrabableLedge>();

                if (ledge)
                {
                    m_ledgeGrab = true;
                    m_body2d.velocity = Vector2.zero;
                    m_body2d.gravityScale = 0;
                    
                    m_climbPosition = ledge.transform.position + new Vector3(ledge.topClimbPosition.x, ledge.topClimbPosition.y, 0);
                    if (m_facingDirection == 1)
                        transform.position = ledge.transform.position + new Vector3(ledge.leftGrabPosition.x, ledge.leftGrabPosition.y, 0);
                    else
                        transform.position = ledge.transform.position + new Vector3(ledge.rightGrabPosition.x, ledge.rightGrabPosition.y, 0);
                }
                m_animator.SetBool("LedgeGrab", m_ledgeGrab);
            }
            
        }


        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e") && !m_dodging)
        {
            Die();
        }
        
        //Hurt
        else if (Input.GetKeyDown("q") && !m_dodging)
        {
            Damage(1);
            
        }

        // Parry & parry stance
        else if (Input.GetMouseButtonDown(1) && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_grounded)
        {
            // Parry
            // Used when you are in parry stance and something hits you
            if (m_parryTimer > 0.0f)
            {
                m_animator.SetTrigger("Parry");
                m_body2d.velocity = new Vector2(-m_facingDirection * m_parryKnockbackForce, m_body2d.velocity.y);
            }
                
            // Parry Stance
            // Ready to parry in case something hits you
            else
            {
                m_animator.SetTrigger("ParryStance");
                m_parryTimer = 7.0f / 12.0f;
            }
        }

        //Attack
        else if ( BaseAction != null && BaseAction.CanPerformAction() && GameManager.Instance.InputManger.ActionMap.Player.BaseAttack.WasPressedThisFrame() &&
                 !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_timeSinceAttack > 0.2f)
        {
            BaseAction.PerformAction(this);
        }

        // Slide
        else if ((GameManager.Instance.InputManger.ActionMap.Player.Slide.WasPressedThisFrame() || (downPressed && m_moving && !_hasSlid)) && 
                 m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb)
        {
            _hasSlid = true;
            m_dodging = true;
            m_crouching = false;
            m_animator.SetBool("Crouching", false);
            m_animator.SetTrigger("Dodge");
            m_body2d.velocity = new Vector2(m_facingDirection * m_dodgeForce, m_body2d.velocity.y);
        }

        // Throw
        else if(Input.GetKeyDown("f") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb)
        {
            m_animator.SetTrigger("Throw");

            // Disable movement 
            m_disableMovementTimer = 0.20f;
        }

        // Ledge Climb
        else if(upPressed && m_ledgeGrab)
        {
            DisableWallSensors();
            m_ledgeClimb = true;
            m_body2d.gravityScale = 0;
            m_disableMovementTimer = 6.0f/14.0f;
            m_animator.SetTrigger("LedgeClimb");
        }

        // Ledge Drop
        else if (downPressed && m_ledgeGrab)
        {
            DisableWallSensors();
        }

        //Jump
        else if (GameManager.Instance.InputManger.ActionMap.Player.Jump.WasPressedThisFrame() && (m_grounded || m_wallSlide) && 
                 !m_dodging && !m_ledgeGrab && !m_ledgeClimb && !m_crouching && m_disableMovementTimer < 0.0f)
        {
            // Check if it's a normal jump or a wall jump
            if(!m_wallSlide)
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            else
            {
                m_body2d.velocity = new Vector2(-m_facingDirection * m_jumpForce / 2.0f, m_jumpForce);
                m_facingDirection = -m_facingDirection;
                m_SR.flipX = !m_SR.flipX;
            }

            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_groundSensor.Disable(0.2f);
        }

        //Crouch / Stand up
        else if (downPressed && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_parryTimer < 0.0f)
        {
            m_crouching = true;
            m_animator.SetBool("Crouching", true);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x / 2.0f, m_body2d.velocity.y);
        }
        else if (m_crouching)
        {
            m_crouching = false;
            m_animator.SetBool("Crouching", false);
        }
        //Walk
        else if (m_moving && MathF.Abs(inputX) < 0.5f)
        {
            m_animator.SetInteger("AnimState", 2);
            m_maxSpeed = m_walkSpeed;
        }

        //Run
        else if(m_moving)
        {
            m_animator.SetInteger("AnimState", 1);
            m_maxSpeed = m_runSpeed;
        }

        //Idle
        else
        {
            m_animator.SetInteger("AnimState", 0);
        }


        if (_hasSlid && !downPressed)
            _hasSlid = false;
    }

    // Function used to spawn a dust effect
    // All dust effects spawns on the floor
    // dustXoffset controls how far from the player the effects spawns.
    // Default dustXoffset is zero
    public void SpawnDustEffect(GameObject dust, float dustXOffset = 0, float dustYOffset = 0)
    {
        if (dust != null)
        {
            // Set dust spawn position
            Vector3 dustSpawnPosition = transform.position + new Vector3(dustXOffset * m_facingDirection, dustYOffset, 0.0f);
            GameObject newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity) as GameObject;
            // Turn dust in correct X direction
            newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(m_facingDirection, 1, 1);
        }
    }

    void DisableWallSensors()
    {
        m_ledgeGrab = false;
        m_wallSlide = false;
        m_ledgeClimb = false;
        m_wallSensorR1.Disable(0.8f);
        m_wallSensorR2.Disable(0.8f);
        m_wallSensorL1.Disable(0.8f);
        m_wallSensorL2.Disable(0.8f);
        m_body2d.gravityScale = m_gravity;
        m_animator.SetBool("WallSlide", m_wallSlide);
        m_animator.SetBool("LedgeGrab", m_ledgeGrab);
    }

    // Called in AE_resetDodge in PrototypeHeroAnimEvents
    public void ResetDodging()
    {
        m_dodging = false;
    }

    public void SetPositionToClimbPosition()
    {
        transform.position = m_climbPosition;
        m_body2d.gravityScale = m_gravity;
        m_wallSensorR1.Disable(3.0f / 14.0f);
        m_wallSensorR2.Disable(3.0f / 14.0f);
        m_wallSensorL1.Disable(3.0f / 14.0f);
        m_wallSensorL2.Disable(3.0f / 14.0f);
        m_ledgeGrab = false;
        m_ledgeClimb = false;
    }

    public bool IsWallSliding()
    {
        return m_wallSlide;
    }

    public void DisableMovement(float time = 0.0f)
    {
        m_disableMovementTimer = time;
    }

    void RespawnHero()
    {
        transform.position = Vector3.zero;
        m_dead = false;
        m_animator.Rebind();
    }

    public override void Damage(float attack)
    {
        m_animator.SetTrigger("Hurt");
        // Disable movement 
        m_disableMovementTimer = 0.1f;
        DisableWallSensors();

        base.Damage(attack);

    }

    public override void Die()
    {
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");
        m_respawnTimer = 2.5f;
        DisableWallSensors();
        m_dead = true;
    }
}
