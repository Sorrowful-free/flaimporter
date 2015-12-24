using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData.FillStyles;
using Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles;
using Assets.FlaImporter.Editor.Data.RawData.FillStyles.FillStyles.GradientFillStyles;
using Assets.FlaImporter.Editor.Data.RawData.FrameElements;
using Assets.FlaImporter.Editor.Data.RawData.Geom;
using Assets.FlaImporter.Editor.Data.RawData.StorkeStyle.StorkeStyles;
using Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw;
using Assets.FlaImporter.Editor.Plugins.LibTessDotNet;
using Assets.FlaImporter.Editor.Utils;
using Assets.FlaImporter.FlaImporter.Geom;
using Assets.FlaImporter.FlaImporter.Renderer;
using Assets.FlaImporter.FlaImporter.Renderer.Enums;
using Assets.FlaImporter.FlaImporter.Renderer.FillStyles;
using Assets.FlaImporter.FlaImporter.Transorm;
using UnityEditor;
using UnityEngine;
using Mesh = UnityEngine.Mesh;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaShapeProcessor
    {
        
        public const float UnitsPerPixel = 20;
        public static IEnumerator ProcessFlaShape(FlaShapeRaw shape, Action<GameObject> callback)
        {
            var shapeName = shape.GetUniqueName();


            var shapeGO = default(GameObject);
            var prefab = AssetDataBaseUtility.LoadShape(shapeName);
            
            if (prefab != null)
            {
                shapeGO = GameObject.Instantiate(prefab);
                if (callback != null)
                {
                    callback(shapeGO);
                }
                yield break;
            }
           
            shapeGO = new GameObject(shapeName);

            var shapeComponent = shapeGO.AddComponent<FlaShape>();
            shapeGO.AddComponent<FlaTransform>();
            
            if (shape.Matrix != null && shape.Matrix.Matrix != null)
            {
                shape.Matrix.Matrix.CopyMatrix(shapeGO.transform);
            }
            
            foreach (var flaEdgeRaw in shape.Edges)
            {
                if (flaEdgeRaw == null || flaEdgeRaw.Edges == null || flaEdgeRaw.Edges == "")
                {
                    continue;
                }
                var fillStyleRaw = shape.FillStyles.FirstOrDefault(e => e.Index == flaEdgeRaw.FillStyle1);
                if (fillStyleRaw == null)
                {
                    fillStyleRaw = shape.FillStyles.FirstOrDefault(e => e.Index == flaEdgeRaw.FillStyle0);
                }

                if (fillStyleRaw == null)
                {
                    continue;
                }

                var ordered = -(float)shape.Edges.IndexOf(flaEdgeRaw) / (float)shape.Edges.Count;
                var flaEdge = ProsessFlaEdge(flaEdgeRaw);
                
                flaEdge.FillStyle = ProcessFlaFillStyle(fillStyleRaw);
                flaEdge.gameObject.GetComponent<MeshRenderer>().material = flaEdge.FillStyle.Material;
                flaEdge.transform.SetParent(shapeGO.transform);
                flaEdge.transform.localScale = Vector3.one;
                flaEdge.transform.localPosition = Vector3.forward * ordered;
                flaEdge.Parent = shapeComponent;
                shapeComponent.Edges.Add(flaEdge);
            }
            if (callback != null)
            {
                callback(shapeGO);
            }

            AssetDataBaseUtility.SaveShape(shapeGO);
        }

        private static FlaEdge ProsessFlaEdge(FlaEdgeRaw edgeRaw)
        {
            var edgeName = FolderAndFileUtils.RemoveUnacceptable("edge_" + edgeRaw.Edges.GetHashCode());
            var prefab = AssetDataBaseUtility.LoadEdge(edgeName);
            var edgeGO = default(GameObject);
            if (prefab != null)
            {
                edgeGO = GameObject.Instantiate(prefab);
                return edgeGO.GetComponent<FlaEdge>();
            }
            edgeGO = new GameObject(edgeName);
            var edgeMeshFilter = edgeGO.AddComponent<MeshFilter>();
            var edgeMeshRenderer = edgeGO.AddComponent<MeshRenderer>();
            var edge = edgeGO.AddComponent<FlaEdge>();
            if (edgeRaw.Edges == null || edgeRaw.Edges == "")
            {
                return null;
            }
            var tess = new Tess();
            var matrix = new Matrix4x4();
            matrix.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
            var polygons = ProcessFlaEdgeString(edgeRaw.Edges);
            foreach (var polygon in polygons)
            {
                var transformPolygon = polygon.Select(e => matrix.MultiplyPoint(e));
                tess.AddContour(transformPolygon.Select(e => new ContourVertex { Position = new Vec3 { X = e.x, Y = e.y } }).ToArray(), ContourOrientation.Original);
            }
            tess.Tessellate(WindingRule.NonZero, ElementType.Polygons, 3);
            var mesh = new Mesh();
            mesh.vertices = tess.Vertices.Select(e => new Vector3(e.Position.X, e.Position.Y)/FlaExporterConstatns.PixelsPerUnits).ToArray();
            mesh.triangles = tess.Elements.ToArray();

            #region uv0 and uv1
            var bounds = mesh.bounds;
            var uvs0 = new List<Vector2>();
            var uvs1 = new List<Vector2>();
            var isHorisontal = bounds.size.x > bounds.size.y;
            var yOffset = isHorisontal ? (bounds.size.x - bounds.size.y) / 2 : 0;// pomestat menyami
            var xOffset = isHorisontal ? 0 : (bounds.size.y - bounds.size.x) / 2;
            var offsets = new Vector3(xOffset, yOffset);
            var maxSize = isHorisontal ? bounds.size.x : bounds.size.y;
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                var normalizedPosition0 = mesh.vertices[i] + offsets - bounds.min;
                var uv0 = Vector2.zero;
                uv0.x = normalizedPosition0.x / maxSize;
                uv0.y = normalizedPosition0.y / maxSize;
                uvs0.Add(uv0);

                var normalizedPosition1 = mesh.vertices[i] - bounds.min;
                var uv1 = Vector2.zero;
                uv1.x = normalizedPosition1.x / bounds.size.x;
                uv1.y = normalizedPosition1.y / bounds.size.y;
                uvs1.Add(uv1);
            }
            mesh.uv = uvs0.ToArray();
            mesh.uv2 = uvs1.ToArray();
            #endregion

            mesh.name = edgeName;
            AssetDataBaseUtility.SaveEdgeMesh(mesh);
            edgeMeshFilter.mesh = mesh;

            AssetDataBaseUtility.SaveEdge(edgeGO);
            return edge;
        }

        private static FlaFillStyle ProcessFlaFillStyle(FlaFillStyleRaw fillStyleRaw)
        {
            var fillStyle = fillStyleRaw.FillStyle;
            var material = default(Material);
            var matrixRaw = fillStyle.Matrix.Matrix;
            var flaMatrix2D = new FlaMatrix2D(new Vector4(matrixRaw.A, matrixRaw.B, matrixRaw.C, matrixRaw.D), new Vector2(matrixRaw.TX, matrixRaw.TY));
            var flaFillStyle = default(FlaFillStyle);
            var needSaveMaterial = false;

            if (fillStyle is FlaSolidColorFillStyleRaw) 
            {
                var solidColor = (FlaSolidColorFillStyleRaw)fillStyle;
                var matName = "material" + solidColor.Color.GetHashCode();
                material = TryLoadMaterial(matName);
                if (material == null)
                {
                    material = new Material(Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.SolidColor]));
                    var color = Color.black;
                    Color.TryParseHexString(solidColor.Color, out color);
                    material.SetColor("_Color", color);
                    material.name = matName;
                    needSaveMaterial = true;
                }
                flaFillStyle = new FlaFillStyle(material, flaMatrix2D,1, false);
            }
            else if (fillStyle is FlaBitmapFillRaw)
            {
                var bitmap = (FlaBitmapFillRaw)fillStyle;
                var matName = "material" + FolderAndFileUtils.RemoveUnacceptable(bitmap.BitmapPath).GetHashCode();
                var texture = default(Texture2D);
                material = TryLoadMaterial(matName);
                
                if (material == null)
                {
                    material = new Material(Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.Bitmap]));
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) +
                        FolderAndFileUtils.RemoveUnacceptable(bitmap.BitmapPath));
                    material.SetTexture("_Bitmap", texture);
                    material.name = matName;
                    needSaveMaterial = true;
                }
                else
                {
                    texture = (Texture2D) material.GetTexture("_Bitmap");
                }
                flaFillStyle = new FlaFillStyle(material, flaMatrix2D, (float)texture.width / (float)texture.height, bitmap.BitmapIsClipped);
            }
            else if (fillStyle is FlaLinearGradientFillStyleRaw)
            {
                var linearGradient = (FlaLinearGradientFillStyleRaw) fillStyle;
                var matName = "material" + linearGradient.GetHashCode();
                material = TryLoadMaterial(matName);
                if (material == null)
                {
                    material = new Material(Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.LinearGradient]));
                    ProcessFlaGradientEntries(linearGradient.GradientEntries, ref material);
                    material.name = matName;
                    needSaveMaterial = true;
                }
                flaFillStyle = new FlaFillStyle(material,flaMatrix2D,1,false);
            }
            else if (fillStyle is FlaRadialGradientFillStyleRaw)
            {
                var radialGradient = (FlaRadialGradientFillStyleRaw)fillStyle;
                var matName = "material" + radialGradient.GetHashCode();
                material = TryLoadMaterial(matName);
                if (material == null)
                {
                    material = new Material(Shader.Find(FillStyleShadersNames.ShaderNames[FillStyleTypeEnum.RadialGradient]));
                    Debug.Log(radialGradient.GradientEntries + " : " + material);
                    ProcessFlaGradientEntries(radialGradient.GradientEntries, ref material);
                    material.name = matName;
                    needSaveMaterial = true;
                }
                flaFillStyle = new FlaFillStyle(material,flaMatrix2D,1,false);
            }
            
            if (needSaveMaterial)
            {
                FolderAndFileUtils.CheckFolders(FoldersConstants.MaterialsFolder);
                AssetDatabase.CreateAsset(material, FolderAndFileUtils.GetAssetFolder(FoldersConstants.MaterialsFolder) + FolderAndFileUtils.RemoveUnacceptable(material.name) + ".mat");
                AssetDatabase.Refresh();    
            }
            return flaFillStyle;
        }

        private static Material TryLoadMaterial(string name)
        {
            return
                AssetDatabase.LoadAssetAtPath<Material>(
                    FolderAndFileUtils.GetAssetFolder(FoldersConstants.MaterialsFolder) + FolderAndFileUtils.RemoveUnacceptable(name)+".mat");
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
            var commands = edgeString.Split(' ').ToList();
            var operationIndex = 0;
          
            while (operationIndex < commands.Count)
            {
                switch (commands[operationIndex])
                {
                    case "!":
                        if (commands[operationIndex + 3].Contains("!"))
                        {
                            operationIndex += 3;
                            break;
                        }
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
            countOfPoints = Math.Max(3, countOfPoints);
            for (int i = 1; i < countOfPoints ; i++)
            {
                var delta = (float)i / (float)countOfPoints;
                curveList.Add(FlaMathUtils.CalculateQuadCurvePoint(point1, point2, controlpoint, delta));
            }
            return curveList;
        }


    }
}
