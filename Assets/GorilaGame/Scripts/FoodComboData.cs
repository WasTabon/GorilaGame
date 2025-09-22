using UnityEngine;

[System.Serializable]
public class FoodCombination
{
    public string comboName;
    public FoodType[] ingredients;
    public Sprite comboIcon;
}

[CreateAssetMenu(fileName = "FoodComboData", menuName = "Game/Food Combination Data")]
public class FoodComboData : ScriptableObject
{
    public FoodCombination[] foodCombinations;
    
    public FoodCombination GetCombo(FoodType[] ingredients)
    {
        foreach (var combo in foodCombinations)
        {
            if (combo.ingredients.Length != ingredients.Length)
                continue;

            bool match = true;
            for (int i = 0; i < ingredients.Length; i++)
            {
                if (!System.Array.Exists(combo.ingredients, x => x == ingredients[i]))
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return combo;
        }
        return null;
    }
}