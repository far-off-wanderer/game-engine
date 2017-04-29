using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Game
{
    public class RenderPipeline
    {
        GraphicsDevice graphicsDevice;
        DrawFullscreenQuadWithShader fullscreen;
        GBuffer gbuffer;
        Effect shaders;

        //SpriteBatch spriteBatch; // why the fuck do i need it? i don't know yet..

        public RenderPipeline(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.graphicsDevice = graphicsDevice;
            this.fullscreen = new DrawFullscreenQuadWithShader(graphicsDevice);
            this.gbuffer = new GBuffer(graphicsDevice);
            this.shaders = content.Load<Effect>("RenderPipeline\\Shaders");

            //this.spriteBatch = new SpriteBatch(graphicsDevice); // i don't want this
        }

        public void Draw(Action<Effect> drawScene)
        {
            gbuffer.SetTarget();
            shaders.CurrentTechnique.Passes["SceneToGBuffer"].Apply();
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            drawScene(shaders);
            graphicsDevice.SetRenderTargets();

            fullscreen.Draw(shaders, "GBufferToScreen", p =>
            {
                p["GBufferAlbedo"].SetValue(gbuffer.Albedo);
                p["GBufferNormal"].SetValue(gbuffer.Normal);
                p["GBufferDistance"].SetValue(gbuffer.Distance);

                return new Action(() =>
                {
                    p["GBufferAlbedo"].SetValue((Texture2D)null);
                    p["GBufferNormal"].SetValue((Texture2D)null);
                    p["GBufferDistance"].SetValue((Texture2D)null);
                });
            });
        }
    }

    class DrawFullscreenQuadWithShader
    {
        GraphicsDevice graphicsDevice;
        VertexPositionTexture[] fullscreenVertices;
        int[] fullscreenIndicees = new int[] { 0, 1, 2, 2, 3, 0 };
        (int width, int height) screenSize;

        public DrawFullscreenQuadWithShader(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public Vector2 ScreenSize => new Vector2(screenSize.width, screenSize.height);

        public void Draw(Effect effect, string passName, Func<EffectParameterCollection, Action> withParameters)
        {
            ResizeIfNeeded();
            var whenDone = withParameters(effect.Parameters);
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            if (passName != null)
            {
                effect.CurrentTechnique.Passes[passName].Apply();
            }
            else
            {
                effect.CurrentTechnique.Passes[0].Apply();
            }
            graphicsDevice.DrawUserIndexedPrimitives(
                primitiveType: PrimitiveType.TriangleList,
                vertexData: fullscreenVertices,
                vertexOffset: 0,
                numVertices: 4,
                indexData: fullscreenIndicees,
                indexOffset: 0,
                primitiveCount: 2
            );
            whenDone();
        }

        void ResizeIfNeeded()
        {
            var currentScreenSize = (width: graphicsDevice.DisplayMode.Width, height: graphicsDevice.DisplayMode.Height);
            if (screenSize.width != currentScreenSize.width || screenSize.height != currentScreenSize.height)
            {
                screenSize = currentScreenSize;

                fullscreenVertices = new VertexPositionTexture[]
                {
                    new VertexPositionTexture(Vector3.Zero, Vector2.Zero),
                    new VertexPositionTexture(Vector3.UnitX, Vector2.UnitX),
                    new VertexPositionTexture(Vector3.UnitX + Vector3.UnitY, Vector2.UnitX + Vector2.UnitY),
                    new VertexPositionTexture(Vector3.UnitY, Vector2.UnitY)
                };
            }
        }
    }
}
