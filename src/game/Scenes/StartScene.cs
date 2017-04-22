namespace Game.Scenes
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class StartScene : Scene
    {
        GraphicsDevice graphicsDevice;

        ModelWithTexture space;
        ModelWithTexture ship;
        float angle;

        public StartScene(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.graphicsDevice = graphicsDevice;
            
            space = content.Load<Model>("scenes/spaaaaaaaaaaaace");
            ship = content.Load<Model>("ships/main");

            angle = 0f;
        }

        public void Update(GameTime gameTime, MouseState mouse)
        {
            var speed = 0.25;
            angle = (float)(speed * gameTime.TotalGameTime.TotalSeconds * 360) % 360;
        }

        public void Draw(Effect effect)
        {
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.DepthBuffer, Color.Gainsboro, 1, 0);
            
            var aspectRatio = graphicsDevice.DisplayMode.AspectRatio;
            var verticalScreen = aspectRatio < 1f;
            var translation = verticalScreen
                ?
                Matrix.CreateTranslation(12.5f, -25f, -25f)
                :
                Matrix.CreateTranslation(25f, -12.5f, 0)
                ;

            var world = Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * translation;
            var view = Matrix.CreateLookAt(
                    cameraPosition: Vector3.Backward * 50,
                    cameraTarget: Vector3.Zero,
                    cameraUpVector: Vector3.Up
                );
            var projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60),
                    aspectRatio,
                    1f,
                    1000f
                );

            space.Draw(effect, world, view, projection);
            ship.Draw(effect, world, view, projection);
        }
    }
}
