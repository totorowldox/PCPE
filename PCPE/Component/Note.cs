using PCPE.Serialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPE.Component
{
    public record struct Note
    {
        public RpeChartData.RpeNoteSet NoteData;
        public Note(RpeChartData.RpeNoteSet noteData)
        {
            NoteData = noteData;
        }
    }
}
