using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovment : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    public Transform cam;
    public float speed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;  // Gravity acceleration
    public float groundDistance = 0.4f;  // Distance from the ground to check
    public LayerMask groundMask;  // Layer mask to define what is considered ground
    public Transform groundCheck;  // Position to check if the player is grounded
    [SerializeField] private Animator anim;  // Animator component
    public List <Animal> animalList;

    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        anim = GetComponent<Animator>();  // Ensure the animator component is assigned
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Apply gravity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Small downward force to ensure the controller stays grounded
        }

        // Input from keyboard or gamepad
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Check if the player is moving
        bool isMoving = direction.magnitude >= 0.1f;

        // Set animation parameters
        anim.SetBool("isWalking", isMoving);

        // Move the character
        if (isMoving)
        {
            // Calculate the angle to rotate towards
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotate the player
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move the player
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // Always apply gravity if not grounded
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Animal>())
        {
            animalList.Add(other.GetComponent<Animal>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Animal>())
        {
            animalList.Remove(other.GetComponent<Animal>());
        }
    }
   
}
