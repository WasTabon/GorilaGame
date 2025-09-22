using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class CookController : MonoBehaviour
{
    public static CookController Instance;

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