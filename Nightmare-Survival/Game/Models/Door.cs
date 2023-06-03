namespace Nightmare_Survival
{
    public enum DoorMaterial
    {
        Wood,
        Stone,
        Iron
    }

    public class Door
    {
        public DoorMaterial Material { get; private set; }
        public int DoorHealth { get; private set; }
        public int MaxDoorHealth;
        public Point DoorPosition { get; set; }

        public Door(DoorMaterial material)
        {
            Material = material;
            DoorHealth = GetMaxHealth(material);
            MaxDoorHealth = DoorHealth;
        }

        public void Upgrade()
        {
            if (Material < DoorMaterial.Iron)
            {
                Material++;
                DoorHealth = GetMaxHealth(Material);
                MaxDoorHealth = DoorHealth;
            }
        }

        private int GetMaxHealth(DoorMaterial material)
        {
            switch (material)
            {
                case DoorMaterial.Wood:
                    return 50;
                case DoorMaterial.Stone:
                    return 100;
                case DoorMaterial.Iron:
                    return 200;
                default:
                    return 0;
            }
        }

        public void Update(GameTime gameTime)
        {

        }
    }

}