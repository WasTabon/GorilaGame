using UnityEngine;

[System.Serializable]
public class FoodInfo
{
    public FoodType foodType;
    public string foodName;
    public Sprite foodIcon;
}

[CreateAssetMenu(fileName = "FoodData", menuName = "Game/Food Data")]
public class FoodData : ScriptableObject
{
    public FoodInfo[] foods;
    
    public FoodInfo GetFoodInfo(FoodType foodType)
    {
        foreach (var food in foods)
        {
            if (food.foodType == foodType)
                return food;
        }
        return null;
    }
    
    public string GetFoodName(FoodType foodType)
    {
        var foodInfo = GetFoodInfo(foodType);
        return foodInfo?.foodName ?? foodType.ToString();
    }
    
    public Sprite GetFoodIcon(FoodType foodType)
    {
        var foodInfo = GetFoodInfo(foodType);
        return foodInfo?.foodIcon;
    }
}