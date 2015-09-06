using System.Collections.Generic;
using System.Linq;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Data.RawData.Geom;
using Assets.FlaExporter.Editor.Utils;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.FlaExporter.Editor
{
    public static class FlaShapeProcessor
    {
        public const float PointByPixels = 0.5f;
        public static GameObject ProcessFlaShape(FlaShapeRaw shape)
        {
            var shapeGO = new GameObject("shape"+shape.GetHashCode());
            shapeGO.AddComponent<MeshRenderer>();
            var meshFilter = shapeGO.AddComponent<MeshFilter>();
            if (shape.Edges.Count > 0)
            {
                var mesh = ProcessFlaEdge(shape.Edges[0]);
                meshFilter.mesh = mesh;
            }
            
            //foreach (var edgeRaw in shape.Edges)
            //{
            //    var line = shapeGO.AddComponent<LineRenderer>();
            //    var mesh = ProcessFlaEdge(edgeRaw);
            //    line.SetVertexCount(mesh.vertices.Length);
            //    for (int i = 0; i < mesh.vertices.Length; i++)
            //    {
            //        line.SetPosition(i,mesh.vertices[i]/100);    //todo costil
            //    }
                
            //}
            if (shape.Matrix != null && shape.Matrix.Matrix != null)
            {
                shape.Matrix.Matrix.CopyMatrix(shapeGO.transform);
            }
            return shapeGO;
        }

        private static Mesh ProcessFlaEdge(FlaEdgeRaw edge)
        {
            var mesh = new Mesh();
            var vertices = ProcessFlaEdgeString(edge.Edges);
            mesh.vertices = vertices.Select(e => (Vector3)e).ToArray();
            mesh.normals = new Vector3[vertices.Count];
            mesh.tangents = new Vector4[vertices.Count];
            mesh.uv = new Vector2[vertices.Count];
            var indexes = FlaTriangulate.Process(vertices);
            if (indexes != null)
            {
                mesh.triangles = indexes.ToArray();
                Debug.Log("indexes = " + indexes.Count);
            }
                
            return mesh;
        }

        private static List<Vector2> ProcessFlaEdgeString(string edgeString)
        {
            var list = new List<Vector2>();
            if (edgeString == null || edgeString == "")
                return list;
            edgeString =
                edgeString.Replace("!", " ! ")
                    .Replace("|", " | ")
                    .Replace("/", " | ")
                    .Replace("\\", " | ")
                    .Replace("[", " [ ")
                    .Replace("]", " [ ")
                    .Replace("  "," ")
                    .Replace("   "," ");
            Debug.Log(edgeString);
            var commands = edgeString.Split(' ');
            var operationIndex = 0;
            while (operationIndex < commands.Length)
            {
                switch (commands[operationIndex])
                {
                    case "!":
                        list.Add(TryParseFlaVector2(commands[operationIndex + 1], commands[operationIndex + 2]));
                        operationIndex += 3;
                        break;

                    case "|":
                        list.Add(TryParseFlaVector2(commands[operationIndex + 1], commands[operationIndex + 2]));
                        operationIndex += 3;
                        break;

                    case "[":
                        list.AddRange(GetFlaCurve(list.Last(),TryParseFlaVector2(commands[operationIndex + 1], commands[operationIndex + 2]), TryParseFlaVector2(commands[operationIndex + 3], commands[operationIndex + 4])));
                        operationIndex += 5;
                        break;
                        
                    default:
                        Debug.LogWarningFormat("can parce operation \"{0}\"",commands[operationIndex]);
                        operationIndex++;
                        break;
                }
            }
            return list.Distinct().ToList();
        }

        private static Vector2 TryParseFlaVector2(string commandX, string commandY)
        {
            return new Vector2((float)FlaMathUtils.ParseFlaInteger(commandX) / 20.0f,
                                (float)FlaMathUtils.ParseFlaInteger(commandY) / 20.0f);
        }

        private static List<Vector2> GetFlaCurve(Vector2 point1, Vector2 point2, Vector2 controlpoint)
        {
            var curveList = new List<Vector2>();
            var length = (point1 - controlpoint).magnitude;
            length += (point2 - controlpoint).magnitude;
            var countOfPoints = (int) (length/PointByPixels);
            for (int i = 1; i < countOfPoints ; i++)
            {
                var delta = (float)i / (float)countOfPoints;
                curveList.Add(FlaMathUtils.CalculateQuadCurvePoint(point1, point2, controlpoint, delta));
            }
            return curveList;
        }

        




        



    }
}
