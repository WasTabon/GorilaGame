using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipePanelUI : MonoBehaviour
{
    [SerializeField] private FoodComboData foodComboData;
    [SerializeField] private GameObject recipeItemPrefab;
    [SerializeField] private RectTransform recipeContent;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI coinsText;

    private List<RecipeItemUI> recipeItems = new List<RecipeItemUI>();

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePanel);
        CreateRecipeItems();
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void CreateRecipeItems()
    {
        for (int i = 0; i < foodComboData.foodCombinations.Length; i++)
        {
            GameObject item = Instantiate(recipeItemPrefab, recipeContent);
            RecipeItemUI recipeItemUI = item.GetComponent<RecipeItemUI>();
            recipeItemUI.Initialize(foodComboData.foodCombinations[i], i, this);
            recipeItems.Add(recipeItemUI);
        }
    }

    public void UpdateUI()
    {
        if (InventoryController.Instance != null)
            coinsText.text = $"Money: {InventoryController.Instance.coins}";

        foreach (var item in recipeItems)
        {
            item.UpdateDisplay();
        }
    }

    public void OnRecipePurchased()
    {
        UpdateUI();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}