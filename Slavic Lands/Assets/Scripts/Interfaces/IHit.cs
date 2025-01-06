using System;
using Gameplay.Resources;

namespace Interfaces
{
    public interface IHit
    {
        public bool TakeDamage(float damage, Action<ResourceType, int> callback = null);

        public ResourceType GetResourceType();
    }
}