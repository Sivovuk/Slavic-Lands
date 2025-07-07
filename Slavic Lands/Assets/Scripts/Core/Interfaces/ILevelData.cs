namespace Core.Interfaces
{
    public interface ILevelData
    {
        int CurrentLevel { get; }
        void AddXp(int xp);
    }
}