namespace Game.Scenes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    public class BasicLevelScene : Scene
    {
        GraphicsDevice graphicsDevice;

        Spaceship spaceship;
        Effect sceneEffect;

        public BasicLevelScene(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.graphicsDevice = graphicsDevice;

            spaceship = new Spaceship(content.Load<Model>("ships/main"));
            sceneEffect = content.Load<Effect>("main");
        }

        public void Update(GameTime gameTime, MouseState mouse)
        {
            spaceship.Update(gameTime, mouse);
        }

        public void Draw(Effect effect)
        {
            var camera = (
                Position: spaceship.Position + Vector3.Backward * 10,
                Up: Vector3.Up
            );

            graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            var aspectRatio = graphicsDevice.DisplayMode.AspectRatio;
            var verticalScreen = aspectRatio < 1f;
            var translation = verticalScreen
                ?
                Matrix.CreateTranslation(12.5f, -25f, -25f)
                :
                Matrix.CreateTranslation(25f, -12.5f, 0)
                ;

            var world = Matrix.Identity;
            var view = Matrix.CreateLookAt(
                    cameraPosition: camera.Position,
                    cameraTarget: spaceship.Position,
                    cameraUpVector: camera.Up
                );
            var projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60),
                    aspectRatio,
                    1f,
                    1000f
                );

            foreach (ModelMesh mesh in spaceship.Model.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = sceneEffect;
                    var worldViewProjection = (world * mesh.ParentBone.Transform) * view * projection;
                    sceneEffect.Parameters["worldViewProjection"].SetValue(worldViewProjection);
                    sceneEffect.Parameters["worldViewProjectionTransposed"].SetValue(Matrix.Transpose(worldViewProjection));
                    sceneEffect.Parameters["albedo"].SetValue(spaceship.Model.Texture);
                    sceneEffect.Parameters["brightness"].SetValue(1f);
                }
                mesh.Draw();
            }

        }
    }

    abstract class ObjectInScene
    {
        public Vector3 Position { get; protected set; }
        public Quaternion Orientation { get; protected set; }
        public ModelWithTexture Model { get; protected set; }

        public abstract void Update(GameTime gameTime, MouseState mouse);
    }

    class Spaceship : ObjectInScene
    {
        public Spaceship(ModelWithTexture model)
        {
            this.Model = model;
            this.Position = Vector3.Zero;
            this.Orientation = Quaternion.Identity;
        }

        public override void Update(GameTime gameTime, MouseState mouse)
        {
            if(mouse.RightButton == ButtonState.Pressed)
            {
                this.Position += Vector3.Forward;
            }
        }
    }
}
