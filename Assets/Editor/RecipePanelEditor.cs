using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class RecipePanelEditor : EditorWindow
{
    [MenuItem("Tools/Create Recipe Panel")]
    public static void ShowWindow()
    {
        GetWindow<RecipePanelEditor>("Create Recipe Panel");
    }

    private Transform parentObject;
    private FoodComboData foodComboData;

    private void OnGUI()
    {
        GUILayout.Label("Create Recipe Panel", EditorStyles.boldLabel);
        
        parentObject = (Transform)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(Transform), true);
        foodComboData = (FoodComboData)EditorGUILayout.ObjectField("Food Combo Data", foodComboData, typeof(FoodComboData), false);

        if (GUILayout.Button("Create Recipe Panel"))
        {
            if (parentObject != null && foodComboData != null)
            {
                CreateRecipePanel();
            }
            else
            {
                Debug.LogWarning("Assign Parent Object and Food Combo Data first!");
            }
        }
    }

    private void CreateRecipePanel()
    {
        // Создаем основную панель
        GameObject recipePanel = CreateUIObject("RecipePanel", parentObject);
        recipePanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        recipePanel.AddComponent<RecipePanelUI>();
        
        RectTransform panelRect = recipePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Создаем контейнер для содержимого
        GameObject contentContainer = CreateUIObject("ContentContainer", recipePanel.transform);
        Image containerImage = contentContainer.AddComponent<Image>();
        containerImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        RectTransform containerRect = contentContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.1f, 0.1f);
        containerRect.anchorMax = new Vector2(0.9f, 0.9f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Создаем заголовок
        GameObject title = CreateTextObject("Title", "RECIPES", contentContainer.transform, 24);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // Создаем текст с монетами
        GameObject coinsObject = CreateTextObject("CoinsText", "Coins: 0", contentContainer.transform, 18);
        RectTransform coinsRect = coinsObject.GetComponent<RectTransform>();
        coinsRect.anchorMin = new Vector2(0, 0.8f);
        coinsRect.anchorMax = new Vector2(1, 0.9f);
        coinsRect.offsetMin = Vector2.zero;
        coinsRect.offsetMax = Vector2.zero;

        // Создаем кнопку закрытия
        GameObject closeButton = CreateButtonObject("CloseButton", "X", contentContainer.transform);
        RectTransform closeRect = closeButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.9f, 0.9f);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.offsetMin = Vector2.zero;
        closeRect.offsetMax = Vector2.zero;

        // Создаем Scroll View
        GameObject scrollView = CreateScrollView("RecipeScrollView", contentContainer.transform);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 0.8f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;

        // Получаем Content из Scroll View
        Transform content = scrollView.transform.Find("Viewport/Content");

        // Создаем префаб для рецепта
        GameObject recipeItemPrefab = CreateRecipeItemPrefab();
        
        // Сохраняем префаб как ассет
        string prefabPath = "Assets/RecipeItemPrefab.prefab";
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(recipeItemPrefab, prefabPath);
        DestroyImmediate(recipeItemPrefab); // Удаляем временный объект

        // Настраиваем RecipePanelUI
        RecipePanelUI panelUI = recipePanel.GetComponent<RecipePanelUI>();
        SerializedObject serializedObject = new SerializedObject(panelUI);
        serializedObject.FindProperty("foodComboData").objectReferenceValue = foodComboData;
        serializedObject.FindProperty("recipeItemPrefab").objectReferenceValue = savedPrefab;
        serializedObject.FindProperty("recipeContent").objectReferenceValue = content.GetComponent<RectTransform>();
        serializedObject.FindProperty("closeButton").objectReferenceValue = closeButton.GetComponent<Button>();
        serializedObject.FindProperty("coinsText").objectReferenceValue = coinsObject.GetComponent<TextMeshProUGUI>();
        serializedObject.ApplyModifiedProperties();
        
        // Обновляем базу данных ассетов
        AssetDatabase.Refresh();

        // Создаем RecipeManager если его нет
        CreateRecipeManager();

        recipePanel.SetActive(false);
        Debug.Log("Recipe Panel created successfully!");
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
        buttonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        Button button = buttonObj.AddComponent<Button>();
        
        GameObject buttonText = CreateTextObject("Text", text, buttonObj.transform, 16);
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
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
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        
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
        
        // Добавляем VerticalLayoutGroup
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;

        return scrollView;
    }

    private GameObject CreateRecipeItemPrefab()
    {
        GameObject prefab = CreateUIObject("RecipeItemPrefab", null);
        prefab.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
        prefab.AddComponent<RecipeItemUI>();
        
        RectTransform prefabRect = prefab.GetComponent<RectTransform>();
        prefabRect.sizeDelta = new Vector2(400, 80);

        // Recipe Name
        GameObject recipeName = CreateTextObject("RecipeNameText", "Recipe Name", prefab.transform, 16);
        RectTransform nameRect = recipeName.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(0.4f, 1);
        nameRect.offsetMin = new Vector2(10, 0);
        nameRect.offsetMax = new Vector2(-5, 0);
        recipeName.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

        // Ingredients
        GameObject ingredients = CreateTextObject("IngredientsText", "Ingredient1 + Ingredient2", prefab.transform, 14);
        RectTransform ingredientsRect = ingredients.GetComponent<RectTransform>();
        ingredientsRect.anchorMin = new Vector2(0, 0);
        ingredientsRect.anchorMax = new Vector2(0.4f, 0.5f);
        ingredientsRect.offsetMin = new Vector2(10, 0);
        ingredientsRect.offsetMax = new Vector2(-5, 0);
        ingredients.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        ingredients.GetComponent<TextMeshProUGUI>().color = Color.gray;

        // Buy Button
        GameObject buyButton = CreateButtonObject("BuyButton", "Buy (50)", prefab.transform);
        RectTransform buyRect = buyButton.GetComponent<RectTransform>();
        buyRect.anchorMin = new Vector2(0.6f, 0.2f);
        buyRect.anchorMax = new Vector2(0.9f, 0.8f);
        buyRect.offsetMin = Vector2.zero;
        buyRect.offsetMax = Vector2.zero;
        buyButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);

        // Cost Text
        GameObject costText = CreateTextObject("CostText", "50", buyButton.transform, 14);
        RectTransform costRect = costText.GetComponent<RectTransform>();
        costRect.anchorMin = Vector2.zero;
        costRect.anchorMax = Vector2.one;
        costRect.offsetMin = Vector2.zero;
        costRect.offsetMax = Vector2.zero;

        // Locked Overlay
        GameObject lockedOverlay = CreateUIObject("LockedOverlay", prefab.transform);
        lockedOverlay.AddComponent<Image>().color = new Color(0, 0, 0, 0.7f);
        RectTransform overlayRect = lockedOverlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        // Unlocked Text
        GameObject unlockedText = CreateTextObject("UnlockedText", "UNLOCKED", prefab.transform, 16);
        RectTransform unlockedRect = unlockedText.GetComponent<RectTransform>();
        unlockedRect.anchorMin = new Vector2(0.6f, 0.2f);
        unlockedRect.anchorMax = new Vector2(0.9f, 0.8f);
        unlockedRect.offsetMin = Vector2.zero;
        unlockedRect.offsetMax = Vector2.zero;
        unlockedText.GetComponent<TextMeshProUGUI>().color = Color.green;

        // Настраиваем RecipeItemUI
        RecipeItemUI itemUI = prefab.GetComponent<RecipeItemUI>();
        SerializedObject serializedObject = new SerializedObject(itemUI);
        serializedObject.FindProperty("recipeNameText").objectReferenceValue = recipeName.GetComponent<TextMeshProUGUI>();
        serializedObject.FindProperty("ingredientsText").objectReferenceValue = ingredients.GetComponent<TextMeshProUGUI>();
        serializedObject.FindProperty("buyButton").objectReferenceValue = buyButton.GetComponent<Button>();
        serializedObject.FindProperty("costText").objectReferenceValue = costText.GetComponent<TextMeshProUGUI>();
        serializedObject.FindProperty("lockedOverlay").objectReferenceValue = lockedOverlay;
        serializedObject.FindProperty("unlockedText").objectReferenceValue = unlockedText.GetComponent<TextMeshProUGUI>();
        serializedObject.ApplyModifiedProperties();

        // Добавляем LayoutElement
        LayoutElement layoutElement = prefab.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 80;

        return prefab;
    }

    private void CreateRecipeManager()
    {
        GameObject recipeManagerObj = GameObject.Find("RecipeManager");
        if (recipeManagerObj == null)
        {
            recipeManagerObj = new GameObject("RecipeManager");
            recipeManagerObj.AddComponent<RecipeManager>();
            Debug.Log("RecipeManager created!");
        }
    }
}