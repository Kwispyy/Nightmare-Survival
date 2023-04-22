namespace Nightmare_Survival
{
    public class GameClass : Game
    {
        //For drawing
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Vector2 baseScreenSize = new Vector2(800, 480);
        private Matrix globalTransformation;
        int backbufferWidth, backbufferHeight;

        //Global
        KeyboardState keyboardState;
        readonly ushort[] widths;
        readonly ushort[] heights;
        int volumeFlag = 0;
        
        //Meta-data for map
        private int mapIndex = -1;
        private Map map;
        Song song;

        private const int numberOfLevels = 1;

        public GameClass()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
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

            ScalePresentationArea();

            song = Content.Load<Song>("Sounds/mainMelody");
            try
            {
                MediaPlayer.Play(song);
                MediaPlayer.Volume = 0.0f;
                MediaPlayer.IsRepeating = true;
            }
            catch { }

            LoadNextMap();
        }

        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
                backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                ScalePresentationArea();
            }

            
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                if(volumeFlag == 0)
                {
                    MediaPlayer.Volume = 0.0f;
                    volumeFlag = 1;
                }
                else
                {
                    MediaPlayer.Volume = 1.0f;
                    volumeFlag = 0;
                }
            }

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

            map.Update(gameTime, keyboardState);

            base.Update(gameTime);
        }

        private void LoadNextMap()
        {
            // move to the next level
            mapIndex = (mapIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
            if (map != null)
                map.Dispose();

            // Load the level.
            string levelPath = string.Format("Content/Map/{0}.txt", mapIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                map = new Map(Services, fileStream, mapIndex);
        }

        private void ReloadCurrentLevel()
        {
            --mapIndex;
            LoadNextMap();
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);

            map.Draw(gameTime, spriteBatch);

            //DrawHud();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}