using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public static Controller Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(this);
    }
    void OnApplicationQuit()
    {
        Instance = null;
    }

    [SerializeField] float _horizontalInput;
    [SerializeField] float _verticalInput;
    [SerializeField] bool _jumpInput;
    [SerializeField] bool _climbInput;
    [SerializeField] bool _wallRunInput;
    [SerializeField] protected Vector3 acceleration;
    [SerializeField] private Vector2 _deltaLook = new();
    
    private float angle;
    public bool JumpInput => _jumpInput;
    public float HorizontalInput => _horizontalInput;
    public float VerticalInput => _verticalInput;
    public bool ClimbInput => _climbInput;

    public Vector2 DeltaLookRaw => _deltaLook;
    public Vector2 DeltaLook => _deltaLook/100;

    public bool WallRunInput => _wallRunInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    #region Movement
    public void ReceiveMoveInput(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            Vector2 value = callback.ReadValue<Vector2>();
            _horizontalInput = value.x;
            _verticalInput = value.y;
        }

        if (callback.canceled)
        {
            _horizontalInput = 0;
            _verticalInput = 0;
        }
    }

    public void ReceiveJumpInput(InputAction.CallbackContext callback)
    {
        if(callback.performed) _jumpInput = true;
        if(callback.canceled) _jumpInput = false;
    }

    public void ReceiveDashInput(InputAction.CallbackContext callback)
    {
        
    }
    
    public void ReceiveClimbInput(InputAction.CallbackContext callback)
    {
        if (callback.performed) _climbInput = true;
        if (callback.canceled) _climbInput = false;
    }

    public void ReceiveWallRunInput(InputAction.CallbackContext callback)
    {
        if (callback.performed) _wallRunInput = true;
        if (callback.canceled) _wallRunInput = false;
    }
    public void ReceiveMouseMovement(InputAction.CallbackContext callback)
    {
        if (callback.performed) _deltaLook = callback.ReadValue<Vector2>();
        if(callback.canceled) _deltaLook = Vector2.zero;
    }
    
    
    #endregion
}
