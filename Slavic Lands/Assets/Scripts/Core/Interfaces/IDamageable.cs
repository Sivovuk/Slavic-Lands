using System;
using System.Collections.Generic;
using Gameplay.Resources;
using UnityEngine;

namespace Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Action callback = null);
        void ApplyKnockback(Vector2 direction, float force);
        bool IsDead { get; }
    }
}