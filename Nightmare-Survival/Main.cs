using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;

namespace Nightmare_Survival
{
    public class Main : Game
    {
        readonly ushort[] widths;
        readonly ushort[] heights;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D player;
        Vector2 position = Vector2.Zero;
        float speed = 10;
        byte fullscreenMode;

        float timer;
        int threshold;
        Rectangle[] sourceRectangles;
        byte prevAnimIndex;
        byte curAnimIndex;




        public Main()
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
                Debug.WriteLine("New resolution: {0} x {1}", _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = Content.Load<Texture2D>(assetName: "player");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            timer = 0;
            threshold = 250;
            sourceRectangles = new Rectangle[8];
            sourceRectangles[0] = new Rectangle(0, 0, 32, 44);
            sourceRectangles[1] = new Rectangle(32, 0, 32, 44);
            sourceRectangles[2] = new Rectangle(64, 0, 32, 44);
            sourceRectangles[3] = new Rectangle(96, 0, 32, 44);
            sourceRectangles[4] = new Rectangle(128, 0, 32, 44);
            sourceRectangles[5] = new Rectangle(160, 0, 32, 44);
            sourceRectangles[6] = new Rectangle(192, 0, 32, 44);
            sourceRectangles[7] = new Rectangle(224, 0, 32, 44);

            prevAnimIndex = 0;
            curAnimIndex = 0;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (timer > threshold)
            {
                // If Alex is in the middle sprite of the animation.
                if (curAnimIndex == 1)
                {
                    // If the previous animation was the left-side sprite, then the next animation should be the right-side sprite.
                    if (prevAnimIndex == 0)
                    {
                        curAnimIndex = 6;
                    }
                    else
                    // If not, then the next animation should be the left-side sprite.
                    {
                        curAnimIndex = 4;
                    }
                    // Track the animation.
                    prevAnimIndex = curAnimIndex;
                }
                // If Alex was not in the middle sprite of the animation, he should return to the middle sprite.
                else
                {
                    curAnimIndex = 1;
                }
                // Reset the timer.
                timer = 0;
            }
            // If the timer has not reached the threshold, then add the milliseconds that have past since the last Update() to the timer.
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }


            // Changing the resolution in the game + !do fullscreen / windowerd option
            #region 
            Keys[] FunctionKeys = new Keys[] { Keys.F1, Keys.F2, Keys.F3, Keys.F4 };

            //Change between Fullscreen & windowed
            #region
            //fullscreenMode = 0;

            //if (Keyboard.GetState().IsKeyDown(Keys.F12))
            //{
            //    switch (fullscreenMode)
            //    {
            //        case 0:
            //            _graphics.IsFullScreen = true;
            //            _graphics.ApplyChanges();
            //            fullscreenMode = 1;
            //            break;
            //        case 1:
            //            _graphics.IsFullScreen = false;
            //            _graphics.ApplyChanges();
            //            fullscreenMode = 0;
            //            break;
            //    }
            //}
            #endregion

            for (byte i = 0; i < FunctionKeys.Length; i++)
            {
                if (Keyboard.GetState().IsKeyDown(FunctionKeys[i]))
                {
                    ChangeResolution(i);
                }
            }
            #endregion

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= speed;
                if (position.Y < 0)
                    position.Y = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= speed;
                if (position.X < 0)
                    position.X = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += speed;
                if (position.Y + player.Height > Window.ClientBounds.Height)
                    position.Y = Window.ClientBounds.Height;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += speed;
                if (position.X + player.Width > Window.ClientBounds.Width)
                    position.X = Window.ClientBounds.Width;
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(player, position, sourceRectangles[curAnimIndex],
                Color.White, 0, Vector2.Zero,
                2, SpriteEffects.None, 0);


            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }













        public static class Program
        {
            static void Main()
            {
                using var game = new Nightmare_Survival.Main();
                game.Run();
            }
        }
    }
}