using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaExporter.Editor.Data.RawData.FillStyles;
using Assets.FlaExporter.Editor.Data.RawData.FillStyles.FillStyles;
using Assets.FlaExporter.Editor.Data.RawData.FillStyles.FillStyles.GradientFillStyles;
using Assets.FlaExporter.Editor.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.Data.RawData.StorkeStyle.StorkeStyles;
using Assets.FlaExporter.Editor.Extentions;
using Assets.FlaExporter.Editor.Plugins.LibTessDotNet;
using Assets.FlaExporter.Editor.Utils;
using Assets.FlaExporter.FlaExporter.Renderers.Enums;
using Assets.FlaExporter.FlaExporter.Renderers.FillStyles;
using UnityEditor;
using UnityEngine;
using Mesh = UnityEngine.Mesh;

namespace Assets.FlaExporter.Editor.FlaProcessors
{
    public static class FlaShapeProcessor
    {
        
        public const float UnitsPerPixel = 20;//flash units
        public static IEnumerator ProcessFlaShape(FlaShapeRaw shape,Action<GameObject> callback)
        {
            var shapeGO = new GameObject(shape.GetUniqueName());
            if (shape.Matrix != null && shape.Matrix.Matrix != null)
            {
                shape.Matrix.Matrix.CopyMatrix(shapeGO.transform);
            }
            var meshRenderer = shapeGO.AddComponent<MeshRenderer>();
            var meshFilter = shapeGO.AddComponent<MeshFilter>();

            var sharedMaterials = new List<Material>();
            foreach (var fillStyleRaw in shape.FillStyles)
            {
                sharedMaterials.Add(ProcessFlaFillStyle(fillStyleRaw));
                yield return null;
            }
            meshRenderer.sharedMaterials = sharedMaterials.ToArray();

            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.ShapesFolder) + shapeGO.name + ".asset");
            if (mesh != null)
            {
                meshFilter.mesh = mesh;
                if (callback != null)
                {
                    callback(shapeGO);
                }
                yield break;
            }

          

