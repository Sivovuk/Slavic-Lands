using System.Collections.Generic;
using Gameplay.Player;
using Gameplay.Resources;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private Transform _container; // Parent where resource UI elements are placed
    [SerializeField] private TMP_Text _resourcePrefab; // Prefab for each resource display

    private readonly Dictionary<ResourceType, TMP_Text> _resourceTexts = new();

    public void Initialize(PlayerResource playerResource)
    {
        playerResource.OnResourceChanged += UpdateResource;

        // Initialize UI for all existing resources
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            CreateResourceUI(type, playerResource.GetResource(type));
        }
    }

    private void CreateResourceUI(ResourceType type, int amount)
    {
        var textElement = Instantiate(_resourcePrefab, _container);
        textElement.text = $"{type}: {amount}";
        _resourceTexts[type] = textElement;
    }

    private void UpdateResource(ResourceType type, int amount)
    {
        if (_resourceTexts.ContainsKey(type))
        {
            _resourceTexts[type].text = $"{type}: {amount}";
        }
        else
        {
            // If a new resource type appears during gameplay
            CreateResourceUI(type, amount);
        }
    }
}