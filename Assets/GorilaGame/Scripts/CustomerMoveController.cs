using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerMoveController : MonoBehaviour
{
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private Transform _movePos1;

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
}
