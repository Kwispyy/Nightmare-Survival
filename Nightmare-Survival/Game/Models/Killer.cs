using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Nightmare_Survival
{
    public class Killer
    {
        // Animations
        private Animation idleAnimation;
        private AnimationPlayer sprite;
        private SpriteEffects flip = SpriteEffects.None;

        // Killer data
        private bool isHunting;
        private bool isResting;
        private bool canMove;
        private int direct;

        private const float killerSpeed = 70;

        private bool lastHuntPhase = false;

        private float timer; // A timer that determines the "hunt or calm" phases

        private const float phaseDuration = 5f; // Phase duration in seconds

        public Map Map
        {
            get { return map; }
        }
        Map map;

        // Killer position
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

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

        public Killer(Map map, Vector2 position)
        {
            isHunting = true;
            isResting = false;
            direct = 1;

            this.map = map;

            LoadContent();

            Reset(position);
        }

        public void LoadContent()
        {
            idleAnimation = new Animation(Map.Content.Load<Texture2D>("Sprites/Killer/Idle"), 0.1f, true);

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
            sprite.PlayAnimation(idleAnimation);
        }

        public void Update(GameTime gameTime)
        {
            PhaseVariation(gameTime);

            //HandleCollisions();
        }

        private void StartHunting()
        {
            isHunting = true;
            isResting = false;
        }

        private void StartResting()
        {
            isHunting = false;
            isResting = true;
        }

        private void RestPhase(GameTime gameTime)
        {
            position.X -= killerSpeed * direct * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (position.X <= 32 || position.X >= 768)
            {
                direct *= -1;
            }
        }

        private void HuntPhase(Vector2 playerPos, Vector2 killerPos, GameTime gameTime)
        {
            Vector2 directionTo = Vector2.Normalize(playerPos - killerPos);
            position += directionTo * killerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        private void PhaseVariation(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= phaseDuration)
            {
                if (lastHuntPhase)
                {
                    StartResting();
                }
                else
                {
                    StartHunting();
                }
                timer = 0f;
            }

            if (isHunting)
            {
                lastHuntPhase = true;
                HuntPhase(Map.Player.Position, position, gameTime);
            }

            else if (isResting)
            {
                lastHuntPhase = false;
                RestPhase(gameTime);
            }
        }

       //private void Collisions()
       // {
            
       // }

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

                        if (collision == TileCollision.Wall || collision == TileCollision.Door)
                        {
                            Position = new Vector2(Position.X + depth.X, Position.Y + depth.Y);
                            canMove = false;
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch, Position, flip);
            spriteBatch.DrawCircle(position, 10, 20, Color.Red);
        }
    }
}