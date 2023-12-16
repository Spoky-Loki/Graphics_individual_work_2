using System.Drawing;
using System.Windows.Forms;
using System.Numerics;
using System.Collections.Generic;

namespace IND2
{
    public partial class Form1 : Form
    {
        private Bitmap Map;
        private Graphics Graphics;
        private int W, H;

        private List<Model> Scene = new List<Model>();

        private Vector3 CameraPosition;

        private Vector3 LightPosition;
        private float LightIntensity;

        public Form1()
        {
            InitializeComponent();

            Map = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics = Graphics.FromImage(Map);

            W = Map.Width;
            H = Map.Height;

            CameraPosition = new Vector3(0 ,0, 0);

            LightPosition = new Vector3(0, 4, 2.5f);
            LightIntensity = 0.7f;

            BuildRoom();
            Render();
        }

        private void BuildRoom()
        {
            Model room = Model.addCube(10, new Vector3(0, 0, 0), Color.White, true);
            room.Edges[0].color = Color.LightYellow;
            room.Edges[1].color = Color.LightYellow;
            room.Edges[2].color = Color.LightYellow;
            room.Edges[3].color = Color.LightYellow;
            room.Edges[4].color = Color.RoyalBlue;
            room.Edges[5].color = Color.RoyalBlue;
            room.Edges[6].color = Color.IndianRed;
            room.Edges[7].color = Color.IndianRed;
            room.Edges[8].color = Color.LightYellow;
            room.Edges[9].color = Color.LightYellow;
            room.Edges[10].color = Color.LightYellow;
            room.Edges[11].color = Color.LightYellow;

            Model cube1 = Model.addCube(2f, new Vector3(0f, 0f, 0f), Color.GreenYellow);
            Operation.Shift(ref cube1, new Vector3(3f, -3f, 3f));
            Operation.ReCalcNormal(ref cube1);

            Model cube2 = Model.addCube(2f, new Vector3(0f, 0f, 0f), Color.GreenYellow);
            Operation.Shift(ref cube2, new Vector3(-3f, -3f, 3f));
            Operation.ReCalcNormal(ref cube2);

            Sphere sphere1 = new Sphere(new Vector3(3f, 1, 3f), 1, Color.Yellow, 0);
            Sphere sphere2 = new Sphere(new Vector3(-4f, 1, 4f), 1, Color.Yellow, 0.9f);

            Scene.Add(room);
            Scene.Add(cube1);
            Scene.Add(cube2);
            Scene.Add(sphere1);
            Scene.Add(sphere2);
        }

        private void Render()
        {
            Graphics.Clear(Color.Black);
            for (int x = -W/2; x < W/2; x++)
            { 
                for (int y = -H/2; y < H/2; y++)
                {
                    var ray_start = CameraPosition;
                    var ray_end = CanvasToViewport(x, y);
                    Color color = RayTrace(ray_start, ray_end, 5);
                    int xxx = W;
                    int yyy = H;
                    int xx = W / 2 + x;
                    int yy = H / 2 - y;
                    Map.SetPixel(xx, yy, color);
                }
            }
            pictureBox.Image = Map;
        }

        private Vector3 CanvasToViewport(int x, int y)
        {
            return new Vector3(x * 1000/W, y * 1000/H, 300);
        }

        private void ComputeCloseIntersection(Vector3 ray_start, Vector3 ray_end, ref float intersect, ref Vector3 normal, ref Color color, ref float reflection, float min, float max)
        {
            intersect = float.MaxValue;
            normal = new Vector3(0, 0, 0);
            color = Color.Black;
            reflection = 0;

            foreach (Model model in Scene)
            {
                float current_intersect = float.MaxValue;
                Color current_color = Color.Black;
                Vector3 current_normal = new Vector3(0, 0, 0);
                float current_reflection = 0;

                if (model.points == null)
                {
                    Sphere s = (Sphere)model;
                    var inter = Sphere.IntersectRaySphere(ray_start, ray_end, s.center, s.radius);
                    var isIntersect = inter.Item1;
                    var intersectValue1 = inter.Item2.Item1;
                    var intersectValue2 = inter.Item2.Item2;
                    if (isIntersect)
                    {
                        if (intersectValue1 > min && intersectValue1 < max && intersectValue1 < current_intersect)
                        {
                            current_intersect = intersectValue1;
                            current_color = s.color;
                            current_reflection = s.reflection;
                            Vector3 point = ray_start + current_intersect * ray_end;
                            current_normal = Vector3.Normalize(point - s.center);
                        }
                        if (intersectValue2 > min && intersectValue2 < max && intersectValue2 < current_intersect)
                        {
                            current_intersect = intersectValue1;
                            current_color = s.color;
                            current_reflection = s.reflection;
                            Vector3 point = ray_start + current_intersect * ray_end;
                            current_normal = Vector3.Normalize(point - s.center);
                        }
                    }
                }
                else
                {
                    foreach (Edge edge in model.Edges)
                    {
                        Vector3[] vertex = new Vector3[3];
                        for (int i = 0; i < 3; i++)
                            vertex[i] = model.points[edge.points_index[i]];

                        var inter = Edge.IntersectRayTriangle(ray_start, ray_end, vertex);
                        var isIntersect = inter.Item1;
                        var intersectValue = inter.Item2;
                        if (isIntersect && intersectValue < current_intersect && intersectValue > min && intersectValue < max)
                        {
                            current_intersect = intersectValue;
                            current_color = edge.color;
                            current_reflection = 0;
                            current_normal = edge.normal;
                        }
                    }
                }

                if (current_intersect < intersect)
                {
                    intersect = current_intersect;
                    color = current_color;
                    reflection = current_reflection;
                    normal = current_normal;
                }
            }
        }

        private Color RayTrace(Vector3 ray_start, Vector3 ray_end, int depth)
        {
            float intersect = float.MaxValue;
            Vector3 normal = new Vector3(0, 0, 0);
            Color res_color = Color.Black;
            float reflection = 0;

            ComputeCloseIntersection(ray_start, ray_end, ref intersect, ref normal, ref res_color, ref reflection, 0, float.MaxValue);

            if (intersect == float.MaxValue)
                return res_color;

            var ray_direction = Vector3.Normalize(ray_end - ray_start);
            Vector3 point = ray_start + intersect * ray_direction;

            var diffuse = ComputeLight(normal, point);
            
            if (depth <= 0 || reflection == 0)
                return Color.FromArgb((int)(res_color.R * diffuse),
                (int)(res_color.G * diffuse),
                (int)(res_color.B * diffuse));

            var r = Operation.ReflectRay(-ray_end, normal);
            Color ref_color = RayTrace(point, r, depth - 1);

            return Color.FromArgb((int)(res_color.R * diffuse * (1 - reflection) + ref_color.R* reflection),
                (int)(res_color.G * diffuse * (1 - reflection) + ref_color.G * reflection),
                (int)(res_color.B * diffuse * (1 - reflection) + ref_color.B * reflection));
        }

        private double ComputeLight(Vector3 normal, Vector3 point)
        {
            double result = 0.0;
            var l = LightPosition - point;
            var v = Vector3.Dot(normal, l);

            float intersect = float.MaxValue;
            Vector3 norm = new Vector3(0, 0, 0);
            Color res_color = Color.Black;
            float reflection = 0;

            // Тень
            ComputeCloseIntersection(point, l, ref intersect, ref norm, ref res_color, ref reflection, 0.001f, 1);
            if (intersect != float.MaxValue)
                return 0.1f;

            // Диффузность
            if (v > 0)
                result += LightIntensity * v / (normal.Length() * l.Length());

            return result;
        }
    }
}
