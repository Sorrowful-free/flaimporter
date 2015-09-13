using System.Collections.Generic;
using System.Linq;
using Assets.BundleExporter.Editor.Helpers;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Plugins.LibTessDotNet;
using Assets.FlaExporter.Editor.Utils;
using UnityEditor;
using UnityEngine;
using Mesh = UnityEngine.Mesh;

namespace Assets.FlaExporter.Editor
{
    public static class FlaShapeProcessor
    {
        public const float PointByPixels = 3f;
        public static GameObject ProcessFlaShape(FlaShapeRaw shape)
        {
            Debug.Log("process shape");
            var shapeGO = new GameObject("shape" + shape.GetHashCode());
            shapeGO.AddComponent<MeshRenderer>();
            var meshFilter = shapeGO.AddComponent<MeshFilter>();
            var shapeVertices = new List<Vector3>();
            var shapeTriangles = new List<List<int>>();
            var shapeMesh = new Mesh();
            if (shape.Edges.Count > 0)
            {
               
                var groupByFill = new Dictionary<int, List<List<Vector3>>>();
                Debug.Log(shape.Edges.Select(e => e.Edges).JoinToString("->").GetHashCode());
                foreach (var edge in shape.Edges)
                {
                    if (edge.Edges == null || edge.Edges == "" )
                    {
                        continue;
                    }
                    var list = default(List<List<Vector3>>);
                    if (!groupByFill.TryGetValue(edge.FillStyle1, out list))
                    {
                        groupByFill.Add(edge.FillStyle1,new List<List<Vector3>>());
                    }
                 //   Debug.Log(edge.Edges.GetHashCode());
                  //  if (edge.FillStyle0 != 0)
                  //  {
                 //       groupByFill[edge.FillStyle1].AddRange(ProcessFlaEdgeString(edge.Edges).Select(e => e.Select(v => (Vector3)v).Reverse().ToList()));    
                 //   }
                 //   else
                //    {
                        groupByFill[edge.FillStyle1].AddRange(ProcessFlaEdgeString(edge.Edges).Select(e => e.Select(v => (Vector3)v).ToList()));
                //    }
                }
                var submeshInstance = 0;
                var combines = new List<CombineInstance>();
                foreach (var pair in groupByFill)
                {
                   
                    var tess = new Tess();
                    
                    foreach (var polygon in pair.Value)
                    {
                        if (polygon.Count <= 0)
                        {
                            continue;
                        }
                        
                        tess.AddContour(polygon.Select(e=> new ContourVertex{Position = new Vec3{X = e.x,Y = e.y}}).ToArray(),ContourOrientation.Original);
                    }

                    tess.Tessellate(WindingRule.NonZero, ElementType.Polygons, 3);
                    shapeTriangles.Add(tess.Elements.Select(e=>e+shapeVertices.Count).Reverse().ToList());
                    shapeVertices.AddRange(tess.Vertices.Select(e => new Vector3(e.Position.X, e.Position.Y)));
                }

                shapeMesh.vertices = shapeVertices.ToArray();
                for (int i = 0; i < shapeTriangles.Count; i++)
                {
                    shapeMesh.SetTriangles(shapeTriangles[i].ToArray(),i);
                }
                FolderAndFileUtils.CheckFolders(FoldersConstants.ShapesFolder);
                AssetDatabase.CreateAsset(shapeMesh,FolderAndFileUtils.GetAssetFolder(FoldersConstants.ShapesFolder)+shapeGO.name+".asset");
                meshFilter.mesh = shapeMesh;

            }
            
            if (shape.Matrix != null && shape.Matrix.Matrix != null)
            {
                shape.Matrix.Matrix.CopyMatrix(shapeGO.transform);
            }
            return shapeGO;
        }

        private static List<List<Vector2>> ProcessFlaEdgeString(string edgeString)
        {
            var list = new List<List<Vector2>>();
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
                        var moveToPos = TryParseFlaVector2(commands[operationIndex + 1], commands[operationIndex + 2]);
                        if(list.Count <= 0) 
                            list.Add(new List<Vector2>());
                        if( list.Last().Count <= 0 || list.Last().Last() != moveToPos)
                            list.Last().Add(moveToPos);
                        operationIndex += 3;
                        break;

                    case "|":
                        var lineToPos = TryParseFlaVector2(commands[operationIndex + 1], commands[operationIndex + 2]);
                        if (list.Last().First() == lineToPos)
                        {
                            list.Add(new List<Vector2>());
                        }
                        else
                        {
                            list.Last().Add(lineToPos);    
                        }
                        operationIndex += 3;
                        break;

                    case "[":
                        var controllPos= TryParseFlaVector2(commands[operationIndex + 1], commands[operationIndex + 2]);
                        var secondPos = TryParseFlaVector2(commands[operationIndex + 3], commands[operationIndex + 4]);
                        list.Last().AddRange(GetFlaCurve(list.Last().Last(), controllPos, secondPos));
                        if (list.Last().First() == secondPos)
                        {
                            list.Last().Remove(list.Last().Last());
                            list.Add(new List<Vector2>());
                        }
                        operationIndex += 5;
                        break;
                        
                    default:
                        Debug.LogWarningFormat("can parse operation \"{0}\"",commands[operationIndex]);
                        operationIndex++;
                        break;
                }
            }
            return list.ToList();
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
