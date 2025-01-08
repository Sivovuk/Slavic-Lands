using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New XP Data", menuName = "XP Data")]
    public class XPDataSO : ScriptableObject
    {
        [Header("Action")] 
        [field:SerializeField] public int AttackXP { get; private set; }
        [field:SerializeField] public int ShootXP { get; private set; }
        [field:SerializeField] public int CuttingXP { get; private set; }
        [field:SerializeField] public int MiningXP { get; private set; }
        
    }
}