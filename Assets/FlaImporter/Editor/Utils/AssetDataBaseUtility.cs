using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.FlaImporter.Editor.Utils
{
    public static class AssetDataBaseUtility
    {
        public static void SaveDocument(GameObject document)
        {
            
        }

        public static void SaveSymbol(GameObject symbol)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.SymbolsFolder);
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + FolderAndFileUtils.RemoveUnacceptable(symbol.name) + ".prefab", symbol);
        }

        public static GameObject LoadSymbol(string symbolName)
        {
            return
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) +
                    FolderAndFileUtils.RemoveUnacceptable(symbolName) + ".prefab");
        }


        public static AnimatorController CreateAnimatorController(string name)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.AnimatorControllerFolder);
            var animationController = AnimatorController.CreateAnimatorControllerAtPath(FolderAndFileUtils.GetAssetFolder(FoldersConstants.AnimatorControllerFolder) + FolderAndFileUtils.RemoveUnacceptable(name) + "_AC.controller");
            return animationController;
        }

        public static void SaveAnimationClip(AnimationClip animationClip)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.AnimationClipsFolder);
            AssetDatabase.CreateAsset(animationClip, FolderAndFileUtils.GetAssetFolder(FoldersConstants.AnimationClipsFolder) + FolderAndFileUtils.RemoveUnacceptable(animationClip.name) + ".anim");
        }

        public static void CopyAndSaveBitmap(string copyFrom , string bitmapName)
        {
            var filePath = copyFrom;
            if (!File.Exists(filePath))
            {
                Debug.Log("file not found " + filePath);
               // continue;
            }

            var bytes = File.ReadAllBytes(filePath);
            SaveBitmapTo(bytes, bitmapName);
        }

        public static void SaveBitmapTo(byte[] bitmapData,string bitmapName)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.BitmapSymbolsTextureFolderFolder);
            if (!File.Exists(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) + FolderAndFileUtils.RemoveUnacceptable(bitmapName)))
            {
                var file = File.Open(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) + FolderAndFileUtils.RemoveUnacceptable(bitmapName), FileMode.OpenOrCreate);
             //   var bytes = zipFileEntry.ToByteArray();
                file.Write(bitmapData, 0, bitmapData.Length);
                file.Close();
                AssetDatabase.ImportAsset(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) + FolderAndFileUtils.RemoveUnacceptable(bitmapName));
                AssetDatabase.Refresh();
            }

        }

        public static void SaveShape(GameObject shape)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.ShapesFolder);
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.ShapesFolder) + shape.name + ".prefab", shape);
        }

        public static GameObject LoadShape(string shapeName)
        {
           return AssetDatabase.LoadAssetAtPath<GameObject>(
                   FolderAndFileUtils.GetAssetFolder(FoldersConstants.ShapesFolder) + shapeName + ".prefab");
        }

        public static void SaveEdge(GameObject edge)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.EdgesFolder);
            PrefabUtility.CreatePrefab(FolderAndFileUtils.GetAssetFolder(FoldersConstants.EdgesFolder) + edge.name + ".prefab", edge);
        }
        
        public static GameObject LoadEdge(string edgeName)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(
                    FolderAndFileUtils.GetAssetFolder(FoldersConstants.EdgesFolder) + edgeName + ".prefab");
        }
        public static void SaveEdgeMesh(Mesh edge)
        {
            FolderAndFileUtils.CheckFolders(FoldersConstants.EdgesFolder);
            AssetDatabase.CreateAsset(edge, FolderAndFileUtils.GetAssetFolder(FoldersConstants.EdgesFolder) + edge.name + ".asset");
        }


    }
}
