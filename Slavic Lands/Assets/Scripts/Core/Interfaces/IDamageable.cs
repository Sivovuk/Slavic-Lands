using UnityEngine;

namespace Core.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float amount, ToolType toolType);
        void ApplyKnockback(Vector2 direction, float force);
        bool IsDead { get; }
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