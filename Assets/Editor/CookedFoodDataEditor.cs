using UnityEngine;
using UnityEditor;

public class CookedFoodDataEditor : EditorWindow
{
    [MenuItem("Tools/Generate Cooked Food Data")]
    public static void ShowWindow()
    {
        GetWindow<CookedFoodDataEditor>("Generate Cooked Food Data");
    }

    private CookedFoodData cookedFoodData;

    private void OnGUI()
    {
        GUILayout.Label("Generate Cooked Food Data", EditorStyles.boldLabel);

        cookedFoodData = (CookedFoodData)EditorGUILayout.ObjectField("Target ScriptableObject", cookedFoodData, typeof(CookedFoodData), false);

        if (GUILayout.Button("Generate Cooked Food Items"))
        {
            if (cookedFoodData != null)
            {
                GenerateCookedFoodItems(cookedFoodData);
                EditorUtility.SetDirty(cookedFoodData);
                AssetDatabase.SaveAssets();
                Debug.Log("Cooked food items generated!");
            }
            else
            {
                Debug.LogWarning("Assign a CookedFoodData object first!");
            }
        }
    }

    private void GenerateCookedFoodItems(CookedFoodData data)
    {
        data.cookedFoodItems = new CookedFoodItem[]
        {
            new CookedFoodItem { cookedFoodType = CookedFoodType.BananaPizza, foodName = "Banana Pizza", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.BananaMilk, foodName = "Banana Milk", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.FishBanana, foodName = "Fish Banana", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.SpicyBanana, foodName = "Spicy Banana", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.BananaOmelette, foodName = "Banana Omelette", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.MilkPizza, foodName = "Milk Pizza", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.FishPizza, foodName = "Fish Pizza", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.SpicyPizza, foodName = "Spicy Pizza", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.EggPizza, foodName = "Egg Pizza", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.FishMilk, foodName = "Fish Milk", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.SpicyMilk, foodName = "Spicy Milk", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.MilkEgg, foodName = "Milk Egg", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.SpicyFish, foodName = "Spicy Fish", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.FishEgg, foodName = "Fish Egg", foodIcon = null },
            new CookedFoodItem { cookedFoodType = CookedFoodType.PepperEgg, foodName = "Pepper Egg", foodIcon = null }
        };
    }
}