using PCPE.Serialized;

namespace PCPE.Component
{
    public record struct JudgeLine
    {
        public RpeChartData.RpeJudgeLineSet LineData;
        public float FloorPosition;

        public JudgeLine(RpeChartData.RpeJudgeLineSet lineData)
        {
            LineData = lineData;
            FloorPosition = 0f;
        }
    }
}