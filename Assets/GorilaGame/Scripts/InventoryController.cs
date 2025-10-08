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
public enum CookedFoodType
{
    BananaPizza,
    BananaMilk,
    FishBanana,
    SpicyBanana,
    BananaOmelette,
    MilkPizza,
    FishPizza,
    SpicyPizza,
    EggPizza,
    FishMilk,
    SpicyMilk,
    MilkEgg,
    SpicyFish,
    FishEgg,
    PepperEgg
}

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    [SerializeField] private AudioClip _sound;
    
    private int _coins;

    public int coins
    {
        get { return _coins; }
        set 
        { 
            _coins = value;
            PlayerPrefs.SetInt("PlayerCoins", _coins);
            PlayerPrefs.Save();
        }
    }
    
    public FoodData foodData;
    public CookedFoodData cookedFoodData;
    
    public GameObject foodCard;
    public GameObject cookedFoodCard;
    public RectTransform content;
    public RectTransform cookedFoodContent;
    
    public List<FoodType> foodsInInventory;
    public List<CookedFoodType> cookedFoodsInInventory;
    
    private List<GameObject> foodCardObjects;
    private List<GameObject> cookedFoodCardObjects;
    
    private void Awake()
    {
        Instance = this;

        _coins = PlayerPrefs.GetInt("PlayerCoins", 0); 
        
        foodsInInventory = new List<FoodType>();
        cookedFoodsInInventory = new List<CookedFoodType>();
        foodCardObjects = new List<GameObject>();
        cookedFoodCardObjects = new List<GameObject>();
    }

    [ContextMenu("Give coins")]
    public void GiveCoins()
    {
        coins += 50;
    }
    
    public void AddFood(FoodType foodType)
    {
        MusicController.Instance.PlaySpecificSound(_sound);
        
        foodsInInventory.Add(foodType);
        
        GameObject food = Instantiate(foodCard, content);
        food.transform.Find("FoodIcon").GetComponent<Image>().sprite = foodData.GetFoodIcon(foodType);
        food.GetComponentInChildren<TextMeshProUGUI>().text = foodData.GetFoodName(foodType);
        food.GetComponent<FoodButton>().food = foodType;
        food.GetComponent<FoodButton>().sprite = foodData.GetFoodIcon(foodType);
        foodCardObjects.Add(food);
    }

    public void AddCookedFood(CookedFoodType cookedFoodType)
    {
        cookedFoodsInInventory.Add(cookedFoodType);
        
        GameObject cookedFood = Instantiate(cookedFoodCard, cookedFoodContent);
        cookedFood.transform.Find("FoodIcon").GetComponent<Image>().sprite = cookedFoodData.GetCookedFoodIcon(cookedFoodType);
        cookedFood.GetComponentInChildren<TextMeshProUGUI>().text = cookedFoodData.GetCookedFoodName(cookedFoodType);
        cookedFood.GetComponent<CookedFoodButton>().cookedFood = cookedFoodType;
        cookedFood.GetComponent<CookedFoodButton>().sprite = cookedFoodData.GetCookedFoodIcon(cookedFoodType);
        cookedFoodCardObjects.Add(cookedFood);
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

    private void UseCookedFood(int index)
    {
        if (index >= 0 && index < cookedFoodsInInventory.Count)
        {
            cookedFoodsInInventory.RemoveAt(index);
            
            if (index < cookedFoodCardObjects.Count)
            {
                Destroy(cookedFoodCardObjects[index]);
                cookedFoodCardObjects.RemoveAt(index);
            }
        }
    }
    
    public void UseCookedFood(CookedFoodType cookedFoodType)
    {
        int index = cookedFoodsInInventory.IndexOf(cookedFoodType);
        if (index != -1)
        {
            UseCookedFood(index);
        }
    }
}