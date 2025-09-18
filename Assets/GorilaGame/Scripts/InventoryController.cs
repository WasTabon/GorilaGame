using UnityEngine;

public enum FoodType
{
    Banana,
    Pizza,
    Milk,
    Fish,
    Pepper,
    Egg
}

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    
    private void Awake()
    {
        Instance = this;
    }
}
