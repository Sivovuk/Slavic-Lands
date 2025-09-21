using Core.Interfaces;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// ScriptableObject that defines how much XP is awarded for specific player actions/tools.
    /// Provides a centralized way to balance and modify XP values across tool types.
    /// </summary>
    [CreateAssetMenu(fileName = "New XP Data", menuName = "XP Data")]
    public class XPDataSO : ScriptableObject
    {
        [Header("Action")] 
        // XP rewards for different combat or tool-based actions

        [field: SerializeField] public int AttackXP { get; private set; }
        [field: SerializeField] public int ShootXP { get; private set; }
        [field: SerializeField] public int CuttingXP { get; private set; }
        [field: SerializeField] public int MiningXP { get; private set; }
        [field: SerializeField] public int SlashXP { get; private set; }
        [field: SerializeField] public int ShieldBashXP { get; private set; }
        [field: SerializeField] public int PiercingArrowXP { get; private set; }

        /// <summary>
        /// Returns the appropriate XP reward based on the tool type used.
        /// </summary>
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