using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPE.Engine
{
    class SmartFramerate
    {
        double currentFrametimes;
        double weight;
        int numerator;

        public double framerate
        {
            get
            {
                return (numerator / currentFrametimes);
            }
        }

        public SmartFramerate(int oldFrameWeight)
        {
            numerator = oldFrameWeight;
            weight = (double)oldFrameWeight / ((double)oldFrameWeight - 1d);
        }

        public void Update(double timeSinceLastFrame)
        {
            currentFrametimes = currentFrametimes / weight;
            currentFrametimes += timeSinceLastFrame;
        }
    }
}
