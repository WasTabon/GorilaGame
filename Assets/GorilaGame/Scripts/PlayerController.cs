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
        
        // Настройка Rigidbody для лучшего контроля
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.drag = drag;
        
        // Поиск джойстика если не назначен
        if (joystick == null)
        {
            joystick = FindObjectOfType<Joystick>();
            if (joystick == null)
            {
                Debug.LogError("Joystick not found! Please assign it in the inspector.");
            }
        }
        
        // Поиск камеры если не назначена
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
        HandleInput();
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
        
        // Получаем ввод с джойстика
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        
        // Если есть камера, вычисляем направление относительно неё
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
        
        // Применяем мертвую зону
        if (moveDirection.magnitude < 0.1f)
        {
            moveDirection = Vector3.zero;
        }
    }
    
    private void HandleMovement()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            // Плавное ускорение
            targetVelocity = moveDirection * moveSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            
            wasMoving = true;
        }
        else
        {
            // Моментальный запуск анимации остановки
            if (wasMoving)
            {
                wasMoving = false;
                OnStopMoving();
            }
            
            // Плавное замедление физики
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            
            if (currentVelocity.magnitude < 0.1f)
            {
                currentVelocity = Vector3.zero;
            }
        }
        
        // Применяем движение через Rigidbody
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
        
        // Управление анимациями без проверки текущего состояния Animator
        if (currentAnimState != AnimationState.Stopping)
        {
            if (speed > 0.1f)
            {
                // Переход в бег
                if (currentAnimState != AnimationState.Running)
                {
                    currentAnimState = AnimationState.Running;
                    animator.Play(runAnimationName, 0, 0f); // Используем Play вместо CrossFade
                }
            }
            else if (speed < 0.05f && !wasMoving)
            {
                // Переход в Idle
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
            // Если нет анимации остановки, сразу в Idle
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
        
        // Ждем пока анимация остановки проиграется
        yield return new WaitForSeconds(0.5f); // Настройте под длину вашей анимации
        
        // Переход в idle только если всё еще не двигаемся
        if (!wasMoving && currentVelocity.magnitude < 0.05f)
        {
            currentAnimState = AnimationState.Idle;
            animator.CrossFade(idleAnimationName, 0.2f);
        }
        else if (wasMoving)
        {
            // Если снова начали двигаться во время анимации остановки
            currentAnimState = AnimationState.Running;
            animator.Play(runAnimationName, 0, 0f);
        }
        
        stopAnimationCoroutine = null;
    }
    
    // Вспомогательные методы
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0f, newSpeed);
    }
    
    public void SetAcceleration(float newAcceleration)
    {
        acceleration = Mathf.Max(0.1f, newAcceleration);
    }
    
    public void SetDeceleration(float newDeceleration)
    {
        deceleration = Mathf.Max(0.1f, newDeceleration);
    }
}