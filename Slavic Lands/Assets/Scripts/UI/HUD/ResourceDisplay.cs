using Gameplay.Player;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _woodTMP;
    [SerializeField] private TMP_Text _stoneTMP;
    [SerializeField] private TMP_Text _hideTMP;
    
    public void UpdateResource(PlayerResource resource)
    {
        _woodTMP.text = resource.Wood.ToString();
        _stoneTMP.text = resource.Stone.ToString();
        _hideTMP.text = resource.Hide.ToString();
    }
}
