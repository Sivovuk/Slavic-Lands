using System;
using System.Collections.Generic;
using Gameplay.Resources;
using UnityEngine;

namespace Interfaces
{
    public interface IHit
    {
        public bool TakeDamage(float damage, Action<List<ResourceData>, ResourceSO> callback = null);
        public void ApplyForce(float force, Vector2 direction);

        public List<ResourceData> GetResourceType();
    }
}