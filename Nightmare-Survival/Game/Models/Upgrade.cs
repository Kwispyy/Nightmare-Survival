namespace Nightmare_Survival
{
    public class Upgrade
    {
        public string Name { get; }
        public int Cost { get; }

        public Upgrade(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }

    }
}