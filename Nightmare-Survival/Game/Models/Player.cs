namespace Nightmare_Survival
{
    public class Player
    {
        // Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private AnimationPlayer sprite;
        private SpriteEffects flip = SpriteEffects.None;

        public Map Map
        {
            get { return map; }
        }
        Map map;

        // Checking if the player is alive
        bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
        }

        // Player position
        Vector2 position;
        public Vector2 Position
        {
            get { return Position; }
            set { Position = value; }
        }

        // Player speed
        Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { Velocity = value; }
        }

        private Rectangle localPlayerBounds;
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localPlayerBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localPlayerBounds.Y;
                
                return new Rectangle(left, top, localPlayerBounds.Width, localPlayerBounds.Height);
            }
        }

        public Player(Vector2 position)
        {
            LoadContent();

            Reset(position);
        }

        public void LoadContent()
        {
            idleAnimation = new Animation(Map.Content.Load<Texture2D>("Sprites/Player/idlePlayer"), 0.1f, true);
            runAnimation = new Animation(Map.Content.Load<Texture2D>("Sprites/Player/runPlayer"), 0.1f, true);
        }

        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        public void Update(GameTime gameTime)
        {
            if(InputManager.Moving)
            {
                position += Vector2.Normalize(InputManager.Direction) * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Here we need to implement the physics of the player

            if(Velocity.X > 0)
            {
                sprite.PlayAnimation(runAnimation);
            }
            else
            {
                sprite.PlayAnimation(idleAnimation);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if(Velocity.X < 0)
                flip = SpriteEffects.None;

            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}