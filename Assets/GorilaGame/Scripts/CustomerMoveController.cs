using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerMoveController : MonoBehaviour
{
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private Transform _movePos1;
    [SerializeField] private GameObject _noFoodWindow;
    [SerializeField] private GameObject _fullSell;
    [SerializeField] private GameObject _wrongSell;

    [SerializeField] private CustomerController[] _customers;
    
    private CustomerController _currentCustomer;
    private bool _waitingForCustomerReturn;

    private void Start()
    {
        CustomerSpawnHandler();
    }

    private void Update()
    {
        // Проверяем вернулся ли покупатель к спавну
        if (_waitingForCustomerReturn && _currentCustomer != null && _currentCustomer._reachedSpawn)
        {
            DestroyCurrentCustomer();
            _waitingForCustomerReturn = false;
            CustomerSpawnHandler();
        }
    }

    private void CustomerSpawnHandler()
    {
        if (_currentCustomer == null && !_waitingForCustomerReturn)
        {
            int random = Random.Range(0, _customers.Length);

            CustomerController spawned = Instantiate(_customers[random], _spawnPos.position, Quaternion.identity);

            _currentCustomer = spawned;

            _currentCustomer.Initiallize(_spawnPos, _movePos1);
        }
    }

    public void Sell()
    {
        if (_currentCustomer._reachedDestination == false) return;
        
        CookedFoodType neededFood = _currentCustomer.GetRequestedFood();
        
        // Проверяем есть ли вообще готовая еда в инвентаре
        if (InventoryController.Instance.cookedFoodsInInventory.Count == 0)
        {
            // Показываем окно "нет еды"
            if (_noFoodWindow != null)
            {
                _noFoodWindow.SetActive(true);
            }
            return;
        }
        
        // Проверяем есть ли нужная еда
        if (InventoryController.Instance.cookedFoodsInInventory.Contains(neededFood))
        {
            // Есть нужная еда - даем полную цену
            InventoryController.Instance.UseCookedFood(neededFood);
            InventoryController.Instance.coins += 15;
            if (_fullSell != null) _fullSell.SetActive(true);
        }
        else
        {
            // Нужной еды нет - даем случайную за меньшую цену
            int randomIndex = Random.Range(0, InventoryController.Instance.cookedFoodsInInventory.Count);
            CookedFoodType randomFood = InventoryController.Instance.cookedFoodsInInventory[randomIndex];
            
            InventoryController.Instance.UseCookedFood(randomFood);
            InventoryController.Instance.coins += 5;
            if (_wrongSell != null) _wrongSell.SetActive(true);
        }
        
        // Отправляем покупателя обратно к спавну
        _currentCustomer.StartReturning();
        _waitingForCustomerReturn = true;
    }
    
    private void DestroyCurrentCustomer()
    {
        if (_currentCustomer != null)
        {
            Destroy(_currentCustomer.gameObject);
            _currentCustomer = null;
        }
    }
}