using System.Collections.Generic;
using PCPE.Serialized;

namespace PCPE.Engine
{
    public class GameUtils
    {
        public static RpeChartData.RpeValueSet GetEventFromCurrentTime(List<RpeChartData.RpeValueSet> events, float time)
        {
            //Binary search
            int l, r, length;
            length = events.Count - 1;
            l = 0;
            r = length;
            var tempIndex = 0;

            while (l <= r)
            {
                int mid = (l + r) / 2;
                if (events[mid].startTimeSeconds < time)
                {
                    if (mid < length)
                    {
                        if (events[mid + 1].startTimeSeconds >= time)
                        {
                            tempIndex = mid;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    l = mid + 1;
                }
                else if (events[mid].startTimeSeconds >= time)
                {
                    if (mid >= 1)
                    {
                        if (events[mid - 1].startTimeSeconds < time)
                        {
                            tempIndex = mid - 1;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    r = mid - 1;
                }
            }

            return events[tempIndex];
        }

        public static RpeChartData.RpeSpeedEvent GetEventFromCurrentTime(List<RpeChartData.RpeSpeedEvent> events, float time)
        {
            if (events.Count == 0)
                return null;
            if (events.Count == 1) return events[0];

            //Binary search
            int l, r, length;
            length = events.Count - 1;
            l = 0;
            r = length;
            var tempIndex = 0;

            while (l <= r)
            {
                int mid = (l + r) / 2;
                if (events[mid].startTimeSeconds < time)
                {
                    if (mid < length)
                    {
                        if (events[mid + 1].startTimeSeconds >= time)
                        {
                            tempIndex = mid;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    l = mid + 1;
                }
                else if (events[mid].startTimeSeconds >= time)
                {
                    if (mid >= 1)
                    {
                        if (events[mid - 1].startTimeSeconds < time)
                        {
                            tempIndex = mid - 1;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    r = mid - 1;
                }
            }

            return events[tempIndex];
        }

        public static RpeChartData.RpeColorEvent GetEventFromCurrentTime(List<RpeChartData.RpeColorEvent> events, float time)
        {
            //Binary search
            int l, r, length;
            length = events.Count - 1;
            l = 0;
            r = length;
            var tempIndex = 0;

            while (l <= r)
            {
                int mid = (l + r) / 2;
                if (events[mid].startTimeSeconds < time)
                {
                    if (mid < length)
                    {
                        if (events[mid + 1].startTimeSeconds >= time)
                        {
                            tempIndex = mid;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    l = mid + 1;
                }
                else if (events[mid].startTimeSeconds >= time)
                {
                    if (mid >= 1)
                    {
                        if (events[mid - 1].startTimeSeconds < time)
                        {
                            tempIndex = mid - 1;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    r = mid - 1;
                }
            }

            return events[tempIndex];
        }

        public static RpeChartData.RpeTextEvent GetEventFromCurrentTime(List<RpeChartData.RpeTextEvent> events, float time)
        {
            //Binary search
            int l, r, length;
            length = events.Count - 1;
            l = 0;
            r = length;
            var tempIndex = 0;

            while (l <= r)
            {
                int mid = (l + r) / 2;
                if (events[mid].startTimeSeconds < time)
                {
                    if (mid < length)
                    {
                        if (events[mid + 1].startTimeSeconds >= time)
                        {
                            tempIndex = mid;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    l = mid + 1;
                }
                else if (events[mid].startTimeSeconds >= time)
                {
                    if (mid >= 1)
                    {
                        if (events[mid - 1].startTimeSeconds < time)
                        {
                            tempIndex = mid - 1;
                            break;
                        }
                    }
                    else
                    {
                        tempIndex = mid;
                        break;
                    }

                    r = mid - 1;
                }
            }

            return events[tempIndex];
        }
    }
}