using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] float _horizontalInput;
    [SerializeField] float _verticalInput;
    [SerializeField] bool _jumpInput;
    [SerializeField] bool _climbInput;
    [SerializeField] protected Vector3 acceleration;
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;
    public bool JumpInput => _jumpInput;
    public float HorizontalInput => _horizontalInput;
    public float VerticalInput => _verticalInput;
    public bool ClimbInput => _climbInput;

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

    public void ReceiveMouseMovement(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            Vector2 value = callback.ReadValue<Vector2>();
            mouseX = value.x;
            mouseY = value.y;
        }
    }
    #endregion
    
}
