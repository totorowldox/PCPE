using Flecs.NET.Core;
using Microsoft.Xna.Framework;
using PCPE.Component;
using System;

namespace PCPE.Module
{
    public class TransformSystem : IFlecsModule
    {
        public void InitModule(ref World world)
        {
            world.Module<TransformSystem>();
            world.Routine<Transform, Transform>()
                .TermAt(2).Parent()
                .Cascade()
                .Optional()
                .Iter((Iter it, Column<Transform> child, Column<Transform> parent) =>
                {
                    foreach(var i in it)
                    {
                        if (!parent.IsNull)
                        {
                            var t = MathHelper.ToRadians(parent[i].Rotation);
                            child[i].Position = parent[i].Position;
                            child[i].Rotation = child[i].LocalRotation;
                            child[i].Position += new Vector2(-child[i].LocalPosition.X * MathF.Cos(t) + child[i].LocalPosition.Y * MathF.Sin(t)
                                                            ,-child[i].LocalPosition.Y * MathF.Cos(t) - child[i].LocalPosition.X * MathF.Sin(t));
                            child[i].Rotation += parent[i].Rotation;
                        }
                        else
                        {
                            child[i].Position = child[i].LocalPosition;
                            child[i].Rotation = child[i].LocalRotation;
                        }
                    }
                });
        }
    }
}
