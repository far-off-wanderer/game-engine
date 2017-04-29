namespace Game
{
    using Microsoft.Xna.Framework;
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

        public void Draw(Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    var worldViewProjection = (world * mesh.ParentBone.Transform) * view * projection;
                    effect.Parameters["worldViewProjection"].SetValue(worldViewProjection);
                    effect.Parameters["worldViewProjectionTransposed"].SetValue(Matrix.Transpose(worldViewProjection));
                    effect.Parameters["worldViewProjectionInverted"].SetValue(Matrix.Invert(worldViewProjection));
                    effect.Parameters["Albedo"].SetValue(Texture);
                }
                mesh.Draw();
            }
        }

        public static implicit operator ModelWithTexture(Model model)
        {
            return new ModelWithTexture(model);
        }
    }
}
