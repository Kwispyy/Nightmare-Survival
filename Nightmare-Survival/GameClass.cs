namespace Nightmare_Survival
{
    public class GameClass : Game
    {
        readonly ushort[] widths;
        readonly ushort[] heights;
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        //public GameManager gameManager;
        readonly SoundEffect songWav;
        Song song;
        Map map;

        public GameClass()
        {
            graphics = new GraphicsDeviceManager(this);
            widths = new ushort[] { 1920, 1366, 1280, 1280 };
            heights = new ushort[] { 1080, 768, 1024, 720 };
            IsMouseVisible = true;
        }

        void ChangeResolution(byte newResolution)
        {
            if(newResolution >= 0 && newResolution < widths.Length)
            {
                graphics.PreferredBackBufferWidth = widths[newResolution];
                graphics.PreferredBackBufferHeight = heights[newResolution];
                graphics.ApplyChanges();
            }
        }

        protected override void LoadContent()
        {
            this.Content.RootDirectory = "Content";

            spriteBatch = new SpriteBatch(GraphicsDevice);

            song = Content.Load<Song>("Sounds/mainMelody");
            try
            {
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //gameManager.Update();

            #region Everything about changing screen resolution
            // Changing the resolution in the game + !do fullscreen / windowerd option
            #region 
            Keys[] FunctionKeys = new Keys[] { Keys.F1, Keys.F2, Keys.F3, Keys.F4 };

            //Change between Fullscreen & windowed
            #region

            if (Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
            }
            #endregion

            for (byte i = 0; i < FunctionKeys.Length; i++)
            {
                if (Keyboard.GetState().IsKeyDown(FunctionKeys[i]))
                {
                    ChangeResolution(i);
                }
            }
            #endregion
            #endregion
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            spriteBatch.Begin(SpriteSortMode.Immediate);

            //gameManager.Draw();
            //map.Draw();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}