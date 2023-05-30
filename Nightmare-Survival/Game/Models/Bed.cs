using MonoGame.Extended;

namespace Nightmare_Survival
{
    class Bed
    {
        private Texture2D texture;
        private Vector2 origin;
        private readonly SoundEffect sleepSound;

        public readonly int PointValue = 1;
        public readonly Color Color = Color.Red;

        private Vector2 basePosition;
        private float bounce;
        readonly Map map;
        public Map Map
        {
            get { return map; }
        }

        public Vector2 Position
        {
            get { return basePosition = new Vector2(0.0f, bounce); }
        } 

        public Bed(Map map, Vector2 position)
        {
            this.map = map;
            this.basePosition = position;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Map.Content.Load<Texture2D>("Tiles/bed");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            //sleepSound = Map.Content.Load<SoundEffect>("Sounds/Sleeping");
        }

        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        public void PlayerSleeps(Player sleep)
        {
            sleepSound.Play();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}