using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CatchController : MonoBehaviour
{
    public static CatchController Instance;

    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _secondCamera;
    
    private List<GameObject> _foodsToSpawn;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetMainCamera();
    }

    private IEnumerator SpawnFood()
    {
        return null;
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
