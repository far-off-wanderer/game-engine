namespace game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    class ModelWithTexture
    {
        public Model Model { get; private set; }
        public Texture2D Texture { get; private set; }

        public ModelWithTexture(Model model)
        {
            this.Model = model;
            this.Texture = ((BasicEffect)model.Meshes[0].Effects[0]).Texture;
        }


        public static implicit operator ModelWithTexture(Model model)
        {
            return new ModelWithTexture(model);
        }
    }

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D conesoft_entertainment_presents;
        Texture2D far_off_wanderer;
        Effect main;

        ModelWithTexture ship;


        float angle;
        float title_fadein;
        float brightness;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
            };
            IsMouseVisible = false;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            conesoft_entertainment_presents = Content.Load<Texture2D>("conesoft entertainment presents");
            far_off_wanderer = Content.Load<Texture2D>("far off wanderer");
            ship = Content.Load<Model>("ships/main");
            main = Content.Load<Effect>("main");
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

            brightness = (float)gameTime.TotalGameTime.TotalSeconds;

            title_fadein = MathHelper.SmoothStep(0, 1, MathHelper.Clamp((float)gameTime.TotalGameTime.TotalSeconds - 5, 0, 1));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            /* background */
            GraphicsDevice.Clear(new Color(35, 35, 35));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Additive);
            spriteBatch.Draw(conesoft_entertainment_presents, Window.ClientBounds.FillHorizontally(conesoft_entertainment_presents.Width, conesoft_entertainment_presents.Height), new Color(Color.White, 1 - title_fadein));
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

            var world = Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * translation;
            var view = Matrix.CreateLookAt(
                    Vector3.Backward * 50,
                    Vector3.Zero,
                    Vector3.Up
                );
            var projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60),
                    aspectRatio,
                    1f,
                    1000f
                );

            foreach (ModelMesh mesh in ship.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = main;
                    var worldViewProjection = (world * mesh.ParentBone.Transform) * view * projection;
                    main.Parameters["worldViewProjection"].SetValue(worldViewProjection);
                    main.Parameters["worldViewProjectionTransposed"].SetValue(Matrix.Transpose(worldViewProjection));
                    main.Parameters["modelTexture"].SetValue(ship.Texture);
                    main.Parameters["brightness"].SetValue(brightness);
                }
                mesh.Draw();
            }

            /* title */
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Additive);
            spriteBatch.Draw(far_off_wanderer, Window.ClientBounds.FillHorizontally(far_off_wanderer.Width, far_off_wanderer.Height), new Color(Color.White, title_fadein));
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
