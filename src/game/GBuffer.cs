namespace Game
{
    using Microsoft.Xna.Framework.Graphics;
    class GBuffer
    {
        GraphicsDevice graphicsDevice;
        RenderTarget2D albedoWithDepthStencil;
        RenderTarget2D normal;

        public GBuffer(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            albedoWithDepthStencil = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.DisplayMode.Width,
                graphicsDevice.DisplayMode.Height,
                false,
                SurfaceFormat.HdrBlendable,
                DepthFormat.Depth24Stencil8
            );

           normal = new RenderTarget2D(
                graphicsDevice,
                graphicsDevice.DisplayMode.Width,
                graphicsDevice.DisplayMode.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
        }

        public void SetTarget() => graphicsDevice.SetRenderTargets(albedoWithDepthStencil, normal);

        public Texture2D Albedo => albedoWithDepthStencil;
        public Texture2D Normal => normal;
    }
}
