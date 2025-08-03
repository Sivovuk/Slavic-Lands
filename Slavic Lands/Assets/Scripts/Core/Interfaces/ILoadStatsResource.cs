using Data;

namespace Core.Interfaces
{
    public interface ILoadStatsResource
    {
        public void LoadResourceData(ResourceSO resourceSO);
    }
    
    public interface ILoadStatsEntity
    {
        public void LoadEntityData(EntitySO entityData);
    }
}