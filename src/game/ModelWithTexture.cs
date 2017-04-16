namespace Game
{
    using Microsoft.Xna.Framework.Graphics;
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
}
