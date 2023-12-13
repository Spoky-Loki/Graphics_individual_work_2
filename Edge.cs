using System;
using System.Drawing;
using System.Numerics;

namespace IND2
{
    public class Edge
    {
        public int[] points_index;
        public Vector3 normal;
        public Color color;

        public Edge(int[] points_index)
        {
            this.points_index = points_index;
        }

        public Edge(int[] points_index, Color color)
        {
            this.points_index = points_index;
            this.color = color;
        }

        public Edge(int[] points__index, Vector3 normal)
        {
            this.points_index = points__index;
            this.normal = normal;
        }

        public static Tuple<bool, float> IntersectRayTriangle(Vector3 ray_start, Vector3 ray_end, Vector3[] vertex)
        {
            float EPS = 0.0001f;

            Vector3 edge1 = vertex[1] - vertex[0];
            Vector3 edge2 = vertex[2] - vertex[0];
            var ray_direction = Vector3.Normalize(ray_end - ray_start);
            Vector3 pvec = Vector3.Cross(ray_direction, edge2);
            float det = Vector3.Dot(pvec, edge1);

            if (det > -EPS && det < EPS)
                return Tuple.Create(false, 0f);

            float inv_det = 1.0f / det;
            Vector3 tvec = ray_start - vertex[0];
            float u = inv_det * Vector3.Dot(tvec, pvec);

            if (u < 0 || u > 1)
                return Tuple.Create(false, 0f);

            Vector3 qvec = Vector3.Cross(tvec, edge1);
            float v = inv_det * Vector3.Dot(ray_direction, qvec);

            if (v < 0 || u + v > 1)
                return Tuple.Create(false, 0f);

            float t = inv_det * Vector3.Dot(edge2, qvec);
            return Tuple.Create(true, t);
        }
    }
}
