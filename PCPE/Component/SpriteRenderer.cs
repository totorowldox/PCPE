using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PCPE.Component
{
    public record struct SpriteRenderer
    {
        public Color Color;
        public float Alpha;
        public Texture2D Texture;
    }
}