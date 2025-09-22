using UnityEngine;
using UnityEditor;

public class FoodComboDataEditor : EditorWindow
{
    [MenuItem("Tools/Generate Food Combos")]
    public static void ShowWindow()
    {
        GetWindow<FoodComboDataEditor>("Generate Food Combos");
    }

    private FoodComboData comboData;

    private void OnGUI()
    {
        GUILayout.Label("Generate Food Combinations", EditorStyles.boldLabel);

        comboData = (FoodComboData)EditorGUILayout.ObjectField("Target ScriptableObject", comboData, typeof(FoodComboData), false);

        if (GUILayout.Button("Generate Combos"))
        {
            if (comboData != null)
            {
                GenerateCombos(comboData);
                EditorUtility.SetDirty(comboData);
                AssetDatabase.SaveAssets();
                Debug.Log("Food combos generated!");
            }
            else
            {
                Debug.LogWarning("Assign a FoodComboData object first!");
            }
        }
    }

    private void GenerateCombos(FoodComboData data)
    {
        // Определяем ингредиенты
        FoodType banana = FoodType.Banana;
        FoodType pizza = FoodType.Pizza;
        FoodType milk = FoodType.Milk;
        FoodType fish = FoodType.Fish;
        FoodType pepper = FoodType.Pepper;
        FoodType egg = FoodType.Egg;

        data.foodCombinations = new FoodCombination[]
        {
            new FoodCombination { comboName = "Banana Pizza", ingredients = new FoodType[] { banana, pizza }, comboIcon = null },
            new FoodCombination { comboName = "Banana Milk", ingredients = new FoodType[] { banana, milk }, comboIcon = null },
            new FoodCombination { comboName = "Fish Banana", ingredients = new FoodType[] { banana, fish }, comboIcon = null },
            new FoodCombination { comboName = "Spicy Banana", ingredients = new FoodType[] { banana, pepper }, comboIcon = null },
            new FoodCombination { comboName = "Banana Omelette", ingredients = new FoodType[] { banana, egg }, comboIcon = null },
            new FoodCombination { comboName = "Milk Pizza", ingredients = new FoodType[] { pizza, milk }, comboIcon = null },
            new FoodCombination { comboName = "Fish Pizza", ingredients = new FoodType[] { pizza, fish }, comboIcon = null },
            new FoodCombination { comboName = "Spicy Pizza", ingredients = new FoodType[] { pizza, pepper }, comboIcon = null },
            new FoodCombination { comboName = "Egg Pizza", ingredients = new FoodType[] { pizza, egg }, comboIcon = null },
            new FoodCombination { comboName = "Fish Milk", ingredients = new FoodType[] { milk, fish }, comboIcon = null },
            new FoodCombination { comboName = "Spicy Milk", ingredients = new FoodType[] { milk, pepper }, comboIcon = null },
            new FoodCombination { comboName = "Milk Egg", ingredients = new FoodType[] { milk, egg }, comboIcon = null },
            new FoodCombination { comboName = "Spicy Fish", ingredients = new FoodType[] { fish, pepper }, comboIcon = null },
            new FoodCombination { comboName = "Fish Egg", ingredients = new FoodType[] { fish, egg }, comboIcon = null },
            new FoodCombination { comboName = "Pepper Egg", ingredients = new FoodType[] { pepper, egg }, comboIcon = null },
        };
    }
}
