using System;
using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Modules.Services
{
    public static class MathServices
    {
        public static double EuclidianDistance(IUnit unit1,IUnit unit2) => Math.Sqrt(Math.Pow(unit1.Pos.X - unit2.Pos.X,2) + Math.Pow(unit1.Pos.Y - unit2.Pos.Y,2));
        public static double EuclidianDistance(Point2D p1,IUnit unit) => Math.Sqrt(Math.Pow(p1.X - unit.Pos.X,2) + Math.Pow(p1.Y - unit.Pos.Y,2));
        public static double EuclidianDistance(Point p1, float x, float y) => Math.Sqrt(Math.Pow(p1.X - x,2) + Math.Pow(p1.Y - y,2));
        public static double EuclidianDistance(Point p1,Point p2) => Math.Sqrt(Math.Pow(p1.X - p2.X,2) + Math.Pow(p1.Y - p2.Y,2));
        public static double EuclidianDistance(Point2D p1,Point2D p2) => Math.Sqrt(Math.Pow(p1.X - p2.X,2) + Math.Pow(p1.Y - p2.Y,2));
        public static double EuclidianDistance(Point2D p1,float x,float y) => Math.Sqrt(Math.Pow(p1.X - x,2) + Math.Pow(p1.Y - y,2));
    }
}