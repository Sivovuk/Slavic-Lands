using System;
using System.Collections.Generic;
using Gameplay.Resources;

namespace Interfaces
{
    public interface IHit
    {
        public bool TakeDamage(float damage, Action<List<ResourceData>, ResourceSO> callback = null);

        public List<ResourceData> GetResourceType();
    }
}