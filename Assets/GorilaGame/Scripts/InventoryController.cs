using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public int coins;
    
    public FoodData foodData;
    
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
        food.transform.Find("FoodIcon").GetComponent<Image>().sprite = foodData.GetFoodIcon(foodType);
        food.GetComponentInChildren<TextMeshProUGUI>().text = foodData.GetFoodName(foodType);
        food.GetComponent<FoodButton>().food = foodType;
        food.GetComponent<FoodButton>().sprite = foodData.GetFoodIcon(foodType);;
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