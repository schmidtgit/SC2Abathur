using Abathur.Model;
using NydusNetwork.API.Protocol;
using System.Collections.Generic;

namespace Abathur.Extensions {
    public static class PointExtension {
        public static Point2D ConvertTo2D(this Point point) => new Point2D { X = point.X,Y = point.Y };

        public static T GetClosest<T>(this IPosition point,IEnumerable<T> enumerable) where T : IPosition
            => GetClosest(point.Point, enumerable);
        public static T GetClosest<T>(this Point2D point,IEnumerable<T> enumerable) where T : IPosition {
            IPosition result = null;
            double minValue = double.MaxValue; double value;
            foreach(var position in enumerable) {
                value = FastDistance(point,position.Point);
                if(value < minValue) {
                    minValue = value;
                    result = position;
                }
            }
            return (T)result;
        }

        public static double FastDistance(this Point2D point,float x, float y) {
            var deltaX = point.X - x;
            var deltaY = point.Y - y;
            return deltaX * deltaX + deltaY * deltaY;
        }
        public static double FastDistance(this Point2D point,Point2D otherPoint)
            => point.FastDistance(otherPoint.X,otherPoint.Y);

        public static double Distance(this Point2D point,float x, float y) => System.Math.Sqrt(FastDistance(point, x, y));
        public static double Distance(this Point2D point,Point2D otherPoint) => System.Math.Sqrt(FastDistance(point,otherPoint));
    }
}
