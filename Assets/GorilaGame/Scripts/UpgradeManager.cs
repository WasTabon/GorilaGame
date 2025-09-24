using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public string description;
    public int maxLevel = 10;
}

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private GameObject upgradeItemPrefab;
    [SerializeField] private RectTransform upgradeContent;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject noMoneyPanel;
    
    private const int UPGRADE_COST = 100;
    private const string SAVE_KEY_PREFIX = "UpgradeLevel_";
    
    private List<UpgradeData> upgradeDataList = new List<UpgradeData>
    {
        new UpgradeData { upgradeName = "Food Quality", description = "Improves the quality of cooked food" },
        new UpgradeData { upgradeName = "Product Quality", description = "Better ingredients drop more often" },
        new UpgradeData { upgradeName = "Shop Advertisement", description = "Attracts more customers to your shop" },
        new UpgradeData { upgradeName = "Cooking Speed", description = "Reduces cooking time significantly" },
        new UpgradeData { upgradeName = "Customer Patience", description = "Customers wait longer before leaving" },
        new UpgradeData { upgradeName = "Coin Multiplier", description = "Increases coins earned from sales" }
    };
    
    private List<UpgradeItem> upgradeItems = new List<UpgradeItem>();

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePanel);
        CreateUpgradeItems();
        UpdateAllUpgradeItems();
    }

    private void Update()
    {
        coinsText.text = $"Money: {InventoryController.Instance.coins}";
    }

    private void CreateUpgradeItems()
    {
        for (int i = 0; i < upgradeDataList.Count; i++)
        {
            GameObject item = Instantiate(upgradeItemPrefab, upgradeContent);
            UpgradeItem upgradeItem = item.GetComponent<UpgradeItem>();
            upgradeItem.Initialize(upgradeDataList[i], i, this);
            upgradeItems.Add(upgradeItem);
        }
    }
    
    public void UpdateAllUpgradeItems()
    {
        foreach (var item in upgradeItems)
        {
            item.UpdateDisplay();
        }
    }

    public bool TryBuyUpgrade(int upgradeIndex)
    {
        if (InventoryController.Instance.coins >= UPGRADE_COST)
        {
            int currentLevel = GetUpgradeLevel(upgradeIndex);
            if (currentLevel < upgradeDataList[upgradeIndex].maxLevel)
            {
                InventoryController.Instance.coins -= UPGRADE_COST;
                SetUpgradeLevel(upgradeIndex, currentLevel + 1);
                UpdateAllUpgradeItems();
                return true;
            }
        }
        else
        {
            ShowNoMoneyPanel();
        }
        return false;
    }
    
    public int GetUpgradeLevel(int upgradeIndex)
    {
        return PlayerPrefs.GetInt(SAVE_KEY_PREFIX + upgradeIndex, 0);
    }
    
    private void SetUpgradeLevel(int upgradeIndex, int level)
    {
        PlayerPrefs.SetInt(SAVE_KEY_PREFIX + upgradeIndex, level);
        PlayerPrefs.Save();
    }
    
    private void ShowNoMoneyPanel()
    {
        if (noMoneyPanel != null)
        {
            noMoneyPanel.SetActive(true);
        }
    }
    
    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}