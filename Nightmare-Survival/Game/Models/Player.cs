using MonoGame.Extended;
using MonoGame.Extended.Shapes;
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


        private float timer; // Timer to keep track of time

        private const float currencyIncreaseInterval = 1f; // Currency increase interval (in seconds)
        private const int currencyIncreaseAmount = 1; // Amount of currency added when increasing

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

        private const float playerSpeed = 70;

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
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            ApplyPhysics(gameTime);

            // Increasing the timer
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(direction.X != 0)
            {
                sprite.PlayAnimation(runAnimation);
            }
            else
            {
                sprite.PlayAnimation(idleAnimation);
            }

            direction = Vector2.Zero;
        }

        //Rewrite the controls, I want the player to be able to walk diagonally
        public void ApplyPhysics(GameTime gameTime)
        {
            Vector2 previousPosition = Position;
            KeyboardState keyboardState = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply velocity.
            if (keyboardState.IsKeyDown(Keys.A))
            {
                position.X -= playerSpeed * delta;
                direction = new Vector2(-1, 0);
                flip = SpriteEffects.None;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                position.X += playerSpeed * delta;
                direction = new Vector2(1, 0);
                flip = SpriteEffects.FlipHorizontally;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                position.Y -= playerSpeed * delta;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                position.Y += playerSpeed * delta;
            }

            HandleCollisions();
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

                        if(collision == TileCollision.Bed && Keyboard.GetState().IsKeyDown(Keys.E))
                        {
                            // If enough time has passed, increase the currency
                            if (timer >= currencyIncreaseInterval)
                            {
                                Map.Value += currencyIncreaseAmount;
                                timer = 0f; // Reset timer
                            }
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