            var matrix = new Matrix4x4();
            matrix.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1,-1,1));
            var shapeVertices = new List<Vector3>();
            var shapeTriangles = new List<List<int>>();
            var shapeMesh = new Mesh();
            if (shape.Edges.Count > 0)
            {
                var groupByFill = new Dictionary<int, List<List<Vector3>>>();
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
                    groupByFill[edge.FillStyle1].AddRange(ProcessFlaEdgeString(edge.Edges).Select(e => e.Select(v => (Vector3)v).ToList()));
                    yield return null;
                }
                foreach (var pair in groupByFill)
                {
                   var tess = new Tess();
                    
                    foreach (var polygon in pair.Value)
                    {
                        if (polygon.Count <= 0)
                        {
                            continue;
                        }
                        var transformPolygon = polygon.Select(e => matrix.MultiplyPoint(e));
                        tess.AddContour(transformPolygon.Select(e => new ContourVertex { Position = new Vec3 { X = e.x, Y = e.y } }).ToArray(), ContourOrientation.Original);
                        yield return null;
                    }

                    tess.Tessellate(WindingRule.NonZero, ElementType.Polygons, 3);
                    shapeTriangles.Add(tess.Elements.Select(e=>e+shapeVertices.Count).ToList());
                    shapeVertices.AddRange(tess.Vertices.Select(e => new Vector3(e.Position.X, e.Position.Y)/FlaExporterConstatns.PixelsPerUnits));
                    yield return null;

                    
                }

                shapeMesh.vertices = shapeVertices.ToArray();
                for (int i = 0; i < shapeTriangles.Count; i++)
                {
                    shapeMesh.SetTriangles(shapeTriangles[i].ToArray(),i);
                    yield return null;
                }
                #region uv0
                var bounds = shapeMesh.bounds;
                var uvs = new List<Vector2>();
                var isHorisontal = bounds.size.x > bounds.size.y;
                var yOffset = isHorisontal ? (bounds.size.x - bounds.size.y)/2 : 0;// pomestat menyami
                var xOffset = isHorisontal ? 0 : (bounds.size.y - bounds.size.x)/2;
                var offsets = new Vector3(xOffset,yOffset);
                var maxSize = isHorisontal ? bounds.size.x : bounds.size.y;
                for (var i = 0; i < shapeMesh.vertexCount; i++)
                {
                    var normalizedPosition = shapeMesh.vertices[i]+ offsets - bounds.min;
                    var uv = Vector2.zero;
                    uv.x = 1 - normalizedPosition.x / maxSize;
                    uv.y = normalizedPosition.y / maxSize;
                    uvs.Add(uv);
                    yield return null;
                }
                shapeMesh.uv = uvs.ToArray();
                #endregion
                FolderAndFileUtils.CheckFolders(FoldersConstants.ShapesFolder);
                AssetDatabase.CreateAsset(shapeMesh, FolderAndFileUtils.GetAssetFolder(FoldersConstants.ShapesFolder) + shapeGO.name + ".asset");
                meshFilter.mesh = shapeMesh;
            }

            if (callback != null)
            {
                callback(shapeGO);
            }
        }

        private static Material ProcessFlaFillStyle(FlaFillStyleRaw fillStyleRaw)
        {
            var fillStyle = fillStyleRaw.FillStyle;
            var material = default(Material);
            var needSaveMaterial = false;
            if (fillStyle is FlaSolidColorFillStyleRaw)
            {
                
                var solidColor = (FlaSolidColorFillStyleRaw)fillStyle;
                var matName = "material" + solidColor.Color.GetHashCode();
                material = TryLoadMaterial(matName);
                if (material == null)
                {
                    material = new Material(UnityEngine.Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.SolidColor]));
                    var color = Color.black;
                    Color.TryParseHexString(solidColor.Color, out color);
                    material.SetColor("_Color", color);
                    material.name = matName;
                    needSaveMaterial = true;
                }
               
               

            }
            else if (fillStyle is FlaBitmapFillRaw)
            {
                var bitmap = (FlaBitmapFillRaw)fillStyle;
                var matName = "material" + bitmap.BitmapPath.GetHashCode();
                material = TryLoadMaterial(matName);
                if (material == null)
                {
                    material = new Material(UnityEngine.Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.Bitmap]));
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) +
                        bitmap.BitmapPath);
                    material.SetTexture("_Bitmap", texture);
                    material.SetFloat("_TextureAspect", (float)texture.width / (float)texture.height);
                    material.name = matName;
                    needSaveMaterial = true;
                }
            }
            else if (fillStyle is FlaLinearGradientFillStyleRaw)
            {
                var linearGradient = (FlaLinearGradientFillStyleRaw) fillStyle;
                var matName = "material" + linearGradient.GetHashCode();
                material = TryLoadMaterial(matName);
                if (material == null)
                {
                    material = new Material(UnityEngine.Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.LinearGradient]));
                    ProcessFlaGradientEntries(linearGradient.GradientEntries, ref material);
                    material.name = matName;
                    needSaveMaterial = true;
                }
                

            }
            else if (fillStyle is FlaRadialGradientFillStyleRaw)
            {
                var radialGradient = (FlaRadialGradientFillStyleRaw)fillStyle;
                var matName = "material" + radialGradient.GetHashCode();
                material = TryLoadMaterial(matName);
                if (material == null)
                {
                    material = new Material(UnityEngine.Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.RadialGradient]));
                    Debug.Log(radialGradient.GradientEntries + " : " + material);
                    ProcessFlaGradientEntries(radialGradient.GradientEntries, ref material);
                    material.name = matName;
                    needSaveMaterial = true;
                }
                
            }
            var matrix = fillStyleRaw.FillStyle.Matrix.Matrix;

            material.SetVector("_TextureMatrixABCD", new Vector4(matrix.A,matrix.B,matrix.C,matrix.D));// not work =(
            material.SetVector("_TextureMatrixTXTY", new Vector4(matrix.TX, matrix.TY));// not work =(

            if (needSaveMaterial)
            {
                FolderAndFileUtils.CheckFolders(FoldersConstants.MaterialsFolder);
                AssetDatabase.CreateAsset(material, FolderAndFileUtils.GetAssetFolder(FoldersConstants.MaterialsFolder) + material.name + ".mat");
                AssetDatabase.Refresh();    
                Debug.Log("save material "+ material.name);
            }

            return material;
        }

        private static Material TryLoadMaterial(string name)
        {
            return
                AssetDatabase.LoadAssetAtPath<Material>(
                    FolderAndFileUtils.GetAssetFolder(FoldersConstants.MaterialsFolder) + name+".mat");
        }

        private static void ProcessFlaGradientEntries(List<FlaGradientEntryRaw> entries, ref Material material)
        {
            var colors = new Texture2D(entries.Count, 1);
            var weights = new Texture2D(entries.Count, 1);
            for (int i = 0; i < entries.Count; i++)
            {
                var entryRaw = entries[i];
                var color = default(Color);
                Color.TryParseHexString(entryRaw.Color, out color);
                color.a = entryRaw.Alpha;
                colors.SetPixel(i, 0, color);
                weights.SetPixel(i,0,new Color(entryRaw.Ratio,0,0,0));
            }
            FolderAndFileUtils.CheckFolders(FoldersConstants.MaterialsFolder);
            AssetDatabase.CreateAsset(colors, FolderAndFileUtils.GetAssetFolder(FoldersConstants.MaterialsFolder) + "color" + material.GetHashCode() + ".asset");
            FolderAndFileUtils.CheckFolders(FoldersConstants.MaterialsFolder);
            AssetDatabase.CreateAsset(weights, FolderAndFileUtils.GetAssetFolder(FoldersConstants.MaterialsFolder) + "weight" + material.GetHashCode() + ".asset");
            material.SetTexture("_Colors",colors);
            material.SetTexture("_ColorWeight", weights);
            material.SetInt("_GradientEntryCount", entries.Count);
           
        }

        public static GameObject ProcessFlaStorkeEdge(FlaBaseStorkyStyleRaw storkeStyle, List<Vector2> edge)
        {
            return new GameObject("stroke");
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
                    case "":
                        operationIndex++;
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
            return new Vector2((float)FlaMathUtils.ParseFlaInteger(commandX) / UnitsPerPixel,
                                (float)FlaMathUtils.ParseFlaInteger(commandY) / UnitsPerPixel);
        }

        private static List<Vector2> GetFlaCurve(Vector2 point1, Vector2 controlpoint,Vector2 point2)
        {
            var curveList = new List<Vector2>();
            var length = (point1 - controlpoint).magnitude;
            length += (point2 - controlpoint).magnitude;
            var countOfPoints = (int) (length/FlaExporterConstatns.CurveQuality);
            for (int i = 1; i < countOfPoints ; i++)
            {
                var delta = (float)i / (float)countOfPoints;
                curveList.Add(FlaMathUtils.CalculateQuadCurvePoint(point1, point2, controlpoint, delta));
            }
            return curveList;
        }


    }
}
