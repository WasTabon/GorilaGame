using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button buyButton;

    private UpgradeData upgradeData;
    private int upgradeIndex;
    private UpgradeManager upgradeManager;

    public void Initialize(UpgradeData data, int index, UpgradeManager manager)
    {
        upgradeData = data;
        upgradeIndex = index;
        upgradeManager = manager;

        upgradeNameText.text = data.upgradeName;
        descriptionText.text = data.description;
        
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        int currentLevel = upgradeManager.GetUpgradeLevel(upgradeIndex);
        levelText.text = $"Level: {currentLevel}/{upgradeData.maxLevel}";
        
        bool canUpgrade = currentLevel < upgradeData.maxLevel;
        buyButton.interactable = canUpgrade;
        
        if (!canUpgrade)
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
        }
    }

    private void OnBuyButtonClicked()
    {
        upgradeManager.TryBuyUpgrade(upgradeIndex);
    }
}