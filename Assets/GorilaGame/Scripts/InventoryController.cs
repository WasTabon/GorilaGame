using System.Collections.Generic;
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
    
    public List<FoodType> foodsInInventory;
    
    private void Awake()
    {
        Instance = this;

        foodsInInventory = new List<FoodType>();
    }
}
