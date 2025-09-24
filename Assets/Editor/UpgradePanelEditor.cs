using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class UpgradePanelEditor : EditorWindow
{
    [MenuItem("Tools/Create Upgrade Panel")]
    public static void ShowWindow()
    {
        GetWindow<UpgradePanelEditor>("Create Upgrade Panel");
    }

    private Transform parentCanvas;

    private void OnGUI()
    {
        GUILayout.Label("Create Upgrade Panel", EditorStyles.boldLabel);
        
        parentCanvas = (Transform)EditorGUILayout.ObjectField("Parent Canvas", parentCanvas, typeof(Transform), true);

        if (GUILayout.Button("Create Upgrade Panel"))
        {
            if (parentCanvas != null)
            {
                CreateUpgradePanel();
            }
            else
            {
                Debug.LogWarning("Assign Parent Canvas first!");
            }
        }
    }

    private void CreateUpgradePanel()
    {
        // Создаем основную панель
        GameObject upgradePanel = CreateUIObject("UpgradePanel", parentCanvas);
        upgradePanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        upgradePanel.AddComponent<UpgradeManager>();
        
        RectTransform panelRect = upgradePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Создаем контейнер для содержимого
        GameObject contentContainer = CreateUIObject("ContentContainer", upgradePanel.transform);
        Image containerImage = contentContainer.AddComponent<Image>();
        containerImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        RectTransform containerRect = contentContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.1f, 0.1f);
        containerRect.anchorMax = new Vector2(0.9f, 0.9f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Создаем заголовок
        GameObject title = CreateTextObject("Title", "UPGRADES", contentContainer.transform, 24);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // Создаем кнопку закрытия
        GameObject closeButton = CreateButtonObject("CloseButton", "X", contentContainer.transform);
        RectTransform closeRect = closeButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.9f, 0.9f);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.offsetMin = Vector2.zero;
        closeRect.offsetMax = Vector2.zero;

        // Создаем Scroll View для апгрейдов
        GameObject scrollView = CreateScrollView("UpgradeScrollView", contentContainer.transform);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 0.9f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;

        // Получаем Content из Scroll View
        Transform content = scrollView.transform.Find("Viewport/Content");

        // Создаем префаб для апгрейда
        GameObject upgradeItemPrefab = CreateUpgradeItemPrefab();
        
        // Сохраняем префаб как ассет
        string prefabPath = "Assets/UpgradeItemPrefab.prefab";
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(upgradeItemPrefab, prefabPath);
        DestroyImmediate(upgradeItemPrefab);

        // Создаем панель "Недостаточно монет"
        GameObject noMoneyPanel = CreateNoMoneyPanel(upgradePanel.transform);

        // Настраиваем UpgradeManager
        UpgradeManager upgradeManager = upgradePanel.GetComponent<UpgradeManager>();
        SerializedObject serializedObject = new SerializedObject(upgradeManager);
        serializedObject.FindProperty("upgradeItemPrefab").objectReferenceValue = savedPrefab;
        serializedObject.FindProperty("upgradeContent").objectReferenceValue = content.GetComponent<RectTransform>();
        serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton.GetComponent<Button>();
        serializedObject.FindProperty("noMoneyPanel").objectReferenceValue = noMoneyPanel;
        serializedObject.ApplyModifiedProperties();
        
        AssetDatabase.Refresh();

        upgradePanel.SetActive(false);
        Debug.Log("Upgrade Panel created successfully!");
    }

    private GameObject CreateNoMoneyPanel(Transform parent)
    {
        GameObject noMoneyPanel = CreateUIObject("NoMoneyPanel", parent);
        noMoneyPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        
        RectTransform panelRect = noMoneyPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject messageContainer = CreateUIObject("MessageContainer", noMoneyPanel.transform);
        Image containerImage = messageContainer.AddComponent<Image>();
        containerImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        RectTransform containerRect = messageContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.3f, 0.4f);
        containerRect.anchorMax = new Vector2(0.7f, 0.6f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        GameObject messageText = CreateTextObject("MessageText", "Not enough coins!", messageContainer.transform, 20);
        RectTransform messageRect = messageText.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0, 0.3f);
        messageRect.anchorMax = new Vector2(1, 0.7f);
        messageRect.offsetMin = Vector2.zero;
        messageRect.offsetMax = Vector2.zero;

        GameObject okButton = CreateButtonObject("OKButton", "OK", messageContainer.transform);
        RectTransform okRect = okButton.GetComponent<RectTransform>();
        okRect.anchorMin = new Vector2(0.3f, 0.1f);
        okRect.anchorMax = new Vector2(0.7f, 0.3f);
        okRect.offsetMin = Vector2.zero;
        okRect.offsetMax = Vector2.zero;

        Button okButtonComponent = okButton.GetComponent<Button>();
        okButtonComponent.onClick.AddListener(() => noMoneyPanel.SetActive(false));

        noMoneyPanel.SetActive(false);
        return noMoneyPanel;
    }

    private GameObject CreateUpgradeItemPrefab()
    {
        GameObject prefab = CreateUIObject("UpgradeItemPrefab", null);
        prefab.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
        prefab.AddComponent<UpgradeItem>();
        
        RectTransform prefabRect = prefab.GetComponent<RectTransform>();
        prefabRect.sizeDelta = new Vector2(400, 100);

        // Upgrade Name
        GameObject upgradeName = CreateTextObject("UpgradeNameText", "Upgrade Name", prefab.transform, 18);
        RectTransform nameRect = upgradeName.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.6f);
        nameRect.anchorMax = new Vector2(0.6f, 1);
        nameRect.offsetMin = new Vector2(10, 0);
        nameRect.offsetMax = new Vector2(-5, 0);
        upgradeName.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        // Upgrade Description
        GameObject description = CreateTextObject("DescriptionText", "Upgrade Description", prefab.transform, 14);
        RectTransform descriptionRect = description.GetComponent<RectTransform>();
        descriptionRect.anchorMin = new Vector2(0, 0.2f);
        descriptionRect.anchorMax = new Vector2(0.6f, 0.6f);
        descriptionRect.offsetMin = new Vector2(10, 0);
        descriptionRect.offsetMax = new Vector2(-5, 0);
        description.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        description.GetComponent<TextMeshProUGUI>().color = Color.gray;

        // Level Text
        GameObject levelText = CreateTextObject("LevelText", "Level: 0", prefab.transform, 12);
        RectTransform levelRect = levelText.GetComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0, 0);
        levelRect.anchorMax = new Vector2(0.6f, 0.2f);
        levelRect.offsetMin = new Vector2(10, 0);
        levelRect.offsetMax = new Vector2(-5, 0);
        levelText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        levelText.GetComponent<TextMeshProUGUI>().color = Color.yellow;

        // Buy Button
        GameObject buyButton = CreateButtonObject("BuyButton", "Buy (100)", prefab.transform);
        RectTransform buyRect = buyButton.GetComponent<RectTransform>();
        buyRect.anchorMin = new Vector2(0.7f, 0.2f);
        buyRect.anchorMax = new Vector2(0.95f, 0.8f);
        buyRect.offsetMin = Vector2.zero;
        buyRect.offsetMax = Vector2.zero;
        buyButton.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.8f, 1f);

        // Настраиваем UpgradeItem
        UpgradeItem itemUI = prefab.GetComponent<UpgradeItem>();
        SerializedObject serializedObject = new SerializedObject(itemUI);
        serializedObject.FindProperty("upgradeNameText").objectReferenceValue = upgradeName.GetComponent<TextMeshProUGUI>();
        serializedObject.FindProperty("descriptionText").objectReferenceValue = description.GetComponent<TextMeshProUGUI>();
        serializedObject.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();
        serializedObject.FindProperty("buyButton").objectReferenceValue = buyButton.GetComponent<Button>();
        serializedObject.ApplyModifiedProperties();

        // Добавляем LayoutElement
        LayoutElement layoutElement = prefab.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 100;

        return prefab;
    }

    private GameObject CreateScrollView(string name, Transform parent)
    {
        GameObject scrollView = CreateUIObject(name, parent);
        scrollView.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        // Viewport
        GameObject viewport = CreateUIObject("Viewport", scrollView.transform);
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0);
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        // Content
        GameObject content = CreateUIObject("Content", viewport.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 0);
        
        // Добавляем VerticalLayoutGroup
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.elasticity = 0.1f;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
        scrollRect.scrollSensitivity = 1.0f;

        return scrollView;
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    private GameObject CreateTextObject(string name, string text, Transform parent, int fontSize)
    {
        GameObject textObj = CreateUIObject(name, parent);
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        return textObj;
    }

    private GameObject CreateButtonObject(string name, string text, Transform parent)
    {
        GameObject buttonObj = CreateUIObject(name, parent);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.8f, 1f);
        Button button = buttonObj.AddComponent<Button>();
        
        GameObject buttonText = CreateTextObject("Text", text, buttonObj.transform, 14);
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
    }
}