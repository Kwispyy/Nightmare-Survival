using System;

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

        private static Vector2 direction;
        public static Vector2 Direction => direction;
        public static bool Moving => direction != Vector2.Zero;

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

        int value;
        public int Value
        {
            get { return value; }
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
            GetInput(gameTime);

            ApplyPhysics(gameTime);

            //if (Velocity.X > 0)
            //{
            //    sprite.PlayAnimation(runAnimation);
            //}
            //else
            //{
            //    sprite.PlayAnimation(idleAnimation);
            //}

            direction = Vector2.Zero;
            
        }

        private void GetInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                direction.Y--;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                direction.X--;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                direction.Y++;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                direction.X++;
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            velocity.X = 2.0f;
            velocity.Y = 2.0f;
            Vector2 previousPosition = Position;

            // Apply velocity.
            if(direction.X < 0.0f)
            {
                flip = SpriteEffects.None;
                position.X -= velocity.X;
            }
            else if(direction.X > 0.0f)
            {
                flip = SpriteEffects.FlipHorizontally;
                position.X += velocity.X;
            }
            else if(direction.Y < 0.0f)
            {
                position.Y -= velocity.Y;
            }
            else if(direction.Y > 0.0f)
            {
                position.Y += velocity.Y;
            }

            HandleCollisions();

            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        private void HandleCollisions()
        {
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    TileCollision collision = Map.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        Rectangle tileBounds = Map.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);

                        if (collision == TileCollision.Wall)
                        {
                            Position = new Vector2(Position.X + depth.X, Position.Y + depth.Y);
                        }

                        if(collision == TileCollision.Bed)
                        {
                            value += 1;
                        }
                    }
                }
            }
        }

        public void OnKilled()
        {
            isAlive = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}