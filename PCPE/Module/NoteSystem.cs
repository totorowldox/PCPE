using Flecs.NET.Core;
using PCPE.Component;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPE.Module
{
    public class NoteSystem : IFlecsModule
    {
        public void InitModule(ref World world)
        {
            world.Module<NoteSystem>("NoteSystem");
            world.Routine<Note, Transform, JudgeLine>()
                .Kind(Ecs.PreUpdate)
                .TermAt(3).Parent()
                .Cascade()
                .Each((Entity e, ref Note note, ref Transform transform, ref JudgeLine line) =>
                {
                    if ((float)GamePlay.Instance.CurrentTime >= note.NoteData.endTimeSeconds)
                    {
                        //e.Destruct();
                        e.Disable();
                        GamePlay.Instance.NotePool.RecycleObject(e);
                        return;
                    }
                    var height = (float)(note.NoteData.floorPosition - line.FloorPosition + note.NoteData.yOffset)
                    * note.NoteData.speed * 90f
                    * (note.NoteData.above == 1 ? 1 : -1);
                    transform.LocalPosition.Y = height;
                });
        }
    }
}
