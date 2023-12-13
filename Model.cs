using System.Drawing;
using System.Numerics;

namespace IND2
{
    public class Model
    {
        public Vector3 center;
        public Vector3[] points;
        public Edge[] Edges;
        public Color color;

        public Model(Vector3 center, Color color) 
        {
            this.center = center;
            this.color = color;
        }

        public Model(Vector3 centre, Vector3[] points, Edge[] polygons)
        {
            this.center = centre;
            this.points = points;
            this.Edges = polygons;
            this.color = Color.White;
        }

        public Model(Vector3 point, Vector3[] points, Edge[] polygons, Color color)
        {
            this.center = point;
            this.points = points;
            this.Edges = polygons;
            this.color = color;

            foreach (var poly in polygons)
                poly.color = color;
        }

        public static Model addCube(float size, Vector3 point, Color color, bool isRoom = false)
        {
            size /= 2;

            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(size, size, size);
            points[1] = new Vector3(-size, size, size);
            points[2] = new Vector3(-size, size, -size);
            points[3] = new Vector3(size, size, -size);
            points[4] = new Vector3(size, -size, size);
            points[5] = new Vector3(-size, -size, size);
            points[6] = new Vector3(-size, -size, -size);
            points[7] = new Vector3(size, -size, -size);

            Edge[] polygons = new Edge[12];
            if (isRoom)
            {
                polygons[0] = new Edge(new int[] { 0, 1, 2 }, Operation.normal(points[0], points[1], points[2]));
                polygons[1] = new Edge(new int[] { 0, 2, 3 }, Operation.normal(points[0], points[2], points[3]));
                polygons[2] = new Edge(new int[] { 4, 7, 6 }, Operation.normal(points[4], points[7], points[6]));
                polygons[3] = new Edge(new int[] { 4, 6, 5 }, Operation.normal(points[4], points[6], points[5]));
                polygons[4] = new Edge(new int[] { 2, 5, 6 }, Operation.normal(points[2], points[5], points[6]));
                polygons[5] = new Edge(new int[] { 2, 1, 5 }, Operation.normal(points[2], points[1], points[5]));
                polygons[6] = new Edge(new int[] { 3, 4, 0 }, Operation.normal(points[3], points[4], points[0]));
                polygons[7] = new Edge(new int[] { 3, 7, 4 }, Operation.normal(points[3], points[7], points[4]));
                polygons[8] = new Edge(new int[] { 4, 5, 1 }, Operation.normal(points[4], points[5], points[1]));
                polygons[9] = new Edge(new int[] { 4, 1, 0 }, Operation.normal(points[4], points[1], points[0]));
                polygons[10] = new Edge(new int[] { 2, 7, 3 }, Operation.normal(points[2], points[7], points[3]));
                polygons[11] = new Edge(new int[] { 2, 6, 7 }, Operation.normal(points[2], points[6], points[7]));
            }
            else
            {
                polygons[0] = new Edge(new int[] { 1, 3, 2 }, Operation.normal(points[1], points[3], points[2]));
                polygons[1] = new Edge(new int[] { 1, 0, 3 }, Operation.normal(points[1], points[0], points[3]));
                polygons[2] = new Edge(new int[] { 4, 7, 6 }, Operation.normal(points[4], points[7], points[6]));
                polygons[3] = new Edge(new int[] { 4, 6, 5 }, Operation.normal(points[4], points[6], points[5]));
                polygons[4] = new Edge(new int[] { 2, 6, 5 }, Operation.normal(points[2], points[6], points[5]));
                polygons[5] = new Edge(new int[] { 2, 5, 1 }, Operation.normal(points[2], points[5], points[1]));
                polygons[6] = new Edge(new int[] { 7, 0, 4 }, Operation.normal(points[7], points[0], points[4]));
                polygons[7] = new Edge(new int[] { 7, 3, 0 }, Operation.normal(points[7], points[3], points[0]));
                polygons[8] = new Edge(new int[] { 4, 1, 5 }, Operation.normal(points[4], points[1], points[5]));
                polygons[9] = new Edge(new int[] { 4, 0, 1 }, Operation.normal(points[4], points[0], points[1]));
                polygons[10] = new Edge(new int[] { 2, 7, 3 }, Operation.normal(points[2], points[7], points[3]));
                polygons[11] = new Edge(new int[] { 7, 2, 6 }, Operation.normal(points[7], points[2], points[6]));
            }

            Model polyhedron = new Model(point, points, polygons, color);
            return polyhedron;
        }
    }
}
