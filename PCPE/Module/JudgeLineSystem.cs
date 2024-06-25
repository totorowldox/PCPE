using Flecs.NET.Core;
using Microsoft.Xna.Framework;
using PCPE.Component;
using PCPE.Engine;
using PCPE.Serialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PCPE.Module
{
    public class JudgeLineSystem : IFlecsModule
    {

        public void InitModule(ref World world)
        {
            world.Module<JudgeLineSystem>("JudgeLineSystem");

            world.Routine<JudgeLine, Transform, SpriteRenderer>("JudgeLineUpdateSystem")
                .Each((ref JudgeLine line, ref Transform transform, ref SpriteRenderer sr) =>
                {
                    _currentTime = (float)GamePlay.Instance.CurrentTime;
                    line.FloorPosition = line.LineData.CalculateNoteHeight(_currentTime);
                    UpdateEventLayers(line.LineData, ref transform, ref sr);
                    UpdateExtendedState(line.LineData, ref transform, ref sr);
                });
            world.Routine<JudgeLine>("GenerateNoteSystem").Kind(Ecs.PreUpdate)
                .Each((Entity e, ref JudgeLine line) =>
            {
                for (int i = 0; i < Math.Min(5000, line.LineData.notes.Count); i++)
                {
                    RpeChartData.RpeNoteSet n = line.LineData.notes[i];
                    if (Math.Abs(line.FloorPosition - n.floorPosition) <= 30f)
                    {
                        GamePlay.Instance.GenerateNote(e, line.LineData.notes, i);
                    }
                }
            });
        }

        private float _currentTime = 0f;
        
         private void UpdateEventLayers(RpeChartData.RpeJudgeLineSet line, ref Transform transform, ref SpriteRenderer sr)
        {
            var x = 0f;
            var y = 0f;
            var rotation = 0f;
            var alpha = 0f;

            for (var i = 0; i < line.eventLayers.Count; i++)
            {
                if (line.eventLayers[i] == null)
                {
                    break;
                }
                x += UpdateValue(line.eventLayers[i].moveXEvents);
                y += UpdateValue(line.eventLayers[i].moveYEvents);
                rotation += UpdateValue(line.eventLayers[i].rotateEvents);
                alpha += UpdateValue(line.eventLayers[i].alphaEvents);
            }

            // if (Line.father > -1)
            // {
            //     var t = GlobalSetting.lines[Line.father].MoveEventValue;
            //     var r = GlobalSetting.lines[Line.father].RotateEventValue;
            //     var matrix = Matrix4x4.TRS(t, Quaternion.Euler(0, 0, r), Vector3.one);
            //     t = matrix.MultiplyPoint3x4(MoveEventValue);
            //     transform1.position = t;
            //     UIMove = t;
            // }
            // else
            // {
            transform.LocalPosition = new Vector2((-x) * 1600, (-y) * 900);
            // UIMove = MoveEventValue;
            // }


            transform.LocalRotation = rotation;
            sr.Alpha = 1f * alpha / 255f;

            // if (alphaVal < 0 && AlphaExtension == AlphaExtendMode.VisibleAll)
            // {
            //     AlphaExtension = AlphaExtendMode.InvisibleAll;
            // }
            //
            // if (isUILine)
            // {
            //     AlphaEventValue = alphaVal;
            //     alphaVal = 0;
            // }
        }

        private float UpdateValue(List<RpeChartData.RpeValueSet> line)
        {
            if (line.Count == 0)
                return 0;
            var i = GameUtils.GetEventFromCurrentTime(line, _currentTime);

            i.startTimeSeconds = i.startTimeSeconds < 0 ? 0 : i.startTimeSeconds;
            i.endTimeSeconds = i.endTimeSeconds > 10000000 ? i.startTimeSeconds + 1 : i.endTimeSeconds;

            var y = EaseUtils.GetEaseResult((EaseUtils.EaseType)i.easingType,
                _currentTime - i.startTimeSeconds, i.endTimeSeconds - i.startTimeSeconds,
                i.start, i.end, i.easingLeft, i.easingRight);
            return y;
        }

        private void UpdateExtendedState(RpeChartData.RpeJudgeLineSet line, ref Transform transform, ref SpriteRenderer sr)
        {
            if (line.extended == null) return;

            float UpdateScaleX()
            {
                int easeType = 0;

                if (line.extended.scaleXEvents.Count == 0)
                    return 1;

                var i = GameUtils.GetEventFromCurrentTime(line.extended.scaleXEvents, _currentTime);
                i.startTimeSeconds = i.startTimeSeconds < 0 ? 0 : i.startTimeSeconds;
                i.endTimeSeconds = i.endTimeSeconds > 10000000 ? i.startTimeSeconds + 1 : i.endTimeSeconds;

                var a = EaseUtils.GetEaseResult((EaseUtils.EaseType)easeType,
                    _currentTime - i.startTimeSeconds, i.endTimeSeconds - i.startTimeSeconds,
                    i.start, i.end);
                return a;
            }

            float UpdateScaleY()
            {
                int easeType = 0;

                if (line.extended.scaleYEvents.Count == 0)
                    return 1;

                var i = GameUtils.GetEventFromCurrentTime(line.extended.scaleYEvents, _currentTime);
                i.startTimeSeconds = i.startTimeSeconds < 0 ? 0 : i.startTimeSeconds;
                i.endTimeSeconds = i.endTimeSeconds > 10000000 ? i.startTimeSeconds + 1 : i.endTimeSeconds;

                var a = EaseUtils.GetEaseResult((EaseUtils.EaseType)easeType,
                    _currentTime - i.startTimeSeconds, i.endTimeSeconds - i.startTimeSeconds,
                    i.start, i.end);
                return a;
            }

            Color UpdateColor()
            {
                int easeType = 0;

                if (line.extended.colorEvents.Count == 0)
                    return Color.White;

                var i = GameUtils.GetEventFromCurrentTime(line.extended.colorEvents, _currentTime);
                i.startTimeSeconds = i.startTimeSeconds < 0 ? 0 : i.startTimeSeconds;
                i.endTimeSeconds = i.endTimeSeconds > 10000000 ? i.startTimeSeconds + 1 : i.endTimeSeconds;


                //TODO: too much calls for ease
                byte r = (byte)EaseUtils.GetEaseResult((EaseUtils.EaseType)easeType,
                    _currentTime - i.startTimeSeconds, i.endTimeSeconds - i.startTimeSeconds,
                    i.start[0], i.end[0]);
                byte g = (byte)EaseUtils.GetEaseResult((EaseUtils.EaseType)easeType,
                    _currentTime - i.startTimeSeconds, i.endTimeSeconds - i.startTimeSeconds,
                    i.start[1], i.end[1]);
                byte b = (byte)EaseUtils.GetEaseResult((EaseUtils.EaseType)easeType,
                    _currentTime - i.startTimeSeconds, i.endTimeSeconds - i.startTimeSeconds,
                    i.start[2], i.end[2]);
                return new Color(r, g, b);
            }

            string UpdateCustomText()
            {
                string targetStr = "ThisIsNULLText";
                if (line.extended.textEvents.Count > 0)
                {
                    var i = GameUtils.GetEventFromCurrentTime(line.extended.textEvents, _currentTime);

                    float startT = i.startTimeSeconds;
                    float endT = i.endTimeSeconds;
                    string start = i.start;
                    string end = i.end;
                    var now = EaseUtils.GetEaseResult((EaseUtils.EaseType)i.easingType, _currentTime - startT,
                        endT - startT,
                        0, 1);
                    start ??= "";
                    end ??= "";
                    if (start.Contains("%P%") && end.Contains("%P%"))
                    {
                        float startNum = float.Parse(start.Replace("%P%", "").Trim());
                        float endNum = float.Parse(end.Replace("%P%", "").Trim());
                        float targetNum = startNum + (endNum - startNum) * now; // 1,-1 => 1 + (-1-1) * n
                        targetStr = (start.Contains('.') || end.Contains('.'))
                            ? $"{targetNum:F3}"
                            : $"{(int)Math.Round(targetNum)}";
                    }
                    else
                    {
                        if (end.Length >= start.Length)
                        {
                            int deltaLength = (int)((end.Length - start.Length) * Math.Round(now));
                            int deltaPoint = start.Length + deltaLength;
                            if (end.StartsWith(start))
                                targetStr = end.Substring(0, deltaPoint);
                            else if (end.EndsWith(start))
                                targetStr = end.Substring(end.Length - deltaPoint - 1, deltaPoint);
                            else targetStr = now >= 0.5f ? end : start;
                        }
                        else
                        {
                            int deltaLength = (int)((start.Length - end.Length) * Math.Round(now));
                            int deltaPoint = start.Length - deltaLength;
                            if (start.StartsWith(end))
                                targetStr = start.Substring(0, deltaPoint);
                            else if (start.EndsWith(end))
                                targetStr = start.Substring(deltaLength, deltaPoint);
                            else targetStr = now >= 0.5f ? end : start;
                        }
                    }
                }

                return targetStr;
            }

            var targetColor = UpdateColor();

            transform.LocalScale = new Vector2(UpdateScaleX(), UpdateScaleY());

            sr.Color = targetColor;

            var str = UpdateCustomText();
            if (str != "ThisIsNULLText")
            {
                sr.Color = Color.White;
                sr.Alpha = 0;
                //customText.text = str;
                //customText.transform.localScale = ScaleEventScale;
            }
        }

    }
}
