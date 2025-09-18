using UnityEngine;

public enum FoodType
{
    
}

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    
    
    private void Awake()
    {
        Instance = this;
    }
}
