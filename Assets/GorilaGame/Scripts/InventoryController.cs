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

    public GameObject foodCard;
    public RectTransform content;
    
    public List<FoodType> foodsInInventory;
    private List<GameObject> foodCardObjects;
    
    private void Awake()
    {
        Instance = this;

        foodsInInventory = new List<FoodType>();
        foodCardObjects = new List<GameObject>();
    }

    public void AddFood(FoodType foodType)
    {
        foodsInInventory.Add(foodType);
        
        GameObject food = Instantiate(foodCard, content);
        foodCardObjects.Add(food);
    }

    private void UseFood(int index)
    {
        if (index >= 0 && index < foodsInInventory.Count)
        {
            foodsInInventory.RemoveAt(index);
            
            if (index < foodCardObjects.Count)
            {
                Destroy(foodCardObjects[index]);
                foodCardObjects.RemoveAt(index);
            }
        }
    }
    
    public void UseFood(FoodType foodType)
    {
        int index = foodsInInventory.IndexOf(foodType);
        if (index != -1)
        {
            UseFood(index);
        }
    }
}