using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.BundleExporter.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace Assets.BundleExporter.Editor.BudleExporter
{
    public class BundleExporterWindow : EditorWindow
    {
        public static void ShowWindow(List<string> files)
        {
            var window = CreateInstance<BundleExporterWindow>();
            window._files = new Dictionary<string, bool>();
            foreach (var file in files)
            {
                window._files.Add(file,true);
            }
            window.Show();
        }

        private Dictionary<string, bool> _files;
        private Vector2 _scrollPosition;
        private AssetTypeEnum _assetType;
        private string _fileName = "exampleFileName";
        private List<BuildTarget>  _exportPlatform = new List<BuildTarget>
        {
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.WP8Player
        };

        private void OnEnable()
        {
            if (_files == null)
            {
                return;
            }
        }
        private void OnGUI()
        {
            if (_files == null)
            {
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("AddPlatform"))
            {
                _exportPlatform.Add(BuildTarget.Android);
            }
            if (GUILayout.Button("RemovePlatform"))
            {
                if(_exportPlatform.Count >0)
                    _exportPlatform.RemoveAt(_exportPlatform.Count-1);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            for (int i = 0; i < _exportPlatform.Count; i++)
            {
                _exportPlatform[i] = (BuildTarget)EditorGUILayout.EnumPopup(_exportPlatform[i]);
            }
            GUILayout.EndVertical();
            EditorGUILayout.Separator();
            _assetType = (AssetTypeEnum)EditorGUILayout.EnumPopup("Select", _assetType);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SelectAll"))
            {
                foreach (var key in _files.Keys.ToArray())
                {
                    if (IsAssetType(key, _assetType))
                    {
                        _files[key] = true;
                    }
                }
            }
            if (GUILayout.Button("DeselectAll"))
            {
                foreach (var key in _files.Keys.ToArray())
                {
                    if (IsAssetType(key, _assetType))
                    {
                        _files[key] = false;
                    }
                }
            }
            GUILayout.EndHorizontal();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition,false,true);
            
            foreach (var file in _files.Keys.ToArray())
            {
                if (IsAssetType(file, _assetType))
                {
                    _files[file] = GUILayout.Toggle(_files[file],file);
                }
            }
            EditorGUILayout.EndScrollView();
            
            _fileName = EditorGUILayout.TextField("FileName",_fileName);
            if (GUILayout.Button("Save"))
            {
                var path = EditorUtility.SaveFolderPanel("save", "Assets", "assets");
                var filesToSave = _files.Where(e => e.Value).Select(e => e.Key);

                foreach (var platform in _exportPlatform)
                {
                    if (!Directory.Exists(path + "/" + platform))
                    {
                        Directory.CreateDirectory(path + "/" + platform);
                    }
                    Debug.Log(string.Format("platform:{0} assets:{1}", platform, filesToSave.JoinToString("\n")));
                    BuildPipeline.BuildAssetBundles(path + "/" + platform, new AssetBundleBuild[]
                    {
                        new AssetBundleBuild
                        {
                            assetBundleName = _fileName,
                            assetBundleVariant = "unity3d",
                            assetNames = filesToSave.ToArray()
                        } 
                    },BuildAssetBundleOptions.None,platform);
                    File.Delete(path + "/" + platform+"/"+platform);
                    File.Delete(path + "/" + platform + "/" + platform + ".manifest");
                }
            }
        }

        private bool IsAssetType(string path, AssetTypeEnum type)
        {
            var lowerPath = path.ToLower();
            switch (type)
            {
                case AssetTypeEnum.Prefabs:
                    return lowerPath.EndsWith(".prefab");
                case AssetTypeEnum.Textures:
                    return lowerPath.EndsWith(".psd") 
                        || lowerPath.EndsWith(".tiff")
                        || lowerPath.EndsWith(".jpg") 
                        || lowerPath.EndsWith(".tga")
                        || lowerPath.EndsWith(".png")
                        || lowerPath.EndsWith(".bmp")
                        || lowerPath.EndsWith(".gif")
                        || lowerPath.EndsWith(".iff")
                        || lowerPath.EndsWith(".pict");
                case AssetTypeEnum.AudioClips:
                    return lowerPath.EndsWith(".aif")
                        || lowerPath.EndsWith(".wav")
                        || lowerPath.EndsWith(".mp3")
                        || lowerPath.EndsWith(".ogg");
                case AssetTypeEnum.TextAssets:
                    return lowerPath.EndsWith(".txt")
                        || lowerPath.EndsWith(".html")
                        || lowerPath.EndsWith(".htm")
                        || lowerPath.EndsWith(".xml")
                        || lowerPath.EndsWith(".bytes")
                        || lowerPath.EndsWith(".json")
                        || lowerPath.EndsWith(".csv")
                        || lowerPath.EndsWith(".yaml")
                        || lowerPath.EndsWith(".fnt");
                case AssetTypeEnum.Materials:
                    return lowerPath.EndsWith(".mat");
                case AssetTypeEnum.Fonts:
                    return lowerPath.EndsWith(".ttf")
                        || lowerPath.EndsWith(".otf");
                case AssetTypeEnum.Shader:
                    return lowerPath.EndsWith(".shader");
                case AssetTypeEnum.All:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum AssetTypeEnum
    {
        All,
        Prefabs,
        Textures,
        AudioClips,
        Materials,
        TextAssets,
        Fonts,
        Shader
      
    }
}
