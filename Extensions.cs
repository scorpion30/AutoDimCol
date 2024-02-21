using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDimCol
{
    public static class Extensions
    {
        public static Solid GetSymbolGeometry(this Element element, Document doc)
        {
            Solid solid;
            var options = doc.Application.Create.NewGeometryOptions();
            options.View = doc.ActiveView;
            options.ComputeReferences = true;
            var gEle = element.get_Geometry(options);
            foreach (var gObj in gEle)
            {
                if (gObj is GeometryInstance)
                {
                    var geoEle = (gObj as GeometryInstance)?.GetSymbolGeometry();
                    if (geoEle == null) { continue; }

                    foreach (var geoObj in geoEle)
                    {
                        solid = geoObj as Solid;
                        if (solid != null && solid.Faces.Size > 0) { return solid; }
                    }

                }
                else
                { solid = gObj as Solid; if (solid != null && solid.Faces.Size > 0) { return solid; } }

            }
            return null;
        }
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> KeySelector, IComparer<TKey> comparer = null)
        {
            if (comparer == null) comparer = Comparer<TKey>.Default;
            return source.ArgBy(KeySelector, lag => comparer.Compare(lag.Current, lag.Previous) < 0);
        }

        public static Line LineToZero(this Line line)
        {
            var pstart = line.GetEndPoint(0);
            var pend = line.GetEndPoint(1);
            return Line.CreateBound(new XYZ(pstart.X, pstart.Y, 0), new XYZ(pend.X, pend.Y, 0));
        }
        public static bool Approximately(this double value, double comparedValue, double tolerance = 0.00001)
            => Math.Abs(value - comparedValue) < tolerance;
        public static double Abs(this double value) => Math.Abs(value);

        public static XYZ PointToZero(this XYZ pt1)
        {
            return new XYZ(pt1.X, pt1.Y, 0);
        }

        public static XYZ GetMidPoint(this Line line)
        {
            var firstpt = line.GetEndPoint(0);
            var secondpt = line.GetEndPoint(1);
            return new XYZ((firstpt.X + secondpt.X) / 2, (firstpt.Y + secondpt.Y) / 2, (firstpt.Z + secondpt.Z) / 2);
        }

        public static TSource ArgBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> KeySelector, Func<(TKey Current, TKey Previous), bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (KeySelector == null) throw new ArgumentNullException(nameof(KeySelector));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var value = default(TSource);
            var key = default(TKey);

            if (value == null)
            {
                foreach (var other in source)
                {
                    if (other == null) continue;
                    var otherKey = KeySelector(other);
                    if (otherKey == null) continue;
                    if (value == null || predicate((otherKey, key)))
                    {
                        value = other;
                        key = otherKey;
                    }
                    return value; // Move the return statement inside the loop
                }
            }
            else
            {
                bool hasValue = false;
                foreach (var other in source)
                {
                    var otherkey = KeySelector(other);
                    if (otherkey == null) continue;

                    if (hasValue)
                    {
                        if (predicate((otherkey, key)))
                        {
                            value = other;
                            key = otherkey;
                        }
                    }
                    else
                    {
                        value = other;
                        key = otherkey;
                        hasValue = true;
                    }
                    // The return statement is not needed here
                }
                if (hasValue) return value;
                throw new InvalidOperationException("Sequence contains no elements");
            }
            // Add a return statement at the end of the method
            return value;
        }

        public static bool IsAlmostParallel(this Curve line1, Curve line2)
        {
            XYZ line1_Direction = (line1.GetEndPoint(1) - line1.GetEndPoint(0)).Normalize().RoundPoint();
            XYZ line2_Direction = (line2.GetEndPoint(1) - line2.GetEndPoint(0)).Normalize().RoundPoint();
            if (line1_Direction.IsAlmostEqualTo(line2_Direction) ||
                line1_Direction.Negate().IsAlmostEqualTo(line2_Direction))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static XYZ RoundPoint(this XYZ pt) => new XYZ(pt.X.Round(3), pt.Y.Round(3), pt.Z.Round(3));
        public static double Round(this double value, int digits) => Math.Round(value, digits);
    }
}