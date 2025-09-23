using TMPro;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private RectTransform _dialogueBox;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _stopDistance = 0.5f;

    private Animator _animator;
    
    private Transform _movePos1;
    private Transform _movePos2;

    private bool _isInitialized;
    private bool _reachedDestination;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Initiallize(Transform movePos1, Transform movePo2)
    {
        _movePos1 = movePos1;
        _movePos2 = movePo2;
        _isInitialized = true;
        _reachedDestination = false;
        
        // Начинаем движение
        SetWalkAnimation(true);
    }

    private void Update()
    {
        if (_isInitialized && !_reachedDestination)
        {
            MoveToTarget();
        }
    }
    
    private void MoveToTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _movePos2.position);
        
        if (distanceToTarget > _stopDistance)
        {
            // Двигаемся к цели
            Vector3 direction = (_movePos2.position - transform.position).normalized;
            direction.y = transform.position.y;
            transform.position += direction * _moveSpeed * Time.deltaTime;
            
            // Поворачиваем персонажа в сторону движения
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            // Достигли цели
            _reachedDestination = true;
            SetWalkAnimation(false);
            Debug.Log("Customer reached destination!");
        }
    }
    
    private void SetWalkAnimation(bool isWalking)
    {
        if (_animator != null)
        {
            _animator.SetBool("isWalk", isWalking);
            _animator.SetBool("isIdle", !isWalking);
        }
    }
}