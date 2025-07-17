using Core.Interfaces;
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
        [field:SerializeField] public int SlashXP { get; private set; }
        [field:SerializeField] public int ShieldBashXP { get; private set; }
        [field:SerializeField] public int PiercingArrowXP { get; private set; }
        
        public int GetXpForTool(ToolType tool)
        {
            return tool switch
            {
                ToolType.Axe => CuttingXP,
                ToolType.Pickaxe => MiningXP,
                ToolType.BattleAxe => AttackXP,
                ToolType.Bow => ShootXP,
                ToolType.Slashed => SlashXP,
                ToolType.ShieldBash => ShieldBashXP,
                ToolType.PiercingArrow => PiercingArrowXP,
                _ => 0
            };
        }
    }
}