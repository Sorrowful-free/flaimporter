using System;
using System.Collections;
using System.Linq;
using Assets.FlaExporter.Editor.Data.RawData.FrameElements;
using Assets.FlaExporter.Editor.EditorCoroutine;
using Assets.FlaExporter.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.FlaExporter.Editor.FlaProcessors
{
    public static class FlaFrameElementProcessor
    {
        public static IEnumerator ProcessFlaElement(FlaFrameElementRaw element,Action<GameObject> callback)
        {
            
            var shape = element as FlaShapeRaw;
            var instance = element as FlaBaseInstanceRaw;
            
            if (shape != null)
            {
                yield return FlaShapeProcessor.ProcessFlaShape(shape, callback).StartAsEditorCoroutine();
                yield break;
            }

            if (instance != null)
            {
                yield return ProcessFlaInstance(instance, callback).StartAsEditorCoroutine();
            }
        }

        private static IEnumerator ProcessFlaInstance(FlaBaseInstanceRaw instance, Action<GameObject> callback)
        {
            var symbolInstance = instance as FlaSymbolInstanceRaw;
            var bitmapInstance = instance as FlaBitmapInstanceRaw;
            if (symbolInstance != null)
            {
                var symbol = ProcessFlaSymbolInstance(symbolInstance);
                if (callback != null)
                {
                    callback(symbol);
                }
                yield break;
            }
            if (bitmapInstance != null)
            {
                var bitmap = ProcessFlaBitmapInstance(bitmapInstance);
                if (callback != null)
                {
                    callback(bitmap);
                }
            }
        }

        private static GameObject ProcessFlaSymbolInstance(FlaSymbolInstanceRaw instance)
        {
            var symbolGO = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.SymbolsFolder) + FolderAndFileUtils.RemoveUnacceptable(instance.LibraryItemName) + ".prefab"));
            symbolGO.name = FolderAndFileUtils.RemoveUnacceptable(instance.LibraryItemName);
            if (instance.Matrix != null && instance.Matrix.Matrix != null)
            {
                instance.Matrix.Matrix.CopyMatrix(symbolGO.transform);
            }
            return symbolGO;
        }

        private static GameObject ProcessFlaBitmapInstance(FlaBitmapInstanceRaw instance)
        {
            var bitmapSymbolGO = new GameObject(FolderAndFileUtils.RemoveUnacceptable(instance.LibraryItemName));
            var bitmapSriteRenderer = bitmapSymbolGO.AddComponent<SpriteRenderer>();
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(FolderAndFileUtils.GetAssetFolder(FoldersConstants.BitmapSymbolsTextureFolderFolder) + FolderAndFileUtils.RemoveUnacceptable(instance.LibraryItemName));
            var spritesAsObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(texture));
            var sprite = spritesAsObjects.FirstOrDefault(e => e.name == FolderAndFileUtils.RemoveExtention(instance.LibraryItemName)) as Sprite;
            bitmapSriteRenderer.sprite = sprite;
            instance.Matrix.Matrix.CopyMatrix(bitmapSymbolGO.transform);
            return bitmapSymbolGO;
        }

    }
}
