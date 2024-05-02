using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace BreastVMATAutomation.Model.ESAPI
{
    class BeamParameterCalculations
    {
        public static double PercentageAreaComparingBody(Beam beam, Structure body, Structure comparison)
        {
            var outlineBody = beam.GetStructureOutlines(body, true);
            var outlineComparison = beam.GetStructureOutlines(comparison, true);

            var pointListBody = ConvertJaggedPointArrayToPointList(outlineBody);
            var pointListComparison = ConvertJaggedPointArrayToPointList(outlineComparison);
            double value = pointListComparison.Count;
            double value2 = 1;
            return value / value2;


        }
        public static List<Point> ConvertJaggedPointArrayToPointList(Point[][] BEVcontour)
        {
            List<Point> pointList = new List<Point>();
            foreach (var p in BEVcontour)
            {
                foreach (var t in p)
                {
                    pointList.Add(t);
                }
            }
            return pointList;
        }

        public static double GetXColimatorSize(Beam x)
        {
            return (Math.Abs(x.ControlPoints.First().JawPositions.X1) + Math.Abs(x.ControlPoints.First().JawPositions.X2)) ;

        }

        public static int PointsInsideJaw(List<Point> points, Beam beam)
        {
            var jawpos = beam.ControlPoints.First().JawPositions;
            return points.Where(e => e.X > jawpos.X1).Count();
        }
    }
}
