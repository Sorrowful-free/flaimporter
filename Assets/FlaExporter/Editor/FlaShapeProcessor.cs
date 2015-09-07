using System.Collections.Generic;
using System.Linq;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Data.RawData.Geom;
using Assets.FlaExporter.Editor.Plugins.clipper;
using Assets.FlaExporter.Editor.Plugins.LibTessDotNet;
using Assets.FlaExporter.Editor.Plugins.poly2tri;
using Assets.FlaExporter.Editor.Plugins.poly2tri.Polygon;
using Assets.FlaExporter.Editor.Plugins.poly2tri.Triangulation;
using Assets.FlaExporter.Editor.Plugins.poly2tri.Triangulation.Delaunay.Sweep;
using Assets.FlaExporter.Editor.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Mesh = UnityEngine.Mesh;

namespace Assets.FlaExporter.Editor
{
    public static class FlaShapeProcessor
    {
        public const float PointByPixels = 2f;
        public static GameObject ProcessFlaShape(FlaShapeRaw shape)
        {
            Debug.Log("process shape");
           
            if (shape.Edges.Count > 0)
            {
                var resultMesh = new Mesh();
                var i = 0;
                resultMesh.subMeshCount = shape.Edges.Count;

                foreach (var edge in shape.Edges)
                {
                    var shapeGO = new GameObject("shape" + shape.GetHashCode());
                    shapeGO.AddComponent<MeshRenderer>();
                    var meshFilter = shapeGO.AddComponent<MeshFilter>();
           
                  //  var resultVertices = resultMesh.vertices.ToList();
                    
                    var mesh = ProcessFlaEdge(edge);
                    if (edge.FillStyle0 > 0)
                    {
                        mesh.colors = mesh.vertices.Select(e => new Color(1, 0, 0, 1)).ToArray();
                        mesh.vertices = mesh.vertices.Select(e =>
                        {
                            e.z = 1;
                            return new Vector3(e.x,e.y,1);
                        }).ToArray();
                    }
                    //if (mesh.vertices.Length > 0)
                    //{
                    //    resultVertices.AddRange(mesh.vertices);
                    //    resultMesh.vertices = resultVertices.ToArray();
                    //    resultMesh.SetTriangles(mesh.triangles.Select(e => e + resultMesh.vertices.Length - resultVertices.Count).ToArray(), i++);
                    //}
                    meshFilter.mesh = mesh;

                }
              //  meshFilter.mesh = resultMesh;
                
            }
         
            if (shape.Matrix != null && shape.Matrix.Matrix != null)
            {
              //  shape.Matrix.Matrix.CopyMatrix(shapeGO.transform);
            }
            return new GameObject();
        }

        private static Mesh ProcessFlaEdge(FlaEdgeRaw edge)
        {
            var mesh = new Mesh();
            var vertices = ProcessFlaEdgeString(edge.Edges);
            var polygons = Clipper.SimplifyPolygon(vertices.Select(e => new IntPoint(e.x*20, e.y*20)).ToList(),PolyFillType.pftNonZero);
            var resultIndexes = new List<int>();
            var resultVertices = new List<Vector3>();

            //var polygon = new Polygon(vertices.Select(e => new PolygonPoint(e.x, e.y)));
            //P2T.Triangulate(polygon);
            //mesh.vertices = polygon.Points.Select(e => new Vector3((float) e.X, (float) e.Y)).ToArray();
            //mesh.triangles =polygon.Triangles.SelectMany(e => e.Points.Select(p => polygon.Points.IndexOf(p))).ToArray()


            var sweep = new Tess();
            sweep.AddContour(vertices.Select(e=> new ContourVertex{Position = new Vec3{X = e.x,Y = e.y}}).ToArray());
            sweep.Tessellate(WindingRule.NonZero, ElementType.BoundaryContours,50);
            mesh.vertices = sweep.Vertices.Select(e => new Vector3(e.Position.X, e.Position.Y)).ToArray();
            mesh.triangles = sweep.Elements;
            return mesh;
               
            Debug.Log("polygon count:"+polygons.Count+" vertex count:"+vertices.Count);
            for (int i = 0; i < polygons.Count; i++)
            {
               // var oldVertices = mesh.vertices;
                var polygonVertices = polygons[i].Select(e => new Vector3((float) e.X/20.0f, (float) e.Y/20.0f));
                var polygonTrinagles = FlaTriangulate.Process(polygonVertices.Select(e=> (Vector2)e).ToList());
                if (polygonTrinagles == null || polygonTrinagles.Count <= 0)
                {
                    Debug.Log("triangles empty");
                    continue;
                }
                resultIndexes.AddRange(polygonTrinagles.Select(e => e + resultVertices.Count));
                resultVertices.AddRange(polygonVertices);

              //  var meshVertices = oldVertices.ToList();
               // meshVertices.AddRange(polygonVertices);
              //  meshVertices.Reverse();
              //  mesh.vertices = meshVertices.ToArray();
             //   mesh.SetTriangles(polygonTrinagles.Select(e => e + mesh.vertices.Length - meshVertices.Count).ToArray(), i);
                Debug.Log("triangleCount:"+polygonTrinagles.Count);
            }
            mesh.vertices = resultVertices.ToArray();
            mesh.triangles = resultIndexes.ToArray();
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
            if (edgeString.EndsWith(" "))
                edgeString = edgeString.Substring(0, edgeString.Length - 1);
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

        private static List<Vector2> GetFlaCurve(Vector2 point1, Vector2 controlpoint,Vector2 point2)
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
