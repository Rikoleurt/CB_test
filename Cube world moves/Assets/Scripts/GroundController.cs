using System.Collections;
using UnityEngine;


public class GroundController : Controller
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float gravity;
    [SerializeField] private float groundFriction; // Between 0 and 1
    [SerializeField] private float airFriction; // Between 0 and 1
    [SerializeField] private float stopThreshold;
    [SerializeField] private bool isGrounded;


    [Header("MY INPUT")] 
    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;
    [SerializeField] bool jumpInput;
    [SerializeField] bool climbInput;

    float angle;
    private bool canDash = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkGround();
        HandleMouseMovement();
        HandleGroundMovement();
    }

    void HandleGroundMovement()
    {
        acceleration += verticalInput * moveSpeed * transform.forward + horizontalInput * moveSpeed * transform.right;

        acceleration.z = Mathf.Clamp(acceleration.z, -maxSpeed, maxSpeed);
        acceleration.x = Mathf.Clamp(acceleration.x, -maxSpeed, maxSpeed);

        // Handle x and z movements (with friction)
        if (isGrounded)
        {
            if (acceleration.x != 0) acceleration.x += groundFriction * -acceleration.x;
            if (acceleration.z != 0) acceleration.z += groundFriction * -acceleration.z;

            // Passing a certain threshold, we stop the player
            if (Mathf.Abs((acceleration.x + acceleration.z) / 2) < stopThreshold) acceleration = Vector3.zero;

            acceleration.y = 0;
            if (jumpInput) acceleration.y = jumpForce;
        }
        else
        {
            acceleration.y -= gravity;
        }
        
        transform.position += acceleration * Time.fixedDeltaTime;
    }


    void HandleMouseMovement()
    {
        Vector3 mousePosOnScreen = Input.mousePositionDelta;

        angle += mousePosOnScreen.x;
        angle %= 360;
        transform.eulerAngles = new Vector3(0, angle, 0);
    }

    IEnumerator DashCooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canDash = true;
    }

    void checkGround()
    {
        RaycastHit hit = new();
        Physics.Raycast(transform.position, Vector3.down, out hit, 0.55f);
        if (hit.collider && hit.collider.CompareTag("Ground")) isGrounded = true;
        else isGrounded = false;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.55f);
    }
}