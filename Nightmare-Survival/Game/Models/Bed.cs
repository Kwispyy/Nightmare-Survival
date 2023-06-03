using MonoGame.Extended;

namespace Nightmare_Survival
{
    public class Bed
    {
        private Texture2D texture;
        private readonly SoundEffect sleepSound;

        public readonly int PointValue = 1;

        readonly Map map;
        public Map Map
        {
            get { return map; }
        }

        public Bed(Map map)
        {
            this.map = map;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Map.Content.Load<Texture2D>("Tiles/bed");
            //sleepSound = Map.Content.Load<SoundEffect>("Sounds/Sleeping");
        }

        public void PlayerSleeps(Player sleep)
        {
            sleepSound.Play();
        }
    }
}