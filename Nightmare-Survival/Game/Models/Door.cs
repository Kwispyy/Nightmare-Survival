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

        public int doorHealth;

        public int MaxDoorHealth;
        public Point DoorPointPosition { get; set; }
        public Vector2 DoorPosition { get; set; }

        public bool doorIsBroken;

        float timer;

        public Dictionary<DoorMaterial, int> MaterialCost;


        public Door(DoorMaterial material)
        {
            Material = material;
            doorHealth = GetMaxHealth(material);
            MaxDoorHealth = doorHealth;
            doorIsBroken = false;
            MaterialCost = new Dictionary<DoorMaterial, int>();
            MaterialCost[DoorMaterial.Wood] = 40;
            MaterialCost[DoorMaterial.Stone] = 100;
            MaterialCost[DoorMaterial.Iron] = 200;
        }

        public void Upgrade()
        {
            if (Material < DoorMaterial.Iron)
            {
                Material++;
                doorHealth = GetMaxHealth(Material);
                MaxDoorHealth = doorHealth;
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

        public void TakeDamage(int damageAmount, GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(timer >= 1f)
            {
                doorHealth -= damageAmount;

                if (doorHealth <= 0)
                {
                    doorIsBroken |= true;
                }

                timer = 0f;
            }
            
        }

        public void Update(GameTime gameTime)
        {

        }
    }

}