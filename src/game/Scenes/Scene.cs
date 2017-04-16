namespace Game.Scenes
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public interface Scene
    {
        void Update(GameTime gameTime, MouseState mouse);
        void Draw(Effect effect);
    }
}
