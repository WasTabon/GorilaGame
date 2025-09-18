using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float rotationSpeed = 720f; // градусов в секунду
    
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
    private float lastMovementTime;
    
    // Animation states
    private bool isStoppingAnimation;
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
            // Получаем forward и right направления камеры, но только по XZ плоскости
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            
            // Убираем Y компоненту для движения только по XZ
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            // Вычисляем направление движения относительно камеры
            moveDirection = (cameraRight * horizontal + cameraForward * vertical).normalized;
        }
        else
        {
            // Fallback - движение в мировых координатах
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        }
        
        // Применяем мертвую зону для более точного контроля
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
            
            // Сохраняем время последнего движения
            lastMovementTime = Time.time;
            wasMoving = true;
        }
        else
        {
            // Плавное замедление с инерцией
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            
            // Полная остановка при очень малой скорости
            if (currentVelocity.magnitude < 0.1f)
            {
                currentVelocity = Vector3.zero;
                if (wasMoving)
                {
                    wasMoving = false;
                    OnStopMoving();
                }
            }
        }
        
        // Применяем движение через Rigidbody
        Vector3 moveVelocity = currentVelocity;
        moveVelocity.y = rb.velocity.y; // Сохраняем вертикальную скорость
        rb.velocity = moveVelocity;
    }
    
    private void HandleRotation()
    {
        // Поворачиваем персонажа в направлении движения
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
        
        // Плавный переход между анимациями
        if (!isStoppingAnimation)
        {
            if (speed > 0.1f)
            {
                // Бег
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName(runAnimationName))
                {
                    animator.CrossFade(runAnimationName, 0.1f);
                }
            }
            else if (speed < 0.05f && !wasMoving)
            {
                // Idle
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName(idleAnimationName))
                {
                    animator.CrossFade(idleAnimationName, 0.2f);
                }
            }
        }
    }
    
    private void OnStopMoving()
    {
        // Запускаем анимацию остановки сразу без ожидания
        if (animator != null && !string.IsNullOrEmpty(stopAnimationName))
        {
            if (stopAnimationCoroutine != null)
            {
                StopCoroutine(stopAnimationCoroutine);
            }
            stopAnimationCoroutine = StartCoroutine(PlayStopAnimation());
        }
    }
    
    private IEnumerator PlayStopAnimation()
    {
        isStoppingAnimation = true;
        animator.CrossFade(stopAnimationName, 0.1f);
        
        // Ждем пока анимация остановки проиграется
        yield return new WaitForSeconds(0.5f); // Настройте под длину вашей анимации
        
        // Переход в idle только если всё еще не двигаемся
        if (!wasMoving && currentVelocity.magnitude < 0.05f)
        {
            animator.CrossFade(idleAnimationName, 0.2f);
        }
        isStoppingAnimation = false;
        stopAnimationCoroutine = null;
    }
    
    // Вспомогательные методы для настройки
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