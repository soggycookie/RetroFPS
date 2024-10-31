
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInputHandler), typeof(Player))]
public class PlayerMovementAbility : MonoBehaviour
{
    public enum MovementState
    {
        RUNNING,
        AIRING,
        SLAMMING,
        SLIDING,
        DASHING,
    };

    struct PlayerInput
    {
        public bool    isSpaceHeld;
        public bool    isSpacePressed;
        public bool    isShiftPressed;
        public bool    isCtrlPressed;
        public bool    isCtrlReleased;
        public bool    isCtrlHeld;
        public Vector2 moveDirection;
    };


    [SerializeField]
    private PlayerMovementSO m_playerMovementSO;
    

    [Space(10)]
    [SerializeField]
    private Transform   head;

    [SerializeField]
    private Transform[] groundCheckTrs;

    [Space(10)]
    [SerializeField]
    private float       updateSpeedInterval  = 1f;



    public UnityAction                                 OnDash;
    public UnityAction                                 OnExitDash;
    public UnityAction<float, float>                   OnSpeedUpdate;
    public UnityAction<MovementState>                  OnStateUpdate;
    public UnityAction<Vector3, float, float, float>   OnSlam;
    public UnityAction<RaycastHit>                     OnSlamDirectHit;
    public UnityAction<RaycastHit>                     OnSlamIndirectHit;
    public UnityAction<RaycastHit>                     OnDashHit;

    public float                      CurrentDashEnergy { get; private set; }


    const float c_gravity = -31f;


    private PlayerCameraController   m_fpsCamera;
    private Rigidbody                m_rigidbody;
    private PlayerInputHandler       m_inputHandler;
    private MovementState            m_movementState;
    private PlayerInput              m_currentInput;




    private bool  m_canMove;
    
    //ground
    private bool  m_isOnSlope;
    private bool  m_isGrounded;
    private bool  m_isFalling;

    //air
    private float m_airMuiltipler;

    //slide
    private bool  m_canSlide;
     
    //jump
    private bool  m_readyToJump;
    private bool  m_wasJump;
    private bool  m_canWallJump;
    private bool  m_wasWallJump;
    private bool  m_isJumpTriggered;
    private bool  m_isWallJumpTriggered;
    private float m_groundCheckAfterJumpTime = 0.05f;
    private float m_timeSinceLastJump;
    private int   m_wallJumpTime;

    //wall slide
    private bool  m_isWallCollided;
    private bool  m_isCollidedWithGroundAndWall;
    private bool  m_isWallSlide;

    //dash
    private bool  m_isDashTriggered;
    private bool  m_isDashDecelerated;
    private bool  m_isDashExit;

    private bool  m_isSlamTriggered;
    //bool  m_isSlamExit;


    //update UI
    private float m_lastTimeSinceUpdateUI;


               
    private Vector3    m_lastMoveDirBeforeJump;
    private Vector3    m_desiredMoveDir;
    private Vector3    m_storedVelocity;
    private Vector3    m_slideForward;
    private Vector3    m_dashDir;
    private Vector3    m_wallNormal;


    Coroutine  m_decelerateDash;
    RaycastHit m_slopeHit;

    List<int>  m_collidedLayers = new List<int>();
    Collider   m_collider;

