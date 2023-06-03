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
        private bool collidedWithWall = false;

        private Killer_FSM brain;

        private const float killerSpeed = 80;

        private float timer; // A timer that determines the "hunt or calm" phases

        private const float PhaseDuration = 10f; // Phase duration in seconds

        int health;
        public int GetKillerHealth => health;

        int maxHealth;
        public int GetMaxKillerHealth => maxHealth;

        public Map Map
        {
            get { return map; }
        }

        readonly Map map;

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
            maxHealth = 100;
            health = maxHealth;

            Position = position;

            brain = new();

            brain.SetState(HuntPhase);

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

        public void Update(Vector2 playerPos, Vector2 killerPos, GameTime gameTime)
        {
            Vector2 previousPosition = Position;

            brain.Update(playerPos, killerPos, gameTime);

            if (CheckCollisionWithObstacles())
            {
                Position = previousPosition;
                collidedWithWall = true;
            }
        }

        private void RestPhase(Vector2 playerPos, Vector2 killerPos, GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 directionTo = Vector2.Normalize(playerPos - killerPos);
            if(Vector2.Distance(playerPos, killerPos) < 200)
            {
                position -= directionTo * killerSpeed * 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                position += directionTo * killerSpeed * 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if(timer > PhaseDuration)
            {
                brain.SetState(HuntPhase);
                timer = 0f;
            }
        }

        private void HuntPhase(Vector2 playerPos, Vector2 killerPos, GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 directionTo = Vector2.Normalize(playerPos - killerPos);
            position += directionTo * killerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= PhaseDuration)
            {
                brain.SetState(RestPhase);
                timer = 0f;
            }
        }

        private bool CheckCollisionWithObstacles()
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
                    if (collision == TileCollision.Wall)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}