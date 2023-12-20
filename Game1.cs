using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Cannon3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int Screen_Width = 640;
        const int Screen_Height = 480;

        const int Projectile_Speed = 12;
        const int Base_Target_Speed = 8;

        const float Collision_Distance = 20.0f;
        const int Starting_Lives = 3;
        const int Lives_Tile_Width = 16;
        const int Game_Difficulty_Control = 3;
        const int Lives_Display_Offset = 4;
        bool IsGameRunning = false;
        int Lives;
        int Level;
        GameItem player;
        GameItem target;
        GameItem projectile;
        HUDItem titleScreen;
        HUDItem livesDisplay;

        Random random;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Screen_Width;
            graphics.PreferredBackBufferHeight = Screen_Height;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            random = new Random();

            player = new GameItem();
            target = new GameItem();
            projectile = new GameItem();
            projectile.Velocity = new Point(0, -Projectile_Speed);
            titleScreen = new HUDItem();
            titleScreen.Position = new Point(0, 0);
            titleScreen.Width = Screen_Width;
            titleScreen.Height = Screen_Height;
            livesDisplay = new HUDItem();
            livesDisplay.Position = new Point(Lives_Display_Offset, Lives_Display_Offset);
            livesDisplay.Width = Lives_Tile_Width * Starting_Lives;
            livesDisplay.Height = Lives_Tile_Width;


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            player.Texture = Content.Load<Texture2D>(@"Textures\player");
            target.Texture = Content.Load<Texture2D>(@"Textures\target");
            projectile.Texture = Content.Load<Texture2D>(@"Textures\projectile");
            titleScreen.Texture = Content.Load<Texture2D>(@"Textures\title_screen");
            livesDisplay.Texture = Content.Load<Texture2D>(@"Textures\lives");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (IsGameRunning)
            {
                UpdatePlayer();
                target.Update();
                projectile.Update();
                CheckFire();
                CheckGroundCollision();
                CheckAirCollision();
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                BeginGame();
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(212, 208, 200));

            spriteBatch.Begin();
            if (IsGameRunning)
            {
                player.Draw(spriteBatch);
                target.Draw(spriteBatch);
                projectile.Draw(spriteBatch);
                livesDisplay.Draw(spriteBatch);
            }
            else
            {
                titleScreen.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Gameplay Methods

        private void BeginGame()
        {
            IsGameRunning = true;
            Lives = Starting_Lives;
            Level = 1;

            UpdateLivesDisplay();
            ResetProjectile();
            ResetTarget();
            UpdatePlayer();

            

        }

        private void EndGame()
        {
            IsGameRunning = false;

        }

        private void KillPlayer()
        {

            ResetTarget();

            Lives--;
            UpdateLivesDisplay();
            if (Lives == 0)
                EndGame();
        }

        private void KillTarget()
        {
            Level++;
            ResetTarget();
            ResetProjectile();
        }

        private void UpdateLivesDisplay()
        {
            livesDisplay.Width = Lives * Lives_Tile_Width;
        }

        #endregion

        #region Mechanics Methods

        private void UpdatePlayer()
        {
            int playerX = Mouse.GetState().X;
            int playerY = Screen_Height - player.Origin.Y;
            player.Position = new Point(playerX, playerY);
        }

        private void CheckFire()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && 
                projectile.Position.Y < projectile.Origin.Y)
            {
                int projectileX = player.Position.X;
                int projectileY = Screen_Height - player.Texture.Height - projectile.Origin.Y;
                projectile.Position = new Point(projectileX, projectileY);
            }
        }

        private void ResetTarget()
        {
            int velocityX = random.Next(2, 6);
            int velocityY = Base_Target_Speed + Level / Game_Difficulty_Control;

            if (random.Next(2) == 0)
            {
                velocityX *= -1;
            }

            target.Position = new Point(Screen_Width / 2, -target.Origin.Y);
            target.Velocity = new Point(velocityX, velocityY);
        }

        private void ResetProjectile()
        {
            projectile.Position = new Point (0, -projectile.Origin.Y);
        }

        #endregion

        #region Collision Methods

        private void CheckGroundCollision()
        {
            if (target.Position.Y > Screen_Height - target.Origin.Y)
            {
                KillPlayer();
            }
        }

        private void CheckAirCollision()
        {
            if (Distance(projectile.Position, target.Position) < Collision_Distance)
            {
                KillTarget();
            }
        }



        #endregion

        #region Utility Methods

        private float Distance(Point pointA, Point pointB)
        {
            int A = pointA.X - pointB.X;
            int B = pointA.Y - pointB.Y;
            float C = (float)Math.Sqrt((A * A) + (B * B));


            return C;
        }

        #endregion
    }
}