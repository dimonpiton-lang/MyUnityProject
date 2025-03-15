using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    public Rigidbody rb;
    private Vector3 moveDirection;
    private float currentDashTime;
    private float lastDashTime;
    private bool isDashing;
    private bool isGrounded;
    public Transform groundCheck;
    public Transform cameraTransform;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundCheck = transform.Find("GroundCheck");

        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck transform not found. Please create an empty GameObject called 'GroundCheck' as a child of the Player.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the Player.");
        }
        cameraTransform = Camera.main.transform;

        rb.freezeRotation = true;
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);

        // Read input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && (Time.time - lastDashTime >= dashCooldown) && moveDirection != Vector3.zero)
        {
            StartDash();
        }

        if (isDashing)
        {
            currentDashTime += Time.deltaTime;
            if (currentDashTime >= dashDuration)
            {
                isDashing = false;
                lastDashTime = Time.time;
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            PerformDash();
        }
        else
        {
            Move();
        }
        Rotate();
    }


    void Move()
    {
        if (moveDirection != Vector3.zero)
        {
            Vector3 cameraForward = GetCameraForwardDirection();
            Vector3 moveVector = (cameraForward * moveDirection.z + cameraTransform.right * moveDirection.x) * moveSpeed;
            rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(GetCameraForwardDirection());
        transform.rotation = targetRotation;
    }

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        return cameraForward.normalized;
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void StartDash()
    {
        isDashing = true;
        currentDashTime = 0f;
    }

    void PerformDash()
    {
        Vector3 cameraForward = GetCameraForwardDirection();
        Vector3 dashVector = (cameraForward * moveDirection.z + cameraTransform.right * moveDirection.x) * dashSpeed;
        rb.velocity = new Vector3(dashVector.x, rb.velocity.y, dashVector.z);
    }
}