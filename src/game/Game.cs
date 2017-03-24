namespace game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;

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
        MainToScreen mainToScreen;
 
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

            mainToScreen = Content.Load<Effect>("main_to_screen");

            angle = 0f;
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
            var speed = 0.25;
            angle = (float)(speed * gameTime.TotalGameTime.TotalSeconds * 360) % 360;

            brightness = (float)gameTime.TotalGameTime.TotalSeconds;

            title_fadein = MathHelper.SmoothStep(0, 1, MathHelper.Clamp((float)gameTime.TotalGameTime.TotalSeconds - 5, 0, 1));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mainToScreen.SetTarget();
            GraphicsDevice.Clear(new Color(2, 2, 2));

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
                }
                mesh.Draw();
            }

            /* title */
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Additive);
            spriteBatch.Draw(far_off_wanderer, Window.ClientBounds.FillHorizontally(far_off_wanderer.Width, far_off_wanderer.Height), new Color(Color.White, title_fadein));
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            mainToScreen.Brightness = brightness;
            mainToScreen.DrawUsingSpritebatch(spriteBatch);

            base.Draw(gameTime);
        }
    }

    class RenderTargetWithShader
    {
        GraphicsDevice graphicsDevice;
        Effect effect;
        RenderTarget2D renderTarget;

        public RenderTargetWithShader(Effect effect, bool hdr, bool withDepth)
        {
            this.graphicsDevice = effect.GraphicsDevice;
            this.effect = effect;
            this.renderTarget = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.DisplayMode.Width,
                graphicsDevice.DisplayMode.Height,
                false,
                hdr ? SurfaceFormat.HalfVector4 : SurfaceFormat.Color,
                withDepth ? DepthFormat.Depth24Stencil8 : DepthFormat.None
            );
        }

        public void SetTarget() => graphicsDevice.SetRenderTarget(renderTarget);

        public void DrawUsingSpritebatch(SpriteBatch spriteBatch)
        {
            effect.Parameters["source"].SetValue(renderTarget);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Opaque, effect: effect);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public void SetValue(string name, Texture2D value) => effect.Parameters[name].SetValue(value);
        public void SetValue(string name, bool value) => effect.Parameters[name].SetValue(value);
        public void SetValue(string name, float value) => effect.Parameters[name].SetValue(value);
    }

    class MainToScreen : RenderTargetWithShader
    {
        public MainToScreen(Effect effect) : base(effect, hdr: true, withDepth: true)
        {
        }

        public static implicit operator MainToScreen(Effect effect)
        {
            return new MainToScreen(effect);
        }

        public float Brightness { set => SetValue("brightness", value); }
    }
}
