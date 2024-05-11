using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PCPE.Engine;

namespace PCPE.Serialized
{

    [Serializable]
    public class RpeChartData // RPE JSON Chart
    {
        public List<RpeBpmList> BPMList = new();
        public RpeMeta META = new();
        public int MaxZIndex;
        public List<RpeJudgeLineSet> judgeLineList = new();

        [Serializable]
        public class RpeBpmList
        {
            public float bpm;
            public int[] startTime = new int[0];
        }

        [Serializable]
        public class RpeMeta
        {
            public int RPEVersion = -1;
            public string background;
            public string charter;
            public string composer;
            public string name;
            public int offset;
            public int numOfNotes;
            public string song;
        }

        [Serializable]
        public class RpeJudgeLineSet
        {
            public string Texture;
            public string attachUI = "**tHiSisnOne AtTaCH U_i TEmPlAtE**";
            public int numOfNotes;
            public List<RpeEventLayer> eventLayers = new();
            public RpeEventLayerExtended extended;
            public List<RpeNoteSet> notes = new();
            public int father = -1;
            public int zOrder = 0;
            public int isCover = 1;
            public List<RpePosControl> posControl = new();
            public List<RpeSizeControl> sizeControl = new();
            public List<RpeSkewControl> skewControl = new();
            public List<RpeYControl> yControl = new();
            public List<RpeAlphaControl> alphaControl = new();

            public void ArrangeFloorPosition()
            {
                var ret = 0d;
                foreach (var layer in eventLayers)
                {
                    ret = 0d;
                    if (layer == null)
                    {
                        continue;
                    }
                    foreach (var k in layer.speedEvents)
                    {
                        k.floorPosition = ret;
                        ret += (k.start + k.end) * (k.endTimeSeconds - k.startTimeSeconds) * .5;
                    }
                }

                foreach (var note in notes)
                {
                    double t = CalculateNoteHeight_internal(note.startTimeSeconds);
                    note.floorPosition = t;
                }
            }

            public float CalculateNoteHeight(float time)
            {
                return (float)(CalculateNoteHeight_internal(time));
            }

            private double CalculateNoteHeight_internal(float time)
            {
                var ret = 0d;
                foreach (var layer in eventLayers)
                {
                    if (layer == null)
                    {
                        continue;
                    }
                    var e = GameUtils.GetEventFromCurrentTime(layer.speedEvents, time);
                    if (e == null) continue;
                    ret += e.floorPosition;
                    ret += (e.start * 2 + (e.end - e.start) * (time - e.startTimeSeconds) / (e.endTimeSeconds - e.startTimeSeconds)) *
                           (time - e.startTimeSeconds) * .5;
                }
                return ret;
            }
        }

        [Serializable]
        public class RpeNoteSet
        {
            public int above; // 1 above; otherwise below
            public int alpha = 255;
            public int[] endTime = new int[0];
            public int isFake;
            public float positionX;
            public float size;
            public float speed;
            public int[] startTime = new int[0];
            public int type; // 1 tap 2 hold 3 flick 4 drag
            public float visibleTime;
            public float yOffset;
            public float endTimeSeconds;
            public float startTimeSeconds;
            public double floorPosition;
        }

        [Serializable]
        public class RpeEventLayer
        {
            public List<RpeValueSet> alphaEvents = new();
            public List<RpeValueSet> moveXEvents = new();
            public List<RpeValueSet> moveYEvents = new();
            public List<RpeValueSet> rotateEvents = new();
            public List<RpeSpeedEvent> speedEvents = new();
        }

        [Serializable]
        public class RpeEventLayerExtended
        {
            public List<RpeValueSet> scaleXEvents = new();
            public List<RpeValueSet> scaleYEvents = new();
            public List<RpeColorEvent> colorEvents = new();
            public List<RpeValueSet> paintEvents = new();
            public List<RpeTextEvent> textEvents = new();
            public List<RpeValueSet> inclineEvents = new();
        }

        [Serializable]
        public class RpeValueSet
        {
            public float easingLeft = 0;
            public float easingRight = 1;
            public int easingType = 0;
            public float end;
            public int[] endTime = new int[0];
            public float endTimeSeconds;
            public float start;
            public int[] startTime = new int[0];
            public float startTimeSeconds;
        }

        [Serializable]
        public class RpeSpeedEvent : RpeValueSet
        {
            public double floorPosition;
        }

        [Serializable]
        public class RpeColorEvent
        {
            public int easingType;
            public int[] end = new int[0];
            public int[] endTime = new int[0];
            public int[] start = new int[0];
            public int[] startTime = new int[0];
            public float endTimeSeconds;
            public float startTimeSeconds;
        }

        [Serializable]
        public class RpeTextEvent
        {
            public int easingType;
            public string end;
            public int[] endTime = new int[0];
            public string start;
            public int[] startTime = new int[0];
            public float endTimeSeconds;
            public float startTimeSeconds;
        }

        [Serializable]
        public class RpePosControl : RpeNoteControl
        {
            public float pos;
        }

        [Serializable]
        public class RpeSizeControl : RpeNoteControl
        {
            public float size;
        }

        [Serializable]
        public class RpeSkewControl : RpeNoteControl
        {
            public float skew;
        }

        [Serializable]
        public class RpeYControl : RpeNoteControl
        {
            public float y;
        }

        [Serializable]
        public class RpeAlphaControl : RpeNoteControl
        {
            public float alpha;
        }

        [Serializable]
        public class RpeNoteControl
        {
            public float x;
            public int easing;
        }
    }
    
    public static class ChartUtility
    {
        public static float Frac(this int[] frac)
        {
            if (frac.Length == 3) return frac[0] + (float)frac[1] / frac[2];
            return frac[0];
        }
    }
}