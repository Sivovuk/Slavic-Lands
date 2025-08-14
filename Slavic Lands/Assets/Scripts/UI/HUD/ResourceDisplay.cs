using System.Collections.Generic;
using Gameplay.Player;
using Gameplay.Resources;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private TMP_Text _resourcePrefab;

    private readonly Dictionary<ResourceType, TMP_Text> _resourceTexts = new();

    public void Initialize(PlayerResource playerResource)
    {
        playerResource.OnResourceChanged += UpdateResource;

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            CreateResourceUI(type, playerResource.GetResource(type));
    }

    private void CreateResourceUI(ResourceType type, int amount)
    {
        var textElement = Instantiate(_resourcePrefab, _container);
        textElement.text = $"{type}: {amount}";
        _resourceTexts[type] = textElement;
    }

    private void UpdateResource(ResourceType type, int amount)
    {
        
        Debug.LogError("UpdateResource");
        if (_resourceTexts.ContainsKey(type))
            _resourceTexts[type].text = $"{type}: {amount}";
        else
            CreateResourceUI(type, amount);
    }
}