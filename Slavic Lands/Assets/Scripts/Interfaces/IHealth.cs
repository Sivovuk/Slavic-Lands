namespace Interfaces
{
    public interface IHealth
    {
        public void Heal(float amount);
        
        public bool TakeDamage(float amount);

        private bool ModifyHealth(float amount)
        {
            return false;
        }
    }
}