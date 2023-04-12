namespace Nightmare_Survival
{
    public class GameClass : Game
    {
        readonly ushort[] widths;
        readonly ushort[] heights;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public GameManager _gameManager;

        SoundEffect songWav;
        Song song;

        public GameClass()
        {
            _graphics = new GraphicsDeviceManager(this);
            widths = new ushort[] { 1920, 1366, 1280, 1280 };
            heights = new ushort[] { 1080, 768, 1024, 720 };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        void ChangeResolution(byte newResolution)
        {
            if(newResolution >= 0 && newResolution < widths.Length)
            {
                _graphics.PreferredBackBufferWidth = widths[newResolution];
                _graphics.PreferredBackBufferHeight = heights[newResolution];
                _graphics.ApplyChanges();
            }
        }

        protected override void Initialize()
        {
            Globals.ContentManager = Content;
            _gameManager = new();
            _gameManager.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;

            song = Content.Load<Song>("Sounds/mainMelody");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
        {
            MediaPlayer.Volume -= 0.1f;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.Update(gameTime);
            _gameManager.Update();

            //Everything about screen resolution
            #region
            // Changing the resolution in the game + !do fullscreen / windowerd option
            #region 
            Keys[] FunctionKeys = new Keys[] { Keys.F1, Keys.F2, Keys.F3, Keys.F4 };

            //Change between Fullscreen & windowed
            #region

            if (Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                _graphics.IsFullScreen = true;
                _graphics.ApplyChanges();
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
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
            ////Player control (old ver.)
            #region
            //if (Keyboard.GetState().IsKeyDown(Keys.W))
            //{
            //    position.Y -= speed;
            //    if (position.Y < 0)
            //        position.Y = 0;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.A))
            //{
            //    position.X -= speed;
            //    if (position.X < 0)
            //        position.X = 0;
            //}

            //if (Keyboard.GetState().IsKeyDown(Keys.S))
            //{
            //    currentTime += gameTime.ElapsedGameTime.Milliseconds;

            //    if (currentTime > period)
            //    {
            //        currentTime -= period;
            //        position.Y += speed;

            //        if (position.Y > Window.ClientBounds.Height - frameHeight * 1.5)
            //        {
            //            position.Y -= speed;
            //            ++currentFrame.Y;
            //            if (currentFrame.Y >= spriteSize.Y)
            //                currentFrame.Y = 0;
            //        }
            //        ++currentFrame.X;
            //        if (currentFrame.X >= spriteSize.X)
            //        {
            //            currentFrame.X = 0;
            //        }
            //    }
            //}


            //if (Keyboard.GetState().IsKeyDown(Keys.D))
            //{
            //    position.X += speed;
            //    if (position.X > Window.ClientBounds.Width - frameWidth * 1.5)
            //        position.X -= speed;
            //}

            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            //{
            //    if(position.Y < Window.ClientBounds.Height - frameHeight - 10)
            //    {
            //        for (int i = 0; i < 10; i++)
            //        {
            //            position.Y -= 1;
            //        }

            //    }
            //}

            //if(Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            //{
            //    speed = 8;
            //}
            //else if (Keyboard.GetState().IsKeyUp(Keys.LeftShift))
            //{
            //    speed = 3;
            //}
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            _spriteBatch.Begin();
            _gameManager.Draw();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}