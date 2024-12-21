using Gameplay.Player;
using UnityEngine;
using Input = UnityEngine.Input;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] private float _attackDamage;
    
    [SerializeField] private PlayerHealth _playerHealth;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _playerHealth.TakeDamage(_attackDamage);
        }
    }
}
