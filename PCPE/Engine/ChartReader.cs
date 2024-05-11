using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PCPE.Serialized;
using Utf8Json;
using static PCPE.Serialized.RpeChartData;

namespace PCPE.Engine
{
    public static class ChartReader
    {
        public static RpeChartData ArrangeChart(string chart)
        {
            RpeChartData retChart = JsonSerializer.Deserialize<RpeChartData>(chart);
            var bpms = new List<BpmEvent>();

            //Convert meta
            retChart.META.numOfNotes = retChart.judgeLineList
                .Where(x => x.notes != null)
                .Sum(x => x.notes
                    .Where(y => y.isFake != 1)
                    .ToArray().Length);

            //Convert BPM
            retChart.BPMList.OrderBy(x => x.startTime.Frac()).ToList().ForEach(x =>
            {
                bpms.Add(new BpmEvent(x.bpm, x.startTime.Frac()));
                if (bpms.Count >= 2)
                {
                    bpms[^2].end = bpms[^1].start;
                }
            });

            //Convert lines
            
            for (var i = 0; i < retChart.judgeLineList.Count; i++)
            {
                var attachUiFlag = retChart.judgeLineList[i].attachUI != "**tHiSisnOne AtTaCH U_i TEmPlAtE**";
                if (attachUiFlag)
                {
                    retChart.judgeLineList[i].attachUI = retChart.judgeLineList[i].attachUI;
                }

                //Convert layers
                for (var j = 0; j < retChart.judgeLineList[i].eventLayers.Count; j++)
                {
                    if (retChart.judgeLineList[i].eventLayers[j] == null)
                    {
                        break;
                    }
                    
                    //Convert speed
                    foreach (var e in retChart.judgeLineList[i].eventLayers[j].speedEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }
                    
                    retChart.judgeLineList[i].eventLayers[j].speedEvents = retChart.judgeLineList[i].eventLayers[j]
                        .speedEvents.OrderBy(x => x.startTimeSeconds).ToList();

                    var temp = new List<RpeSpeedEvent>();
                    for (int k = 1; k < retChart.judgeLineList[i].eventLayers[j].speedEvents.Count; k++)
                    {
                        temp.Add(retChart.judgeLineList[i].eventLayers[j].speedEvents[k - 1]);
                        if (retChart.judgeLineList[i].eventLayers[j].speedEvents[k].startTimeSeconds >
                            retChart.judgeLineList[i].eventLayers[j].speedEvents[k - 1].endTimeSeconds)
                        {
                            temp.Add(new()
                            {
                                endTimeSeconds = retChart.judgeLineList[i].eventLayers[j].speedEvents[k].startTimeSeconds,
                                end = retChart.judgeLineList[i].eventLayers[j].speedEvents[k - 1].end,
                                start = retChart.judgeLineList[i].eventLayers[j].speedEvents[k - 1].end,
                                startTimeSeconds = retChart.judgeLineList[i].eventLayers[j].speedEvents[k - 1].endTimeSeconds
                            });
                        }
                    }

                    if (retChart.judgeLineList[i].eventLayers[j].speedEvents.Count != 0)
                    {
                        temp.Add(retChart.judgeLineList[i].eventLayers[j].speedEvents[^1]);
                    }

                    if (temp.Count != 0)
                    {
                        temp.Add(new RpeSpeedEvent
                        {
                            start = temp[^1].end,
                            end = temp[^1].end,
                            startTimeSeconds = temp[^1].endTimeSeconds,
                            endTimeSeconds = 1e9f,
                        });
                    }

                    retChart.judgeLineList[i].eventLayers[j].speedEvents = temp;

                    //Convert alpha
                    foreach (var e in retChart.judgeLineList[i].eventLayers[j].alphaEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }

                    retChart.judgeLineList[i].eventLayers[j].alphaEvents = retChart.judgeLineList[i].eventLayers[j]
                        .alphaEvents.OrderBy(x => x.startTimeSeconds).ToList();
                    
                    //Convert rotation
                    foreach (var e in retChart.judgeLineList[i].eventLayers[j].rotateEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }

                    retChart.judgeLineList[i].eventLayers[j].rotateEvents = retChart.judgeLineList[i].eventLayers[j]
                        .rotateEvents.OrderBy(x => x.startTimeSeconds).ToList();
                    
                    //Convert moveX
                    foreach (var e in retChart.judgeLineList[i].eventLayers[j].moveXEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                        e.start = -e.start / 675f / 2f;
                        e.end = -e.end / 675f / 2f;
                    }

                    retChart.judgeLineList[i].eventLayers[j].moveXEvents = retChart.judgeLineList[i].eventLayers[j]
                        .moveXEvents.OrderBy(x => x.startTimeSeconds).ToList();
                    
                    //Convert moveY
                    foreach (var e in retChart.judgeLineList[i].eventLayers[j].moveYEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                        e.start = e.start / 450f / 2f;
                        e.end = e.end / 450f / 2f;
                    }

                    retChart.judgeLineList[i].eventLayers[j].moveYEvents = retChart.judgeLineList[i].eventLayers[j]
                        .moveYEvents.OrderBy(x => x.startTimeSeconds).ToList();
                }

                //Convert notes
                for (var j = 0; j < retChart.judgeLineList[i].notes.Count; j++)
                {
                    var t = retChart.judgeLineList[i].notes[j];
                    t.positionX = -t.positionX / 675f / 2f;
                    t.startTimeSeconds = RecalcTime(bpms, t.startTime.Frac());
                    t.endTimeSeconds = RecalcTime(bpms, t.endTime.Frac());
                }

                if (retChart.judgeLineList[i].extended == null)
                {
                    continue;
                }

                if (retChart.judgeLineList[i].extended.scaleXEvents != null)
                {

                    foreach (var e in retChart.judgeLineList[i].extended.scaleXEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }
                }

                if (retChart.judgeLineList[i].extended.scaleYEvents != null)
                {
                    foreach (var e in retChart.judgeLineList[i].extended.scaleYEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }
                }

                if (retChart.judgeLineList[i].extended.colorEvents != null)
                {
                    foreach (var e in retChart.judgeLineList[i].extended.colorEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }
                }

                if (retChart.judgeLineList[i].extended.textEvents != null)
                {
                    foreach (var e in retChart.judgeLineList[i].extended.textEvents)
                    {
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }
                }

                if (retChart.judgeLineList[i].extended.inclineEvents != null)
                {
                    foreach (var e in retChart.judgeLineList[i].extended.inclineEvents)
                    {
                        e.easingType = e.easingType == 0 ? 1 : e.easingType;
                        e.startTimeSeconds = RecalcTime(bpms, e.startTime.Frac());
                        e.endTimeSeconds = RecalcTime(bpms, e.endTime.Frac());
                    }
                }

            }

            var bucket = new Dictionary<int, int>();
            foreach (var t in retChart.judgeLineList)
            {
                if (!bucket.ContainsKey(t.zOrder))
                {
                    bucket.Add(t.zOrder, 1);
                }
                else
                {
                    bucket[t.zOrder]++;
                }
            }

            var maximumZOrder = 0;

            foreach (var i in bucket)
            {
                maximumZOrder = Math.Max(maximumZOrder, i.Value);
            }

            retChart.MaxZIndex = maximumZOrder;

            return retChart;
        }

        private static float RecalcTime(List<BpmEvent> bpms, float time)
        {
            var timePhi = 0f;
            foreach (var i in bpms)
            {
                if (time > i.end)
                {
                    timePhi += (i.end - i.start) * (60f / i.bpm);
                }
                else if (time >= i.start)
                {
                    timePhi += (time - i.start) * (60f / i.bpm);
                }
            }

            return timePhi;
        }

        private class BpmEvent
        {
            public float bpm;
            public float end;
            public float start;

            public BpmEvent(float b, float s)
            {
                bpm = b;
                start = s;
                end = 1e9f;
            }
        }
    }
}