using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInputHandler : MonoBehaviour
{
    public float sensitivity = 100; 

    private MouseInput    m_mouseData;
    
    private KeyboardInput m_keyboardData;
    
    private AllInput      m_entireInputData;


    #region get input
    public Vector2 GetMoveDirection()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)){
            dir.y = 1;
        } else if (Input.GetKey(KeyCode.S))
        {
            dir.y = -1;
        }

        if (Input.GetKey(KeyCode.D)){
            dir.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dir.x = -1;
        }

        return dir.normalized;
    }

    public Vector2 GetMouseDeltaRotation(){
        Vector2 mouseRotationDelta =Vector2.zero;
        float yRotation = Input.GetAxis("Mouse X");
        float xRotation = Input.GetAxis("Mouse Y");

        mouseRotationDelta.x = xRotation * sensitivity ;
        mouseRotationDelta.y = yRotation * sensitivity;

        return mouseRotationDelta;
    }

    public bool GetSpaceHeld()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public bool GetSpacePressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool GetShiftPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }    
    
    public bool GetCtrlPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }    
    
    public bool GetCtrlReleased()
    {
        return Input.GetKeyUp(KeyCode.LeftControl);
    }

    public bool GetCtrlHeld()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }

    public bool GetLMBPressed(){
        return Input.GetMouseButtonDown(0);
    }

    public bool GetLMBReleased(){
        return Input.GetMouseButtonUp(0);
    }

    public bool GetLMBHeld(){
        return Input.GetMouseButton(0);
    }

    public bool GetRMBPressed()
    {
        return Input.GetMouseButtonDown(1);
    }

    public bool GetRMBReleased()
    {
        return Input.GetMouseButtonUp(1);
    }

    public bool GetRMBHeld()
    {
        return Input.GetMouseButton(1);
    }

    public bool GetQPress(){
        return Input.GetKeyDown(KeyCode.Q);
    }

    #endregion


    private void Update()
    {
        PopulateMouseData();
        PopulateKeyboardData();
        PopulateAllInputData();
    }
    
    private void PopulateMouseData(){
        m_mouseData._lmbPressed  = GetLMBPressed();
        m_mouseData._lmbHeld     = GetLMBHeld();
        m_mouseData._lmbReleased = GetLMBReleased();
        m_mouseData._rmbPressed  = GetRMBPressed();
        m_mouseData._rmbHeld     = GetRMBHeld();
        m_mouseData._rmbReleased = GetRMBReleased();
    }

    private void PopulateKeyboardData(){
       m_keyboardData._spaceHeld      = GetSpaceHeld();
       m_keyboardData._spacePressed   = GetSpacePressed();
       m_keyboardData._shiftPressed   = GetShiftPressed();
       m_keyboardData._ctrlPressed    = GetCtrlPressed();
       m_keyboardData._ctrlReleased   = GetCtrlReleased();
       m_keyboardData._ctrlHeld       = GetCtrlHeld();
       m_keyboardData._moveDirection  = GetMoveDirection();
    }

    private void PopulateAllInputData(){
        m_entireInputData._mouse    = m_mouseData;
        m_entireInputData._keyboard = m_keyboardData;
    }

    public MouseInput GetMouseInput(){
        return m_mouseData;
    }

    public KeyboardInput GetKeyboardInput(){
        return m_keyboardData;
    }

    public AllInput GetAllInput(){
        return m_entireInputData;
    }

    public struct AllInput{
        internal MouseInput    _mouse;
        internal KeyboardInput _keyboard;

        public MouseInput    Mouse    => _mouse;
        public KeyboardInput Keyboard => _keyboard;
    }

    public struct MouseInput{
        internal bool _lmbPressed;
        internal bool _lmbHeld;
        internal bool _lmbReleased;
        internal bool _rmbHeld;
        internal bool _rmbPressed;
        internal bool _rmbReleased;

        public bool LmbPressed  => _lmbPressed;
        public bool LmbHeld     => _lmbHeld;
        public bool LmbReleased => _lmbReleased;
        public bool RmbPressed  => _rmbPressed;
        public bool RmbHeld     => _rmbHeld;
        public bool RmbReleased => _rmbReleased;

    }

    public struct KeyboardInput{
        internal bool    _spaceHeld;
        internal bool    _spacePressed;
        internal bool    _spaceReleased;
        internal bool    _shiftPressed;
        internal bool    _ctrlHeld;
        internal bool    _ctrlPressed;
        internal bool    _ctrlReleased;
        internal Vector2 _moveDirection;

        public bool    SpaceHeld     => _spaceHeld;
        public bool    SpacePressed  => _spacePressed;
        public bool    ShiftPressed  => _shiftPressed;
        public bool    CtrlPressed   => _ctrlPressed;
        public bool    CtrlReleased  => _ctrlReleased;
        public bool    CtrlHeld      => _ctrlHeld;
        public Vector2 MoveDirection => _moveDirection;
    }
}
