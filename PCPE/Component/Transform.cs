using Microsoft.Xna.Framework;

namespace PCPE.Component
{
    public record struct Transform
    {
        public Vector2 Position;
        public Vector2 LocalScale;
        public float Rotation;


        public Vector2 LocalPosition;
        public float LocalRotation;

        public Transform(Vector2 position, Vector2 scale, float rotation)
        {
            LocalPosition = position;
            LocalScale = scale;
            LocalRotation = rotation;
            Position = Vector2.Zero;
            Rotation = 0f;
        }
    }
}