using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace uwp_project
{
    public interface Scene<SceneView>
    {
        void Load(Action<string> content);
        bool Update(TimeSpan timeSpan);
        SceneView View { get; }
    }

    public class View
    {
        public Color BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public float BackgroundOpacity { get; set; }
    }

    public class BootScene : Scene<View>
    {
        double runtime;
        double fade;

        public void Load(Action<string> content)
        {
            content(View.BackgroundImage);
        }

        public bool Update(TimeSpan timeSpan)
        {
            runtime += timeSpan.TotalSeconds;
            fade = (float)Math.Max(0, Math.Min(runtime - 1, Math.Min(2, 6 - (runtime - 1))));

            return runtime < 7;
        }

        public View View => new View
        {
            BackgroundColor = Color.Black,
            BackgroundImage = "scenes/boot/background",
            BackgroundOpacity = (float)fade
        };
    }

    public class TitleScene : Scene<View>
    {
        double runtime;
        double fade;

        public void Load(Action<string> content)
        {
            content(View.BackgroundImage);
        }

        public bool Update(TimeSpan timeSpan)
        {
            runtime += timeSpan.TotalSeconds;
            fade = (float)Math.Max(0, Math.Min(runtime - 1, Math.Min(2, 6 - (runtime - 1))));

            return runtime < 7;
        }

        public View View => new View
        {
            BackgroundColor = Color.Black,
            BackgroundImage = "scenes/title/background",
            BackgroundOpacity = (float)fade
        };
    }

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Scene<View>[] scenes;
        Scene<View> currentScene;
        Dictionary<string, Texture2D> textures;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";

            scenes = new Scene<View>[]
            {
                new BootScene(),
                new TitleScene()
            };
            currentScene = scenes[0];

            textures = new Dictionary<string, Texture2D>();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            currentScene.Load(content => textures.Add(content, Content.Load<Texture2D>(content)));
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Escape) || TouchPanel.GetState().Count > 0)
            {
                Exit();
            }
            if(currentScene.Update(gameTime.ElapsedGameTime) == false)
            {
                var nextScene = scenes.SkipWhile(scene => scene != currentScene).Skip(1).FirstOrDefault();
                if(nextScene == null)
                {
                    Exit();
                }
                currentScene = nextScene;
                currentScene.Load(content => textures.Add(content, Content.Load<Texture2D>(content)));
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var view = currentScene.View;
            GraphicsDevice.Clear(view.BackgroundColor);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            var texture = textures[view.BackgroundImage];
            var color = new Color(1, 1, 1, view.BackgroundOpacity);
            spriteBatch.Draw(texture, new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height) / 2 - new Vector2(texture.Width, texture.Height) / 2, color);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
