using Gameplay.Player;

namespace Core.Interfaces
{
    public interface IGatherable
    {
        void Gather(ToolType toolType, PlayerCombat player);
    }

    public enum ToolType
    {
        None = 0,
        Axe = 1,
        Pickaxe = 2,
        BattleAxe = 3,
        Bow = 4,
        Slashed = 5,
        ShieldBash = 6,
        PiercingArrow = 7
    }
}