namespace Game
{
    using global::Game.Scenes;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        RenderPipeline renderPipeline;
        List<Scene> scenes;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this)
            {
#if DEBUG
                IsFullScreen = false,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
#else
                IsFullScreen = true,
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
#endif
            };
            IsMouseVisible = false;
            Content.RootDirectory = "Content";
        }



        protected override void LoadContent()
        {
            renderPipeline = new RenderPipeline(GraphicsDevice, Content);

            scenes = new List<Scene>()
            {
                new StartScene(GraphicsDevice, Content),
                //new BasicLevelScene(GraphicsDevice, Content)
            };
        }

        protected override void Update(GameTime gameTime)
        {
            var gamePad1 = GamePad.GetState(PlayerIndex.One);
            var keyboard = Keyboard.GetState();
            var mouse    = Mouse.GetState();
            var touch    = TouchPanel.GetState();
            if (
                gamePad1.Buttons.Back == ButtonState.Pressed
                ||
                keyboard.IsKeyDown(Keys.Escape)
                ||
                mouse.LeftButton == ButtonState.Pressed
                ||
                touch.Count > 0
            )
            {
                Exit();
            }
            foreach(var scene in scenes)
            {
                scene.Update(gameTime, mouse);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            renderPipeline.Draw(effect =>
            {
                foreach(var scene in scenes)
                {
                    scene.Draw(effect);
                }
            });
            base.Draw(gameTime);
        }
    }
}
