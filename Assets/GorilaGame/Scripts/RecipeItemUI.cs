using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI ingredientsText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private TextMeshProUGUI unlockedText;

    private FoodCombination recipe;
    private int recipeIndex;
    private RecipePanelUI parentPanel;

    private bool _isUpdated;

    public void Initialize(FoodCombination recipe, int index, RecipePanelUI parent)
    {
        this.recipe = recipe;
        this.recipeIndex = index;
        this.parentPanel = parent;

        recipeNameText.text = recipe.comboName;
        ingredientsText.text = GetIngredientsText();
        costText.text = RecipeManager.Instance.GetRecipeCost().ToString();
        
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        
        UpdateDisplay();
    }

    private string GetIngredientsText()
    {
        if (recipe.ingredients.Length < 2) return "";
        
        return recipe.ingredients[0].ToString() + " + " + recipe.ingredients[1].ToString();
    }

    public void UpdateDisplay()
    {
        if (_isUpdated) return;
        
        bool isUnlocked = RecipeManager.Instance.IsRecipeUnlocked(recipeIndex);
        bool canBuy = RecipeManager.Instance.CanBuyRecipe(recipeIndex);

        lockedOverlay.SetActive(!isUnlocked);
        buyButton.gameObject.SetActive(!isUnlocked);
        unlockedText.gameObject.SetActive(isUnlocked);
        
        //buyButton.interactable = canBuy;

        // Визуальное отображение заблокированного состояния
        if (!isUnlocked)
        {
            recipeNameText.color = Color.gray;
            ingredientsText.color = Color.gray;
        }
        else
        {
            recipeNameText.color = Color.white;
            ingredientsText.color = Color.white;
            _isUpdated = true;
        }
    }

    private void OnBuyButtonClicked()
    {
        Debug.Log("Buy");
        if (RecipeManager.Instance.BuyRecipe(recipeIndex))
        {
            UpdateDisplay();
            parentPanel.OnRecipePurchased();
        }
    }
}