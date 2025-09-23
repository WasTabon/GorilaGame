using UnityEngine;

[System.Serializable]
public class CookedFoodItem
{
    public CookedFoodType cookedFoodType;
    public string foodName;
    public Sprite foodIcon;
}

[CreateAssetMenu(fileName = "CookedFoodData", menuName = "Game/Cooked Food Data")]
public class CookedFoodData : ScriptableObject
{
    public CookedFoodItem[] cookedFoodItems;
    
    public string GetCookedFoodName(CookedFoodType cookedFoodType)
    {
        foreach (var item in cookedFoodItems)
        {
            if (item.cookedFoodType == cookedFoodType)
            {
                return item.foodName;
            }
        }
        return cookedFoodType.ToString();
    }
    
    public Sprite GetCookedFoodIcon(CookedFoodType cookedFoodType)
    {
        foreach (var item in cookedFoodItems)
        {
            if (item.cookedFoodType == cookedFoodType)
            {
                return item.foodIcon;
            }
        }
        return null;
    }
    
    public CookedFoodType GetCookedFoodTypeByName(string comboName)
    {
        switch (comboName)
        {
            case "Banana Pizza": return CookedFoodType.BananaPizza;
            case "Banana Milk": return CookedFoodType.BananaMilk;
            case "Fish Banana": return CookedFoodType.FishBanana;
            case "Spicy Banana": return CookedFoodType.SpicyBanana;
            case "Banana Omelette": return CookedFoodType.BananaOmelette;
            case "Milk Pizza": return CookedFoodType.MilkPizza;
            case "Fish Pizza": return CookedFoodType.FishPizza;
            case "Spicy Pizza": return CookedFoodType.SpicyPizza;
            case "Egg Pizza": return CookedFoodType.EggPizza;
            case "Fish Milk": return CookedFoodType.FishMilk;
            case "Spicy Milk": return CookedFoodType.SpicyMilk;
            case "Milk Egg": return CookedFoodType.MilkEgg;
            case "Spicy Fish": return CookedFoodType.SpicyFish;
            case "Fish Egg": return CookedFoodType.FishEgg;
            case "Pepper Egg": return CookedFoodType.PepperEgg;
            default: return CookedFoodType.BananaPizza;
        }
    }
}