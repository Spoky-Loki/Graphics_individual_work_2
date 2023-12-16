using System;
using System.Drawing;
using System.Numerics;

namespace IND2
{
    internal class Sphere : Model
    {
        public float radius;
        public float reflection;

        public Sphere(Vector3 center, float radius, Color color, float reflection) : base(center, color)
        {
            this.radius = radius;
            this.reflection = reflection;
        }

        public static Tuple<bool, Tuple<float, float>> IntersectRaySphere(Vector3 ray_start, Vector3 ray_end, Vector3 center, float radius)
        {
            var oc = ray_start - center;

            var k1 = Vector3.Dot(ray_end, ray_end);
            var k2 = 2 * Vector3.Dot(oc, ray_end);
            var k3 = Vector3.Dot(oc, oc) - radius*radius;

            var dec = k2 * k2 - 4 * k1 * k3;
            if (dec < 0)
                return Tuple.Create(false, Tuple.Create(0f, 0f));

            var t1 = (float)(-k2 + Math.Sqrt(dec)) / (2 * k1);
            var t2 = (float)(-k2 - Math.Sqrt(dec)) / (2 * k1);

            return Tuple.Create(true, Tuple.Create(t1, t2));
        }
    }
}
