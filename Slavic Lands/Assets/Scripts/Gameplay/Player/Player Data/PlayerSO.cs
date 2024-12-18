using UnityEngine;

namespace Gameplay.Player
{
    [CreateAssetMenu(fileName = "New Player", menuName = "Player")]
    public class PlayerSO : ScriptableObject
    {
        [Header("Movement")]
        public float WalkSpeed;
        public float RunSpeed;
        
        [Header("Jump")]
        public float JumpForce;
        
        [Header("Health")]
        public float MaxHealth;

        [Header("Energy")] 
        public float MaxEnergy;

        [Header("Character")] 
        public int StartingLevel = 1;
    }
}