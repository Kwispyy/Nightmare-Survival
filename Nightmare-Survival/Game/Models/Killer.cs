using Microsoft.Xna.Framework;

namespace Nightmare_Survival
{
    enum FaceDirection
    {
        Left = -1,
        Right = 1
    }

    public class Killer
    {
        Map map;
        public Map Map
        {
            get { return map; }
        }

        //Position in the space
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }

        private Rectangle localBounds;
        public Rectangle BoungingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        //Animation
        private Animation runAnimation;
        private Animation idleAnimation;
        private AnimationPlayer sprite;

        private FaceDirection direction = FaceDirection.Left;

        //How long will the killer wait before starting the hunt
        private float waitTime;

        private const float MaxWaitTime = 0.5f;

        //Killer move speed
        private const float MoveSpeed = 50.0f;

        //Construct a new killer
        public Killer(Map map, Vector2 position, string spriteSet)
        {
            this.map = map;
            this.position = position;

            LoadContent(spriteSet);
        }

        public void LoadContent(string spriteSet)
        {
            //Load animation
            spriteSet = "Sprite/" + spriteSet + "/";
            runAnimation = new Animation(Map.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, true);
            idleAnimation = new Animation(Map.Content.Load<Texture2D>(spriteSet + "Idle"), 0.2f, true);

            //Calculate bounds
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if(waitTime > 0)
            {
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if(waitTime <= 0.0f)
                {
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                if(Map.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Wall ||
                    Map.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Stop running when the game is paused or before turning around.
            if (!Map.Player.IsAlive ||
                Map.TimeRemaining == TimeSpan.Zero ||
                waitTime > 0)
            {
                sprite.PlayAnimation(idleAnimation);
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
            }

            // Draw facing the way the enemy is moving.
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}