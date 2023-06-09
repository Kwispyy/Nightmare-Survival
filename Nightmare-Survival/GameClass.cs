﻿namespace Nightmare_Survival
{
    public class GameClass : Game
    {
        #region Basic elements
        //For drawing
        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Vector2 baseScreenSize = new(800, 480);
        private Matrix globalTransformation;
        int backbufferWidth, backbufferHeight;

        //For UI !TEST
        private UI ui;

        private string welcomeText = 
            "Rules:" +
            "\nDon't let the killer catch you!" +
            "\nTo win the game, you must survive the timer or kill the ghost." +
            "\nYou can save money for upgrades while lying on the bed." +
            "\n" +
            "\nControls:" +
            "\nWASD - move" +
            "\n1,2 - buttons to buy upgrades (see top right corner)" +
            "\nF1,F2,F3,F4 - change the resolution" +
            "\nF11/F12 - not full/full screen" +
            "\nGood luck!" +
            "\n" +
            "\nPress 'E' to start." +
            "\nESC - exit";

        private string lossText = 
            "You lost..." +
            "\nPress 'Space' to return to the main menu.";

        private string winText = 
            "You win!" +
            "\nPress 'Space' to return to the main menu.";

        //Global
        KeyboardState keyboardState;
        readonly ushort[] widths;
        readonly ushort[] heights;
        private int screenWidth;
        int volumeFlag = 0;
        private SpriteFont hudFont;
        

        //Meta-data for map
        private int mapIndex = -1;
        private Map map;
        Song song;

        private const int numberOfLevels = 1;

        public int ScreenWidth
        {
            get { return screenWidth; }
        }

        private enum GameState
        {
            Rules, // Print rules
            Game, // Start game
            Win, // Print that player is win
            Loss, // Print that player is lost
        }

        private GameState currentState = 0;
        #endregion
        public GameClass()
        {
            

            graphics = new GraphicsDeviceManager(this);
            screenWidth = graphics.PreferredBackBufferWidth;
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

            hudFont = Content.Load<SpriteFont>("Fonts/hudFont");

            ScalePresentationArea();

            //Create ex for ui
            ui = new UI(spriteBatch, hudFont);

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
            Vector3 screenScalingFactor = new(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();

            switch (currentState)
            {
                case GameState.Rules:
                    if (keyboard.IsKeyDown(Keys.E))
                    {
                        currentState = GameState.Game;
                    }
                    break;

                case GameState.Game:
                    map.Update(gameTime, keyboardState);
                    //Update ui... need to realize !TEST
                    ui.Update();

                    if (!map.Player.IsAlive || map.Door.doorIsBroken)
                        currentState = GameState.Loss;

                    if (map.TimeRemaining.TotalMinutes <= 0 && map.TimeRemaining.TotalSeconds <= 0) // Добавить, что игра выйграна, если убийца мёртв.
                            currentState = GameState.Win;
                    break;

                case GameState.Loss:
                    if (keyboard.IsKeyDown(Keys.Space))
                    {
                        ReloadCurrentLevel();
                        currentState = GameState.Rules;
                    }
                    break;
                case GameState.Win:
                    if (keyboard.IsKeyDown(Keys.Space))
                    {
                        ReloadCurrentLevel();
                        currentState = GameState.Rules;
                    }
                    break;
            }

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
            Keys[] FunctionKeys = new Keys[] { Keys.F1, Keys.F2, Keys.F3, Keys.F4 };

            //Change between Fullscreen & windowed

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

            for (byte i = 0; i < FunctionKeys.Length; i++)
            {
                if (Keyboard.GetState().IsKeyDown(FunctionKeys[i]))
                {
                    ChangeResolution(i);
                }
            }
            #endregion

            base.Update(gameTime);
        }

        private void LoadNextMap()
        {
            // Move to the next level
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

            switch (currentState)
            {
                case GameState.Rules:
                    spriteBatch.DrawString(hudFont, welcomeText, new Vector2(baseScreenSize.X / 4, baseScreenSize.Y / 4), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    break;
                case GameState.Game:
                    map.Draw(gameTime, spriteBatch);
                    DrawHud();
                    break;
                case GameState.Loss:
                    spriteBatch.DrawString(hudFont, lossText, new Vector2(baseScreenSize.X / 2, baseScreenSize.Y / 2), Color.Red, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    break;
                case GameState.Win:
                    spriteBatch.DrawString(hudFont, winText, new Vector2(baseScreenSize.X / 2, baseScreenSize.Y / 2), Color.Green, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new(baseScreenSize.X / 2, baseScreenSize.Y / 2);

            string timeString = "TIME: " + map.TimeRemaining.Minutes.ToString("00") + ":" + map.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if ((int)map.TimeRemaining.TotalSeconds > 0)
            {
                timeColor = Color.White;
            }
            else
            {
                timeColor = Color.Green;
            }
            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "Value: " + map.Value.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.White);

            //Draw UI with upgrades !TEST
            ui.Draw();

            //Draw HP Killer and Door
            spriteBatch.DrawString(hudFont, map.Killer.GetKillerHealth.ToString() + "/" + map.Killer.GetMaxKillerHealth.ToString(), new Vector2(map.Killer.Position.X-12, map.Killer.Position.Y-46), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(hudFont, map.Door.doorHealth.ToString() + "/" + map.Door.MaxDoorHealth.ToString(), new Vector2(map.Door.DoorPointPosition.X - 12, map.Door.DoorPointPosition.Y - 28), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

            // Determine the status overlay message to show.
            Texture2D status = null;

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }

    public static class Program
    {
        private static void Main()
        {
            using var game = new GameClass();
            game.Run();
        }
    }
}