    private void Awake()
    {
        m_collider     = GetComponent<Collider>();


        m_fpsCamera    = Camera.main.GetComponent<PlayerCameraController>();
        m_inputHandler = GetComponent<PlayerInputHandler>();

        m_rigidbody    = GetComponent<Rigidbody>();
        m_rigidbody.freezeRotation = true;


        Physics.gravity  = new Vector3(0, c_gravity, 0);


        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Start()
    {
        CheckGround();

        m_readyToJump     =  m_isGrounded;
        m_movementState   =  m_isGrounded ? MovementState.RUNNING : MovementState.AIRING;
                             
        m_rigidbody.drag  =  m_isGrounded ? m_playerMovementSO.groundDrag : 0;
                             
        CurrentDashEnergy =  m_playerMovementSO.totalDashEnergy;
        
        m_wasJump         = !m_isGrounded;
        m_canMove         =  true;

    }

    void Update()
    {
        CheckGround();
        CheckFreeFall();
        SetMaxMultiplierWhileFall();

        HandleInput();
        GetDesiredMoveAndDashDir();

        ManageState();
        RechargeDash();


        SendUpdateUI();
        SendStateUpdateUI();

        SpeedControl();
    }

    private void FixedUpdate()
    {

        Vector3 camRotation = m_fpsCamera.GetCameraYRotation();
        m_rigidbody.MoveRotation(Quaternion.Euler(camRotation));

        Move();
        ExecuteMechanics();

    }

    void HandleInput()
    {
        m_currentInput.isSpaceHeld    = m_inputHandler.GetSpaceHeld();
        m_currentInput.isSpacePressed = m_inputHandler.GetSpacePressed();
        m_currentInput.isShiftPressed = m_inputHandler.GetShiftPressed();
        m_currentInput.isCtrlPressed  = m_inputHandler.GetCtrlPressed();
        m_currentInput.isCtrlReleased = m_inputHandler.GetCtrlReleased();
        m_currentInput.isCtrlHeld     = m_inputHandler.GetCtrlHeld();
        m_currentInput.moveDirection  = m_inputHandler.GetMoveDirection();
    }

    void ManageState()
    {
        switch (m_movementState)
        {
            case MovementState.SLIDING:
                //HandleDashCondition();
                HandleGroundJumpCondition();
                HandleSlideExitCondition();

                break;

            case MovementState.SLAMMING:
                //if (m_isSlamExit && !m_isSlamTriggered){
                if (!m_isSlamTriggered){
                    m_movementState = MovementState.RUNNING;
                }
                break;

            case MovementState.AIRING:
                if (m_isGrounded && !m_isJumpTriggered)
                    m_movementState = MovementState.RUNNING;

                HandleWallJumpCondition();
                HandleDashCondition();
                HandleSlamCondition();

                break;

            case MovementState.RUNNING:
                if (!m_isGrounded)
                    m_movementState = MovementState.AIRING;

                HandleSlideCondition();
                HandleDashCondition();
                HandleGroundJumpCondition();

                break;
            case MovementState.DASHING:
                HandleSlamCondition();
                CheckDashObstacles();

                if (m_isSlamTriggered)
                    CancelDash();

                HandleDashExitCondition();

                break;
        }

    }

    void ExecuteMechanics()
    {
        if (!m_canMove)
            return;

        switch (m_movementState)
        {
            case MovementState.SLIDING:
                Slide();
                break;

            case MovementState.SLAMMING:
                //if (m_isSlamTriggered)
                    GroundSlam();
                break;

            case MovementState.AIRING:
                if (m_isJumpTriggered){
                    GroundJump();
                }
                if(m_isWallJumpTriggered)
                    WallJump();

                break;

            case MovementState.RUNNING:


                break;
            case MovementState.DASHING:
                if(m_isDashTriggered){
                    Dash();
                }
                break;
        }
    }


    #region Move
    void CheckGround()
    {
        m_isGrounded = false;

        if (Time.time - m_timeSinceLastJump > m_groundCheckAfterJumpTime){
            for (int i = 0; i < groundCheckTrs.Length; i++)
            {
                bool rayCheck = Physics.Raycast(groundCheckTrs[i].position + Vector3.up, Vector3.down, 0.1f + Vector3.up.magnitude, m_playerMovementSO.ground);

                if (rayCheck)
                {
                    m_isGrounded = true;

                    break;
                }
            } 
        }
    }

    void CheckFreeFall(){
        if(!m_isGrounded && !m_wasJump){
            m_isFalling = true;
        }else{
            m_isFalling = false;
        }
    }

    void GetDesiredMoveAndDashDir()
    {
        m_desiredMoveDir = transform.right * m_currentInput.moveDirection.x + transform.forward * m_currentInput.moveDirection.y;
        m_desiredMoveDir = m_desiredMoveDir.normalized;

        if (m_movementState != MovementState.DASHING)
            m_dashDir = m_desiredMoveDir;
    }


    void SetMaxMultiplierWhileFall(){
        if(m_isFalling){
            m_airMuiltipler = m_playerMovementSO.fallingAirMultiplier;
        }else{
            m_airMuiltipler = m_playerMovementSO.maxAirMultiplier;
        }
    }

    void Move()
    {

        if (!m_canMove) return;

        UseGravity();
        SetUpMoveParam();

        switch (m_movementState)
        {
            case MovementState.SLIDING:
                break;

            case MovementState.SLAMMING:

                break;

            case MovementState.AIRING:
                //if(m_wasJump)
                    MoveWhileAir();
                break;

            case MovementState.RUNNING:

                if (m_isOnSlope)
                {
                    MoveWhileOnSlope();
                }
                else
                    MoveWhileWalk();

                break;
            case MovementState.DASHING:
                break;
        }
    }

    void SetUpMoveParam()
    {
        switch (m_movementState)
        {
            case MovementState.SLIDING:
                m_rigidbody.drag = m_playerMovementSO.groundDrag;
                break;

            case MovementState.SLAMMING:
                m_rigidbody.drag = 0;

                break;

            case MovementState.AIRING:
                m_rigidbody.drag = 0;
                break;

            case MovementState.RUNNING:
                m_rigidbody.drag = m_playerMovementSO.groundDrag;

                m_wasJump        = false;
                m_wallJumpTime   = 0;
                m_wasWallJump    = false;
                m_readyToJump    = true;

                m_isOnSlope      = CheckOnSlope();

                break;
            case MovementState.DASHING:

                if (!m_isDashDecelerated)
                    m_rigidbody.drag = 0;
                else
                    m_rigidbody.drag = m_playerMovementSO.groundDrag * 2;

                break;
        }
    }

    void UseGravity()
    {
        switch (m_movementState)
        {
            case MovementState.SLIDING:
                m_rigidbody.useGravity = true; 
                
                break;

            case MovementState.SLAMMING:
                m_rigidbody.useGravity = true;
                
                break;

            case MovementState.AIRING:
                if(m_isWallSlide)
                    m_rigidbody.useGravity = false;
                else
                    m_rigidbody.useGravity = true;
                
                break;

            case MovementState.RUNNING:
                m_rigidbody.useGravity = !m_isOnSlope; 
                
                break;

            case MovementState.DASHING:
                m_rigidbody.useGravity = false; 
                
                break;

        }

    }

    void MoveWhileAir()
    {
        Vector3 moveDir = m_desiredMoveDir;

        if(m_isWallCollided){
            moveDir = Vector3.ProjectOnPlane(m_desiredMoveDir, m_wallNormal);
        }

        m_rigidbody.AddForce( moveDir * m_airMuiltipler * m_playerMovementSO.moveSpeed, ForceMode.Force);
    }

    void MoveWhileWalk()
    {
        m_rigidbody.AddForce(m_desiredMoveDir * m_playerMovementSO.moveSpeed, ForceMode.Force);
    }

    void MoveWhileOnSlope()
    {
        ModifyMoveDirOnSlope();

        if (m_desiredMoveDir.y < 0)
        {
            m_rigidbody.AddForce(Vector3.down * m_playerMovementSO.slopeDownwardForce, ForceMode.Impulse);
            m_rigidbody.AddForce(m_desiredMoveDir * m_playerMovementSO.moveSpeed * 2, ForceMode.Force);
        }
        else
            m_rigidbody.AddForce(m_desiredMoveDir * m_playerMovementSO.moveSpeed, ForceMode.Force);
    }

    bool CheckOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out m_slopeHit, 0.3f, m_playerMovementSO.ground))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, m_slopeHit.normal);

            return slopeAngle <= m_playerMovementSO.maxSlopeDegAngle && slopeAngle != 0;
        }
        return false;
    }

    void ModifyMoveDirOnSlope()
    {
        Vector3 slopeForward = GetSlopeForward();
        m_desiredMoveDir = (transform.right * m_currentInput.moveDirection.x + slopeForward * m_currentInput.moveDirection.y).normalized;
    }

    Vector3 GetSlopeForward()
    {
        return Vector3.Cross(transform.right, m_slopeHit.normal);
    }

    #endregion


    #region Jump
    void HandleGroundJumpCondition()
    {
        if (m_isGrounded && m_readyToJump && m_inputHandler.GetSpacePressed())
        {
            m_movementState   = MovementState.AIRING;
            m_isJumpTriggered = true;
        }
        else
        {
            m_isJumpTriggered = false;
        }
    }

    void GroundJump()
    {
        Jump(Vector3.up, m_playerMovementSO.jumpForce);

        m_isJumpTriggered = false;
    }

    void Jump(Vector3 jumpDir, float force)
    {

        m_readyToJump = false;
        m_timeSinceLastJump = Time.time;
        m_lastMoveDirBeforeJump = m_desiredMoveDir;
        m_wasJump = true;

        if (m_wasWallJump)
        {
            m_rigidbody.velocity = Vector3.zero;
        }

        m_rigidbody.AddForce(jumpDir * force, ForceMode.Impulse);
        StartCoroutine(ResetJumpCoolDown());

    }

    void HandleWallJumpCondition()
    {
        if (m_wallJumpTime >= m_playerMovementSO.maxWallJumpTime)
            return;

        if (!m_canWallJump)
        {
            return;
        }

        if (!m_readyToJump)
            return;


        if (!m_inputHandler.GetSpacePressed())
        {
            return;
        }

        m_isWallJumpTriggered = true; ;
    }

    void WallJump()
    {
        Vector3 jumpDir = (Vector3.up * 2 + m_wallNormal).normalized;

        m_wasWallJump = true;
        m_canWallJump = false;
        m_wallJumpTime++;

        Jump(jumpDir, m_playerMovementSO.wallJumpForce);

        m_isWallJumpTriggered = false;
    }

    IEnumerator ResetJumpCoolDown()
    {
        yield return new WaitForSeconds(m_playerMovementSO.jumpCoolDown);
        m_readyToJump = true;
    }


    #endregion


    #region Slam
    void HandleSlamCondition()
    {
        if(m_isGrounded)
            return;

        if (!m_inputHandler.GetCtrlPressed())
        {
            return;
        }

        m_movementState   = MovementState.SLAMMING;
        m_isSlamTriggered = true;
    }

    void GroundSlam()
    {
        m_rigidbody.AddForce(Vector3.down * m_playerMovementSO.groundSlamForce, ForceMode.Force);
        RaycastHit[] hits = HandleSlamDirectHit();

        foreach(RaycastHit hit in hits) {
            OnSlamDirectHit?.Invoke(hit);
        }
        //m_isSlamExit = false;
    }

    void HandleSlamCollisonEnter()
    {
        OnSlam?.Invoke(m_playerMovementSO.pivotOffset, m_playerMovementSO.amplitude, m_playerMovementSO.frequency, m_playerMovementSO.shakeDuration);

        m_storedVelocity  = m_rigidbody.velocity;
        //m_isSlamExit = true;
        m_isSlamTriggered = false;

        m_rigidbody.velocity = Vector3.zero;
        //StartCoroutine(FreezePlayerMovement(frozenSlamTime));

        RaycastHit[] hits = HandleSlamIndirectHit();

        foreach (RaycastHit hit in hits)
        {
            OnSlamIndirectHit?.Invoke(hit);
        }
    }

    RaycastHit[] HandleSlamDirectHit(){
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * 0.2f, Vector3.down, Quaternion.identity, 2f, m_playerMovementSO.enemy);

        return hits;
    }

    RaycastHit[] HandleSlamIndirectHit()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position + Vector3.up * 0.5f, new Vector3(2.5f, 0.5f, 2.5f), Vector3.down, Quaternion.identity, 0.1f, m_playerMovementSO.enemy);

        return hits;
    }

    #endregion


    #region Speed Control
    void SpeedControl()
    {

        switch (m_movementState)
        {

            case MovementState.RUNNING:
                if (m_isOnSlope)
                {
                    if (m_rigidbody.velocity.magnitude > m_playerMovementSO.maxWalkingVelocity)
                    {
                        m_rigidbody.velocity = m_rigidbody.velocity.normalized * m_playerMovementSO.maxWalkingVelocity;
                    }

                }
                else
                {

                    Vector3 walkingVelocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
                    if (walkingVelocity.magnitude > m_playerMovementSO.maxWalkingVelocity)
                    {
                        m_rigidbody.velocity = walkingVelocity.normalized * m_playerMovementSO.maxWalkingVelocity;
                    }
                }

                break;

            case MovementState.SLIDING:
                if(m_rigidbody.velocity.sqrMagnitude > m_playerMovementSO.maxSlidingVelocity * m_playerMovementSO.maxSlidingVelocity)
                    m_rigidbody.velocity = m_rigidbody.velocity.normalized * m_playerMovementSO.maxSlidingVelocity;

                break;

            case MovementState.DASHING:

                if ((m_rigidbody.velocity.sqrMagnitude > m_playerMovementSO.maxDashVelocity * m_playerMovementSO.maxDashVelocity))
                {
                    m_rigidbody.velocity = m_rigidbody.velocity.normalized * m_playerMovementSO.maxDashVelocity;
                }

                break;

            case MovementState.AIRING:
                LimitVelocityWhileJump();
                LimitVelocityWhileAir();

                break;

            default:
                break;
        }
    }

    void LimitVelocityWhileJump(){
        if(!m_wasJump)
            return;

        if (new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z).sqrMagnitude > m_playerMovementSO.maxJumpVelocity * m_playerMovementSO.maxJumpVelocity)
        {
            m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z).normalized * m_playerMovementSO.maxJumpVelocity + (Vector3.up * m_rigidbody.velocity.y);
        }
    }

    void LimitVelocityWhileAir(){
        if (new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z).sqrMagnitude > m_playerMovementSO.maxAirVelocity * m_playerMovementSO.maxAirVelocity)
        {
            m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z).normalized * m_playerMovementSO.maxAirVelocity + (Vector3.up * m_rigidbody.velocity.y);
        }
    }

    #endregion


    #region Slide

    void GetSlideDir()
    {
        m_rigidbody.velocity = Vector3.zero;
        m_slideForward = transform.forward;
    }

    void Slide()
    {
        m_rigidbody.AddForce(m_slideForward * m_playerMovementSO.slideForce, ForceMode.Force);
    }

    void HandleSlideCondition()
    {

        if (m_wasJump)
            return;

        if (m_inputHandler.GetCtrlPressed())
        {
            GetSlideDir();
            m_canSlide = true;
        }

        if (!m_inputHandler.GetCtrlHeld())
            return;

        if (!m_canSlide)
            return;


        m_movementState = MovementState.SLIDING;
        ChangePlayerHeight(new Vector3(1, 0.5f, 1));

    }

    void HandleSlideExitCondition()
    {
        if (m_inputHandler.GetCtrlReleased() || m_isJumpTriggered)
        {
            if(m_isJumpTriggered)
                m_movementState = MovementState.AIRING;
            else
                m_movementState = m_isGrounded ? MovementState.RUNNING : MovementState.AIRING;
            
            m_canSlide = false;
            
            ChangePlayerHeight(new Vector3(1, 1, 1));
        }
    }

    void ChangePlayerHeight(Vector3 scale)
    {
        transform.localScale = scale;
    }

    #endregion


    #region Dash
    void HandleDashCondition(){
        if (!m_inputHandler.GetShiftPressed())
            return;

        if(CurrentDashEnergy < (m_playerMovementSO.totalDashEnergy / (float) m_playerMovementSO.maxDashCharge))
        {
            return;
        }

        m_movementState   = MovementState.DASHING;
        m_isDashTriggered = true;
    }

    void Dash()
    {
        if (m_dashDir == Vector3.zero)
            m_dashDir = transform.forward;

        m_isDashDecelerated  = false;
        m_isDashExit         = false;
        m_isDashTriggered    = false;
        CurrentDashEnergy   -= m_playerMovementSO.totalDashEnergy / (float)m_playerMovementSO.maxDashCharge;

        CheckDashObstacles();
        m_rigidbody.AddForce(m_dashDir * m_playerMovementSO.dashForce, ForceMode.Impulse);
        m_decelerateDash = StartCoroutine(DecelerateDash(m_playerMovementSO.dashDuration));

        OnDash?.Invoke();
    }

    void CheckDashObstacles(){
        if(m_movementState != MovementState.DASHING)
            return;
        
        RaycastHit[] hits = Physics.BoxCastAll(head.position,Vector3.one * 2f,  m_dashDir, Quaternion.identity, 3.0f, m_playerMovementSO.enemy, QueryTriggerInteraction.Ignore);
        foreach(RaycastHit hit in hits){    
            OnDashHit?.Invoke(hit);
        }
    }

    void HandleDashExitCondition()
    {
        if(!m_isDashExit)
            return;
        
        if(m_isDashTriggered){
            return;
        }

        OnExitDash?.Invoke();
        
        m_collider.isTrigger = false;

        if(m_isSlamTriggered)
            m_movementState = MovementState.SLAMMING;
        else
            m_movementState = m_isGrounded ? MovementState.RUNNING : MovementState.AIRING;


    }

    void CancelDash(){
        if (m_decelerateDash != null)
        {
            StopCoroutine(m_decelerateDash);
            m_decelerateDash = StartCoroutine(DecelerateDash(0));
        }
    }

    void RechargeDash(){
        if(!m_isDashExit)
            return;

        if(CurrentDashEnergy > m_playerMovementSO.totalDashEnergy){
            CurrentDashEnergy = m_playerMovementSO.totalDashEnergy;
            return;
        }

        CurrentDashEnergy += Time.deltaTime * m_playerMovementSO.rechargeEnergySpeed;
    }

    IEnumerator DecelerateDash(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_isDashDecelerated = true;

        while (true)
        {
            if (new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z).magnitude < m_playerMovementSO.maxWalkingVelocity)
            {
                break;
            }
            yield return null;
        }

        m_isDashExit = true;
    }


    #endregion


    #region wall jump
    void GetWallNormalAndCheckWallJump(Collision collision)
    {
        m_wallNormal = collision.GetContact(0).normal;
        if (Vector3.Dot(m_wallNormal, Vector3.up) < Mathf.Cos(m_playerMovementSO.maxSlopeDegAngle / 180 * Mathf.PI))
        {
            m_canWallJump = true;
            m_isWallCollided = true;
        }
    }

    void CheckCollidingWithGroundAndWall(){
        foreach (int i in m_collidedLayers)
        {
            if (m_collidedLayers.Contains(3) && m_collidedLayers.Contains(6))
            {
                m_isCollidedWithGroundAndWall = true;
            }
        }



    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        m_collidedLayers.Add(collision.gameObject.layer);
        CheckCollidingWithGroundAndWall();



        switch (m_movementState){
            case MovementState.SLAMMING:
                if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
                    HandleSlamCollisonEnter();



                break;

            case MovementState.DASHING:
                if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6) 
                    CancelDash();
                
                break;
            default:
                break;
        }

        if (collision.gameObject.layer == 6)
        {
            GetWallNormalAndCheckWallJump(collision);
        }

    }

    void WallSlide(){
        if(!m_isCollidedWithGroundAndWall){
            if (m_rigidbody.velocity.y < 0)
            {
                m_isWallSlide = true;
                m_rigidbody.AddForce(Vector3.down * 2f, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {

        //To handle case which player press slamming button at the exact moment collision enter under air state -> stuck in slam state
        if(m_movementState == MovementState.SLAMMING){
            if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
            {
                HandleSlamCollisonEnter();
            }
        }
    }
 

    private void OnCollisionExit(Collision collision)
    {
        m_collidedLayers.Remove(collision.gameObject.layer);

        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            m_isCollidedWithGroundAndWall = false;
        }


        if (collision.gameObject.layer == 6)
        {
            m_isWallSlide    = false;
            m_canWallJump    = false;
            m_isWallCollided = false;
        }
    }


    void SendUpdateUI()
    {
        if (Time.time - m_lastTimeSinceUpdateUI > updateSpeedInterval)
        {
            //OnSpeedUpdate?.Invoke(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude, rb.velocity.y);
            OnSpeedUpdate?.Invoke(new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z).magnitude, m_rigidbody.velocity.y);
            m_lastTimeSinceUpdateUI = Time.time;
        }
    }

    void SendStateUpdateUI()
    {
        OnStateUpdate?.Invoke(m_movementState);
    }

    public float GetTotalDashEnergy(){
        return m_playerMovementSO.totalDashEnergy;
    }

    public int GetMaxDashCharge(){
        return m_playerMovementSO.maxDashCharge;
    }
}
