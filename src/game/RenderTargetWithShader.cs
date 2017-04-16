namespace Game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Linq;

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

        public Texture2D AsTexture => renderTarget;

        public void SetTarget() => graphicsDevice.SetRenderTarget(renderTarget);

        public void DrawUsingSpritebatch(SpriteBatch spriteBatch)
        {
            Set("source", p => p.SetValue(renderTarget));
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Opaque, effect: effect);
            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        void Set(string name, Action<EffectParameter> setter)
        {
            try
            {
                setter(effect.Parameters[name]);
            }
            catch (Exception)
            {
            }
        }

        protected void SetValue(string name, Texture2D value) => Set(name, p => p.SetValue(value));
        protected void SetValue(string name, bool value) => Set(name, p => p.SetValue(value));
        protected void SetValue(string name, float value) => Set(name, p => p.SetValue(value));
        protected void SetValue(string name, Vector2 value) => Set(name, p => p.SetValue(value));
    }
}
