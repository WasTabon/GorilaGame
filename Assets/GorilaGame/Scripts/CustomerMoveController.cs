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

    private void Start()
    {
        CustomerSpawnHandler();
    }

    private void CustomerSpawnHandler()
    {
        if (_currentCustomer == null)
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
            _fullSell.SetActive(true);
        }
        else
        {
            // Нужной еды нет - даем случайную за меньшую цену
            int randomIndex = Random.Range(0, InventoryController.Instance.cookedFoodsInInventory.Count);
            CookedFoodType randomFood = InventoryController.Instance.cookedFoodsInInventory[randomIndex];
            
            InventoryController.Instance.UseCookedFood(randomFood);
            InventoryController.Instance.coins += 5;
            _wrongSell.SetActive(true);
        }
        
        // Убираем текущего покупателя и спавним нового
        DestroyCurrentCustomer();
        CustomerSpawnHandler();
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