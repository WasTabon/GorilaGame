using DG.Tweening;
using TMPro;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private RectTransform _dialogueBox;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CookedFoodData _cookedFoodData;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _stopDistance = 0.5f;

    private Animator _animator;

    private CookedFoodType _requestedFood;
    
    private Transform _movePos1;
    private Transform _movePos2;

    private bool _isInitialized;
    private bool _reachedDestination;

    private void Start()
    {
        _dialogueBox.DOScale(Vector3.zero, 0f);
    }

    public void Initiallize(Transform movePos1, Transform movePo2)
    {
        _animator = GetComponent<Animator>();
        
        _movePos1 = movePos1;
        _movePos2 = movePo2;
        _isInitialized = true;
        _reachedDestination = false;
        
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
            Vector3 direction = (_movePos2.position - transform.position).normalized;
            direction.y = 0f;
            transform.position += direction * _moveSpeed * Time.deltaTime;
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            _reachedDestination = true;
            SetWalkAnimation(false);

            // Выбираем случайную еду
            _requestedFood = (CookedFoodType)Random.Range(0, System.Enum.GetValues(typeof(CookedFoodType)).Length);
            
            // Устанавливаем название еды в текст
            if (_cookedFoodData != null)
            {
                _text.text = "I want " + _cookedFoodData.GetCookedFoodName(_requestedFood);
            }
            else
            {
                _text.text = "I want " + _requestedFood.ToString();
            }
            
            _dialogueBox.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.InOutBack);
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

    public CookedFoodType GetRequestedFood()
    {
        return _requestedFood;
    }
}