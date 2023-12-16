using System;
using System.Numerics;

namespace IND2
{
    public class Operation
    {
        public static Vector3 ReflectRay(Vector3 ray, Vector3 normal)
        {
            return 2 * normal * Vector3.Dot(normal, ray) - ray;
        }

        public static Vector3 normal(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 v1 = p1 - p0;
            Vector3 v2 = p2 - p0;
            Vector3 normal = Vector3.Cross(v1, v2);
            return Vector3.Normalize(normal);
        }

        public static void ReCalcNormal(ref Model model)
        {
            foreach (var edge in model.Edges)
            {
                Vector3[] vertex = new Vector3[3];
                for (int i = 0; i < 3; i++)
                    vertex[i] = model.points[edge.points_index[i]];

                edge.normal = normal(vertex[0], vertex[1], vertex[2]);
            }
        }

        public static void Rotate(ref Model model, float angle, int axis)
        {
            float[,] matrix = new float[4, 4];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
            matrix[3, 3] = 1;

            if (axis == 0) 
            {
                float[,] addMatrix = new float[4, 4];
                addMatrix[0, 0] = 1;
                addMatrix[1, 1] = (float)Math.Cos(angle * (float)Math.PI / 180);
                addMatrix[2, 2] = (float)Math.Cos(angle * (float)Math.PI / 180);
                addMatrix[2, 1] = -(float)Math.Sin(angle * (float)Math.PI / 180);
                addMatrix[1, 2] = (float)Math.Sin(angle * (float)Math.PI / 180);
                addMatrix[3, 3] = 1;
                matrix = MultiplicationMatrix(matrix, addMatrix);
            }
            else if (axis == 1)
            {
                float[,] addMatrix = new float[4, 4];
                addMatrix[1, 1] = 1;
                addMatrix[0, 0] = (float)Math.Cos(angle * (float)Math.PI / 180);
                addMatrix[2, 2] = (float)Math.Cos(angle * (float)Math.PI / 180);
                addMatrix[2, 0] = (float)Math.Sin(angle * (float)Math.PI / 180);
                addMatrix[0, 2] = -(float)Math.Sin(angle * (float)Math.PI / 180);
                addMatrix[3, 3] = 1;
                matrix = MultiplicationMatrix(matrix, addMatrix);
            }
            else if (axis == 2)
            {
                float[,] addMatrix = new float[4, 4];
                addMatrix[2, 2] = 1;
                addMatrix[0, 0] = (float)Math.Cos(angle * (float)Math.PI / 180);
                addMatrix[1, 1] = (float)Math.Cos(angle * (float)Math.PI / 180);
                addMatrix[1, 0] = -(float)Math.Sin(angle * (float)Math.PI / 180);
                addMatrix[0, 1] = (float)Math.Sin(angle * (float)Math.PI / 180);
                addMatrix[3, 3] = 1;
                matrix = MultiplicationMatrix(matrix, addMatrix);
            }
            for (int i = 0; i < model.points.Length; i++)
            {
                var vec = Multiplication(model.points[i], matrix);
                model.points[i] = new Vector3(vec[0], vec[1], vec[2]);
            }
        }

        public static void Shift(ref Model model, Vector3 vector)
        {
            float[,] matrix = new float[,]
                { {  1,  0, 0, 0 },
                  {  0,  1, 0, 0 },
                  {  0,  0, 1, 0 },
                  { vector.X, vector.Y, vector.Z, 1 } };

            for (int i = 0; i < model.points.Length; i++)
            {
                var vec = Multiplication(model.points[i], matrix);
                model.points[i] = new Vector3(vec[0], vec[1], vec[2]);
            }
        }

        public static float[,] MultiplicationMatrix(float[,] matrix1, float[,] matrix2)
        {
            float[,] resMatrix = new float[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                        resMatrix[i, j] += matrix1[i, k] * matrix2[k, j];
            return resMatrix;
        }

        private static float[] Multiplication(Vector3 point, float[,] matrix)
        {
            float[] newPoint = new float[4];
            float[] pointArray = new float[]
                { point.X, point.Y, point.Z, 1 };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    newPoint[i] += pointArray[j] * matrix[j, i];
            return newPoint;
        }
    }
}
