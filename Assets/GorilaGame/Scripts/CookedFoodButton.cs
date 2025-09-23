using UnityEngine;
using UnityEngine.UI;

public class CookedFoodButton : MonoBehaviour
{
    public CookedFoodType cookedFood;
    public Sprite sprite;
    
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
    {
        // Здесь можно добавить логику использования готовой еды
        // Например, удаление из инвентаря или другие действия
        InventoryController.Instance.UseCookedFood(cookedFood);
    }
}