using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Transform cookButton;
    [SerializeField] private Transform sellButton;
    [SerializeField] private Transform playButton;
    
    [Header("Food Pickup Animation")]
    [SerializeField] private TextMeshProUGUI foodPickupText;
    [SerializeField] private float animationDuration = 1.5f;
    [SerializeField] private float fadeDistance = 50f;
    
    [Header("Animation Settings")]
    [SerializeField] private float buttonAnimationDuration = 0.3f;
    [SerializeField] private Ease animationEase = Ease.OutBack;
    
    private Dictionary<string, Transform> buttonMap;
    private Vector3 originalTextPosition;
    
    void Start()
    {
        InitializeButtons();
        HideAllButtons();
        InitializeFoodPickupText();
    }
    
    private void InitializeButtons()
    {
        buttonMap = new Dictionary<string, Transform>
        {
            { "Cook", cookButton },
            { "Sell", sellButton },
            { "Play", playButton }
        };
    }
    
    private void InitializeFoodPickupText()
    {
        if (foodPickupText != null)
        {
            originalTextPosition = foodPickupText.transform.position;
            foodPickupText.gameObject.SetActive(false);
        }
    }
    
    private void HideAllButtons()
    {
        foreach (var button in buttonMap.Values)
        {
            if (button != null)
            {
                button.localScale = Vector3.zero;
            }
        }
    }
    
    public void ShowButton(string tag)
    {
        if (buttonMap.ContainsKey(tag) && buttonMap[tag] != null)
        {
            buttonMap[tag].DOScale(Vector3.one, buttonAnimationDuration)
                .SetEase(animationEase);
        }
    }
    
    public void HideButton(string tag)
    {
        if (buttonMap.ContainsKey(tag) && buttonMap[tag] != null)
        {
            buttonMap[tag].DOScale(Vector3.zero, buttonAnimationDuration)
                .SetEase(animationEase);
        }
    }
    
    public void ShowFoodPickupAnimation(FoodType foodType, Vector3 worldPosition)
    {
        if (foodPickupText == null) return;
        
        // Сбрасываем анимацию если она уже идет
        foodPickupText.transform.DOKill();
        
        // Конвертируем мировые координаты в экранные
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        
        // Устанавливаем позицию текста
        foodPickupText.transform.position = screenPosition;
        
        // Устанавливаем текст
        string foodName = GetFoodName(foodType);
        foodPickupText.text = $"+1 {foodName}";
        
        Color textColor = foodPickupText.color;
        textColor.a = 0f;
        foodPickupText.color = textColor;
        foodPickupText.transform.localScale = Vector3.zero;

// Включаем объект
        foodPickupText.gameObject.SetActive(true);

// Создаем последовательность анимации
        Sequence animSequence = DOTween.Sequence();

// Появление
        animSequence.Append(foodPickupText.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        animSequence.Join(foodPickupText.DOFade(1f, 0.2f));
        
        // Движение вверх
        animSequence.Append(foodPickupText.transform.DOMoveY(screenPosition.y + fadeDistance, animationDuration).SetEase(Ease.OutQuart));
        
        // Исчезновение
        animSequence.Join(foodPickupText.DOFade(0f, 0.5f).SetDelay(animationDuration - 0.5f));
        
        // Выключаем объект в конце
        animSequence.OnComplete(() => {
            foodPickupText.gameObject.SetActive(false);
            foodPickupText.transform.position = originalTextPosition;
        });
    }
    
    private string GetFoodName(FoodType foodType)
    {
        switch (foodType)
        {
            case FoodType.Banana: return "Banana";
            case FoodType.Pizza: return "Pizza";
            case FoodType.Milk: return "Milk";
            case FoodType.Fish: return "Fish";
            case FoodType.Pepper: return "Pepper";
            case FoodType.Egg: return "Egg";
            default: return foodType.ToString();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (buttonMap.ContainsKey(other.tag))
        {
            ShowButton(other.tag);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (buttonMap.ContainsKey(other.tag))
        {
            HideButton(other.tag);
        }
    }
}