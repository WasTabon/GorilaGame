using UnityEngine;

public class FoodButton : MonoBehaviour
{
    [HideInInspector] public FoodType food;
    [HideInInspector] public Sprite sprite;
    
    public void UseFood()
    {
        if (CookController.Instance.isCook)
        {
            if (CookController.Instance.isFood1 == false)
            {
                CookController.Instance.SetFood1(food, sprite);
            }
            else if (CookController.Instance.isFood2 == false)
            {
                CookController.Instance.SetFood2(food, sprite);
            }
        }
    }
}
