using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float rotationSpeed = 720f;
    
    [Header("Auto Movement Settings")]
    [SerializeField] private float stopDistance = 0.1f;
    
    [Header("Physics Settings")]
    [SerializeField] private float drag = 8f;
    
    [Header("Joystick")]
    [SerializeField] private Joystick joystick;
    
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string idleAnimationName = "Idle";
    [SerializeField] private string runAnimationName = "Run";
    [SerializeField] private string stopAnimationName = "Stop";
    
    // Components
    private Rigidbody rb;
    private BoxCollider boxCollider;
    
    // State
    private Vector3 moveDirection;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    private bool wasMoving;
    
    // Auto movement
    private bool isAutoMoving = false;
    private Vector3 targetPosition;
    
    // Animation states
    private enum AnimationState
    {
        Idle,
        Running,
        Stopping
    }
    private AnimationState currentAnimState = AnimationState.Idle;
    private Coroutine stopAnimationCoroutine;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.drag = drag;
        
        if (joystick == null)
        {
            joystick = FindObjectOfType<Joystick>();
            if (joystick == null)
            {
                Debug.LogError("Joystick not found! Please assign it in the inspector.");
            }
        }
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
        }
    }
    
    void Update()
    {
        if (isAutoMoving)
        {
            HandleAutoMovement();
        }
        else
        {
            HandleInput();
        }
        
        HandleRotation();
        HandleAnimations();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void HandleInput()
    {
        if (joystick == null) return;
        
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        
        if (playerCamera != null)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            moveDirection = (cameraRight * horizontal + cameraForward * vertical).normalized;
        }
        else
        {
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        }
        
        if (moveDirection.magnitude < 0.1f)
        {
            moveDirection = Vector3.zero;
        }
    }
    
    private void HandleAutoMovement()
    {
        Vector3 currentPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetPos = new Vector3(targetPosition.x, 0f, targetPosition.z);
        
        float distanceToTarget = Vector3.Distance(currentPos, targetPos);
        
        if (distanceToTarget <= stopDistance)
        {
            StopAutoMovement();
            return;
        }
        
        moveDirection = (targetPos - currentPos).normalized;
    }
    
    private void HandleMovement()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            targetVelocity = moveDirection * moveSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            
            wasMoving = true;
        }
        else
        {
            if (wasMoving)
            {
                wasMoving = false;
                OnStopMoving();
            }
            
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            
            if (currentVelocity.magnitude < 0.1f)
            {
                currentVelocity = Vector3.zero;
            }
        }
        
        Vector3 moveVelocity = currentVelocity;
        moveVelocity.y = rb.velocity.y;
        rb.velocity = moveVelocity;
    }
    
    private void HandleRotation()
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            Vector3 lookDirection = currentVelocity.normalized;
            lookDirection.y = 0;
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
    
    private void HandleAnimations()
    {
        if (animator == null) return;
        
        float speed = currentVelocity.magnitude;
        
        if (currentAnimState != AnimationState.Stopping)
        {
            if (speed > 0.1f)
            {
                if (currentAnimState != AnimationState.Running)
                {
                    currentAnimState = AnimationState.Running;
                    animator.Play(runAnimationName, 0, 0f);
                }
            }
            else if (speed < 0.05f && !wasMoving)
            {
                if (currentAnimState != AnimationState.Idle)
                {
                    currentAnimState = AnimationState.Idle;
                    animator.CrossFade(idleAnimationName, 0.2f);
                }
            }
        }
    }
    
    private void OnStopMoving()
    {
        if (animator != null && !string.IsNullOrEmpty(stopAnimationName))
        {
            if (stopAnimationCoroutine != null)
            {
                StopCoroutine(stopAnimationCoroutine);
            }
            stopAnimationCoroutine = StartCoroutine(PlayStopAnimation());
        }
        else
        {
            currentAnimState = AnimationState.Idle;
            if (animator != null)
            {
                animator.CrossFade(idleAnimationName, 0.2f);
            }
        }
    }
    
    private IEnumerator PlayStopAnimation()
    {
        currentAnimState = AnimationState.Stopping;
        animator.CrossFade(stopAnimationName, 0.1f);
        
        yield return new WaitForSeconds(0.5f);
        
        if (!wasMoving && currentVelocity.magnitude < 0.05f)
        {
            currentAnimState = AnimationState.Idle;
            animator.CrossFade(idleAnimationName, 0.2f);
        }
        else if (wasMoving)
        {
            currentAnimState = AnimationState.Running;
            animator.Play(runAnimationName, 0, 0f);
        }
        
        stopAnimationCoroutine = null;
    }
    
    public void MoveToPosition(Vector3 position)
    {
        targetPosition = position;
        isAutoMoving = true;
    }
    
    public void StopAutoMovement()
    {
        isAutoMoving = false;
        moveDirection = Vector3.zero;
    }
}