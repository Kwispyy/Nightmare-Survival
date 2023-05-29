namespace Nightmare_Survival
{
    struct AnimationPlayer
    {
        Animation animation;
        public Animation Animation
        {
            get { return animation; }
        }

        int frameIndex;
        public int FrameIndex
        {
            get { return frameIndex; }
        }

        private float time;

        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        public void PlayAnimation(Animation animation)
        {
            if (Animation == animation)
                return;

            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                if (Animation.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % Animation.FrameCount;
                }
                else
                {
                    frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                }
            }

            Rectangle source = new(FrameIndex * Animation.FrameHeight, 0, Animation.Texture.Height, Animation.Texture.Height);

            spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}