using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CatchController : MonoBehaviour
{
    public static CatchController Instance;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _secondCamera;
    
    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform gameStartPosition;
    [SerializeField] private Transform gameEndPosition;
    
    [Header("UI Elements")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button stopGameButton;
    
    [Header("Food Spawning")]
    [SerializeField] private List<GameObject> _foodsToSpawn;
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float minSpawnDelay = 0.3f;
    [SerializeField] private float maxSpawnDelay = 1f;
    [SerializeField] private Transform spawnPointA;
    [SerializeField] private Transform spawnPointB;
    [SerializeField] private float spawnPositionX = 0f;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    
    private Coroutine spawnCoroutine;
    private bool isGameActive = false;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetMainCamera();
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        joystick.gameObject.SetActive(true);
        moveLeftButton.gameObject.SetActive(false);
        moveRightButton.gameObject.SetActive(false);
        stopGameButton.gameObject.SetActive(false);
        
        moveLeftButton.onClick.AddListener(OnMoveLeftClick);
        moveRightButton.onClick.AddListener(OnMoveRightClick);
        stopGameButton.onClick.AddListener(StopGame);
    }

    public void StartGame()
    {
        if (isGameActive) return;
        
        isGameActive = true;
        
        joystick.transform.DOScale(0f, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => 
            {
                joystick.gameObject.SetActive(false);
                MovePlayerToStartPosition();
            });
    }
    
    private void MovePlayerToStartPosition()
    {
        playerController.MoveToPosition(gameStartPosition.position);
        StartCoroutine(WaitForPlayerToReachPosition());
    }
    
    private IEnumerator WaitForPlayerToReachPosition()
    {
        while (playerController.IsAutoMoving())
        {
            yield return null;
        }
        
        SetSecondCamera();
        
        ShowGameButtons();
        StartFoodSpawning();
    }
    
    private void ShowGameButtons()
    {
        moveLeftButton.gameObject.SetActive(true);
        moveRightButton.gameObject.SetActive(true);
        stopGameButton.gameObject.SetActive(true);
        
        moveLeftButton.transform.localScale = Vector3.zero;
        moveRightButton.transform.localScale = Vector3.zero;
        stopGameButton.transform.localScale = Vector3.zero;
        
        moveLeftButton.transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack).SetDelay(0f);
        moveRightButton.transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack).SetDelay(0.1f);
        stopGameButton.transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack).SetDelay(0.2f);
    }
    
    private void StartFoodSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnFood());
    }

    private IEnumerator SpawnFood()
    {
        while (isGameActive)
        {
            if (_foodsToSpawn.Count > 0)
            {
                Debug.Log("FoodSpawn");
                
                GameObject foodPrefab = _foodsToSpawn[Random.Range(0, _foodsToSpawn.Count)];
                
                float randomZ = Random.Range(spawnPointA.position.z, spawnPointB.position.z);
                Vector3 spawnPosition = new Vector3(spawnPointA.position.x, spawnPointA.position.y, randomZ);
                
                GameObject spawnedFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
            }
            
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    public void StopGame()
    {
        if (!isGameActive) return;
        
        isGameActive = false;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        SetMainCamera();
        HideGameButtons();
        playerController.MoveToPosition(gameEndPosition.position);
        StartCoroutine(WaitAndShowJoystick());
    }
    
    private void HideGameButtons()
    {
        moveLeftButton.transform.DOScale(0f, animationDuration).SetEase(Ease.InBack)
            .OnComplete(() => moveLeftButton.gameObject.SetActive(false));
        
        moveRightButton.transform.DOScale(0f, animationDuration).SetEase(Ease.InBack)
            .OnComplete(() => moveRightButton.gameObject.SetActive(false));
        
        stopGameButton.transform.DOScale(0f, animationDuration).SetEase(Ease.InBack)
            .OnComplete(() => stopGameButton.gameObject.SetActive(false));
    }
    
    private IEnumerator WaitAndShowJoystick()
    {
        while (playerController.IsAutoMoving())
        {
            yield return null;
        }
        
        joystick.gameObject.SetActive(true);
        joystick.transform.localScale = Vector3.zero;
        joystick.transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
    }
    
    private void OnMoveLeftClick()
    {
        Debug.Log("Move Left");
    }
    
    private void OnMoveRightClick()
    {
        Debug.Log("Move Right");
    }

    public void SetMainCamera()
    {
        _secondCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(true);
    }
    
    public void SetSecondCamera()
    {
        _secondCamera.gameObject.SetActive(true);
        _mainCamera.gameObject.SetActive(false);
    }
}