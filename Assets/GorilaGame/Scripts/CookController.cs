using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CookController : MonoBehaviour
{
    public static CookController Instance;

    [SerializeField] private CookedFoodData _cookedFoodData;
    
    [SerializeField] private Sprite _cantCookIcon;
    
    [SerializeField] private GameObject _comboPanel;
    [SerializeField] private Image _comboIcon;
    [SerializeField] private TextMeshProUGUI _comboName;
    
    [SerializeField] private GameObject _cookParticle;
    
    [SerializeField] private FoodComboData _foodComboData;
    
    [SerializeField] private GameObject _inventoryPanel;
    
    [SerializeField] private Image _icon1;
    [SerializeField] private Image _icon2;
    [SerializeField] private TextMeshProUGUI _text1;
    [SerializeField] private TextMeshProUGUI _text2;
    
    [SerializeField] private RectTransform _moveButtons;
    [SerializeField] private RectTransform _cookButtons;
    [SerializeField] private float animationDuration = 0.5f;

    private FoodType _food1;
    private FoodType _food2;
    
    public bool isCook;
    public bool isFood1;
    public bool isFood2;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeCookButtons();
    }
    
    private void InitializeCookButtons()
    {
        for (int i = 0; i < _cookButtons.childCount; i++)
        {
            _cookButtons.GetChild(i).localScale = Vector3.zero;
        }
    }

    public void OpenCookInventory()
    {
        if (isCook)
            _inventoryPanel.SetActive(true);
    }
    
    public void SetFood1(FoodType food, Sprite icon)
    {
        isFood1 = true;
        _text1.gameObject.SetActive(false);
        _food1 = food;
        _icon1.gameObject.SetActive(true);
        _icon1.sprite = icon;
        _inventoryPanel.SetActive(false);
    }
    public void SetFood2(FoodType food, Sprite icon)
    {
        isFood2 = true;
        _text2.gameObject.SetActive(false);
        _food2 = food;
        _icon2.gameObject.SetActive(true);
        _icon2.sprite = icon;
        _inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (isFood1 && isFood2)
        {
            // СНАЧАЛА сохраняем ингредиенты
            FoodType savedFood1 = _food1;
            FoodType savedFood2 = _food2;
        
            isFood1 = false;
            isFood2 = false;
        
            _text1.gameObject.SetActive(true);
            _food1 = FoodType.Banana;
            _icon1.gameObject.SetActive(false);
            _icon1.sprite = null;
        
            _text2.gameObject.SetActive(true);
            _food2 = FoodType.Banana;
            _icon2.gameObject.SetActive(false);
            _icon2.sprite = null;

            _cookParticle.SetActive(true);
        
            DOVirtual.DelayedCall(1.5f, () =>
            {
                Debug.Log("Start Crafting");
            
                // Используем сохраненные ингредиенты
                FoodType[] ingredients = new FoodType[] { savedFood1, savedFood2 };

                if (RecipeManager.Instance.CanCraftRecipe(ingredients, _foodComboData))
                {
                    Debug.Log("Craft");
                    FoodCombination data = _foodComboData.GetCombo(ingredients);
                    _comboName.text = data?.comboName;
                    _comboIcon.sprite = data?.comboIcon;
                    _comboPanel.SetActive(true);

                    // Добавляем готовое блюдо в инвентарь
                    if (data != null)
                    {
                        Debug.Log("Added");
                        CookedFoodType cookedFoodType = _cookedFoodData.GetCookedFoodTypeByName(data.comboName);
                        InventoryController.Instance.AddCookedFood(cookedFoodType);
                    }
                }
                else
                {
                    _comboName.text = "You lost your ingredients";
                    _comboIcon.sprite = _cantCookIcon;
                    _comboPanel.SetActive(true);
                }

                Debug.Log("Off");
                _cookParticle.SetActive(false);
            });
        }
    }

    public void StartCook()
    {
        isCook = true;
        AnimateMoveButtonsOut();
    }
    
    private void AnimateMoveButtonsOut()
    {
        int childCount = _moveButtons.childCount;
        int completedAnimations = 0;
        
        for (int i = 0; i < childCount; i++)
        {
            Transform child = _moveButtons.GetChild(i);
            child.DOScale(0f, animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    completedAnimations++;
                    if (completedAnimations == childCount)
                    {
                        OnMoveButtonsHidden();
                    }
                });
        }
    }
    
    private void OnMoveButtonsHidden()
    {
        CatchController.Instance.SetThirdCamera();
        AnimateCookButtonsIn();
    }
    
    private void AnimateCookButtonsIn()
    {
        for (int i = 0; i < _cookButtons.childCount; i++)
        {
            Transform child = _cookButtons.GetChild(i);
            child.DOScale(1f, animationDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.1f);
        }
    }
    
    public void StopCook()
    {
        isCook = false;
        AnimateCookButtonsOut();
    }
    
    private void AnimateCookButtonsOut()
    {
        int childCount = _cookButtons.childCount;
        int completedAnimations = 0;
        
        for (int i = 0; i < childCount; i++)
        {
            Transform child = _cookButtons.GetChild(i);
            child.DOScale(0f, animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    completedAnimations++;
                    if (completedAnimations == childCount)
                    {
                        OnCookButtonsHidden();
                    }
                });
        }
    }
    
    private void OnCookButtonsHidden()
    {
        CatchController.Instance.SetMainCamera();
        AnimateMoveButtonsIn();
    }
    
    private void AnimateMoveButtonsIn()
    {
        Transform fixedJoystick = _moveButtons.Find("Fixed Joystick");
        if (fixedJoystick != null)
        {
            fixedJoystick.DOScale(1f, animationDuration)
                .SetEase(Ease.OutBack);
        }
    }
}