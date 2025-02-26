using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public Transform player;               // Player's transform
    public float repelRadius = 5f;         // Distance within which the object starts repelling
    public float attractRadius = 10f;      // Distance within which the object starts attracting
    public float forceStrength = 10f;      // Strength of the force for repelling
    public float attractSpeed = 8f;        // Speed at which the object is attracted to the player
    public AudioSource playerAudio;        // AudioSource component on the player
    public Collider triggerBox;            // Trigger box where the animal will wander
    public float wanderSpeed = 8f;        // Speed at which the object wanders randomly
    public ParticleSystem attractionParticles;  // Particle system for visual effects
    public Animator animator;              // Animator component for the animal

    private Rigidbody rb;                  // Rigidbody of the object to apply forces
    private bool isAttracting = false;     // Toggle state for attracting behavior
    private bool isInTriggerBox = false;   // State when in trigger box
    private float wanderTimer = 0;
    private Vector3 nextWanderDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody is required for this script to work!");
        }
        if (playerAudio == null)
        {
            Debug.LogError("AudioSource component is not assigned.");
        }
        if (attractionParticles != null)
            attractionParticles.Stop();  // Ensure particles are stopped at start
        if (animator == null)
        {
            Debug.LogError("Animator component is not assigned.");
        }
    }

    void Update()
    {
        if (playerAudio != null)
        {
            if (!playerAudio.isPlaying && isAttracting)
            {
                isAttracting = false;
                if (attractionParticles.isPlaying)
                    attractionParticles.Stop();
            }
            else if (playerAudio.isPlaying && isAttracting)
            {
                if (!attractionParticles.isPlaying)
                    attractionParticles.Play();
            }
        }
    }

    void FixedUpdate()
    {
        if (isInTriggerBox)
        {
            if (isAttracting)
            {
                AttractToPlayer();
            }
            else
            {
                WanderRandomly();
            }
        }
        else
        {
            HandleRepelAndAttract();
        }

        // Update animation state
        UpdateAnimationState();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == triggerBox)
        {
            isInTriggerBox = true;
            StartCoroutine(AttractionDelay());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == triggerBox)
        {
            isInTriggerBox = false;
            isAttracting = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator AttractionDelay()
    {
        isAttracting = true;
        yield return new WaitForSeconds(0.8f);
        isAttracting = false;
    }

    public void ToggleAttractingState()
    {
        if (!isInTriggerBox)
        {
            isAttracting = !isAttracting;
            if (isAttracting && playerAudio != null && !playerAudio.isPlaying)
            {
                playerAudio.Play();
            }
        }
    }

    private void AttractToPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        rb.velocity = Vector3.Lerp(rb.velocity, directionToPlayer.normalized * attractSpeed, Time.fixedDeltaTime);
        transform.LookAt(player.position);
    }

    private void HandleRepelAndAttract()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        if (isAttracting && distance < attractRadius)
        {
            Vector3 attractDirection = directionToPlayer.normalized;
            rb.velocity = Vector3.Lerp(rb.velocity, attractDirection * attractSpeed, Time.fixedDeltaTime);
            transform.LookAt(player.position);
        }
        else if (!isAttracting && distance < repelRadius)
        {
            Vector3 repelDirection = -directionToPlayer.normalized;
            float forceMagnitude = forceStrength * (1 - (distance / repelRadius));
            rb.velocity = Vector3.Lerp(rb.velocity, repelDirection * attractSpeed, Time.fixedDeltaTime);
            transform.LookAt(transform.position + repelDirection);
        }
    }

    void WanderRandomly()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            nextWanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            wanderTimer = Random.Range(0.5f, 1.5f);
            transform.rotation = Quaternion.LookRotation(nextWanderDirection);
        }

        rb.velocity = nextWanderDirection * wanderSpeed;
    }

    // Manage animation states based on movement
    private void UpdateAnimationState()
    {
        bool isMoving = rb.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isIdle", !isMoving);
    }
}