using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [SerializeField] private List<int> unlockedRecipeIndexes = new List<int>();
    
    private const int RECIPE_COST = 50;
    private const string SAVE_KEY = "UnlockedRecipes";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUnlockedRecipes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Первый рецепт разблокирован по умолчанию
        if (unlockedRecipeIndexes.Count == 0)
        {
            Debug.Log("Zero");
            unlockedRecipeIndexes.Add(0);
            SaveUnlockedRecipes();
        }
    }

    public bool IsRecipeUnlocked(int recipeIndex)
    {
        return unlockedRecipeIndexes.Contains(recipeIndex);
    }

    public bool CanBuyRecipe(int recipeIndex)
    {
        return !IsRecipeUnlocked(recipeIndex) && InventoryController.Instance.coins >= RECIPE_COST;
    }

    public bool BuyRecipe(int recipeIndex)
    {
        if (CanBuyRecipe(recipeIndex))
        {
            InventoryController.Instance.coins -= RECIPE_COST;
            unlockedRecipeIndexes.Add(recipeIndex);
            SaveUnlockedRecipes();
            return true;
        }
        else
        {
            Debug.Log("Cant Buy");
            return false;   
        }
    }

    public int GetRecipeCost()
    {
        return RECIPE_COST;
    }

    private void SaveUnlockedRecipes()
    {
        string data = string.Join(",", unlockedRecipeIndexes);
        PlayerPrefs.SetString(SAVE_KEY, data);
        PlayerPrefs.Save();
    }

    private void LoadUnlockedRecipes()
    {
        string data = PlayerPrefs.GetString(SAVE_KEY, "");
        unlockedRecipeIndexes.Clear();
        
        if (!string.IsNullOrEmpty(data))
        {
            string[] indexes = data.Split(',');
            foreach (string index in indexes)
            {
                if (int.TryParse(index, out int result))
                {
                    unlockedRecipeIndexes.Add(result);
                }
            }
        }
    }

    public bool CanCraftRecipe(FoodType[] ingredients, FoodComboData comboData)
    {
        for (int i = 0; i < comboData.foodCombinations.Length; i++)
        {
            var combo = comboData.foodCombinations[i];
            if (combo.ingredients.Length != ingredients.Length)
                continue;

            bool match = true;
            for (int j = 0; j < ingredients.Length; j++)
            {
                if (!System.Array.Exists(combo.ingredients, x => x == ingredients[j]))
                {
                    match = false;
                    break;
                }
            }

            if (match && IsRecipeUnlocked(i))
                return true;
        }
        return false;
    }
}