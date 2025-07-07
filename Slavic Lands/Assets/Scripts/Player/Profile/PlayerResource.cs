using Core;
using Gameplay.Resources;
using UI.HUD;

namespace Gameplay.Player
{
    [System.Serializable]
    public class PlayerResource
    {
        public int Wood { get; private set; }
        public int Stone { get; private set; }
        public int Hide { get; private set; }
        public int Food { get; private set; }

        public void AddResource(int amount, ResourceType resourceType)
        {
            if (resourceType == ResourceType.Wood)
                AddWood(amount); 
            else if (resourceType == ResourceType.Stone)
                AddStone(amount);
            else if (resourceType == ResourceType.Hide)
                AddHide(amount);
            else if (resourceType == ResourceType.Food)
                AddFood(amount);
            
            HUDController.Instance._resourceDisplay.UpdateResource(this);
        }

        public void AddWood(int wood)
        {
            Wood += wood;
        }
        
        public void AddStone(int stone)
        {
            Stone += stone;
        }
        
        public void AddHide(int hide)
        {
            Hide += hide;
        }
        
        public void AddFood(int food)
        {
            Hide += food;
        }
    }
}