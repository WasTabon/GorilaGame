using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Transform cookButton;
    [SerializeField] private Transform sellButton;
    [SerializeField] private Transform playButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease animationEase = Ease.OutBack;
    
    private Dictionary<string, Transform> buttonMap;
    
    void Start()
    {
        InitializeButtons();
        HideAllButtons();
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
            buttonMap[tag].DOScale(Vector3.one, animationDuration)
                .SetEase(animationEase);
        }
    }
    
    public void HideButton(string tag)
    {
        if (buttonMap.ContainsKey(tag) && buttonMap[tag] != null)
        {
            buttonMap[tag].DOScale(Vector3.zero, animationDuration)
                .SetEase(animationEase);
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