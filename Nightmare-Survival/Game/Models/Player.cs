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
            get { return position; }
            set { position = value; }
        }

        private float previousBottom;

        // Player speed
        Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private float movement;

        private Rectangle localBounds;
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public Player(Map map, Vector2 position)
        {
            this.map = map;

            LoadContent();

            Reset(position);
        }

        public void LoadContent()
        {
            idleAnimation = new Animation(Map.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, true);
            runAnimation = new Animation(Map.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, true);

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            GetInput(keyboardState);

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

        private void GetInput(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement = -1.0f;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                movement = 1.0f;
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