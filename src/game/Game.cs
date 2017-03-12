namespace game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;

        Model model;

        float angle;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true
            };
            IsMouseVisible = false;
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("conesoft entertainment presents");
            model = Content.Load<Model>("ships/main");
            angle = 0f;
        }

        protected override void Update(GameTime gameTime)
        {
            var gamePad1 = GamePad.GetState(PlayerIndex.One);
            var keyboard = Keyboard.GetState();
            if (gamePad1.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            var speed = 0.25;
            angle = (float)(speed * gameTime.TotalGameTime.TotalSeconds * 360) % 360;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            /* background */
            GraphicsDevice.Clear(new Color(35, 35, 35));
 
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(texture, Window.ClientBounds.FillHorizontally(texture.Width, texture.Height), Color.White);
            spriteBatch.End();

            /* spaceship */
            spriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp);
            spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            var aspectRatio = (float)Window.ClientBounds.Width / Window.ClientBounds.Height;
            var verticalScreen = aspectRatio < 1f;
            var translation = verticalScreen
                ?
                Matrix.CreateTranslation(12.5f, -25f, -25f)
                :
                Matrix.CreateTranslation(25f, -12.5f, 0)
                ;

            model.Draw(
                Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * translation,
                Matrix.CreateLookAt(
                    Vector3.Backward * 50,
                    Vector3.Zero,
                    Vector3.Up
                ),
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60),
                    aspectRatio,
                    1f,
                    1000f
                )
            );

            base.Draw(gameTime);
        }
    }
}
