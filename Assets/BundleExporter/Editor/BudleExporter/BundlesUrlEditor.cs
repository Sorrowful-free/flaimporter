using System.Collections.Generic;
using System.IO;
using Assets.BundleExporter.Data;
using Assets.BundleExporter.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace Assets.BundleExporter.Editor.BudleExporter
{
    public class BundlesUrlEditor : EditorWindow
    {
        [MenuItem("BundleExporter/BundlesUrl")]
        private static void Init()
        {
            var window = CreateInstance<BundlesUrlEditor>();
            window.Show();
        }

        

        private PlatformsInfo _infos = new PlatformsInfo
        {
            Infos = new List<PlatformInfo>()
            {
                new PlatformInfo
                {
                    Platform = RuntimePlatform.Android,
                    LanguagesInfos = new List<LanguageInfo>
                    {
                        new LanguageInfo
                        {
                            Language = SystemLanguage.English,
                            Url = "example.com",
                            IconPath = "Assets/icon"
                        }
                    }
                },
                new PlatformInfo
                {
                    Platform = RuntimePlatform.IPhonePlayer,
                    LanguagesInfos = new List<LanguageInfo>
                    {
                        new LanguageInfo
                        {
                            Language = SystemLanguage.English,
                            Url = "example.com",
                            IconPath = "Assets/icon"
                        }
                    }
                },
                new PlatformInfo
                {
                    Platform = RuntimePlatform.WP8Player,
                    LanguagesInfos = new List<LanguageInfo>
                    {
                        new LanguageInfo
                        {
                            Language = SystemLanguage.English,
                            Url = "example.com",
                            IconPath = "Assets/icon"
                            
                        }
                    }
                }
            }
        };
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("AddPlatform"))
            {
                _infos.Infos.Add(new PlatformInfo());
            }

            if (GUILayout.Button("RemovePlatform"))
            {
                if (_infos.Infos.Count >0)
                    _infos.Infos.RemoveAt(_infos.Infos.Count - 1);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal();
            GUILayout.Space(35);
            GUILayout.BeginVertical();
            foreach (var info in _infos.Infos)
            {
                info.Platform = (RuntimePlatform) EditorGUILayout.EnumPopup(info.Platform);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Language"))
                {
                    info.LanguagesInfos.Add(new LanguageInfo());
                }
                if (GUILayout.Button("Remove Language"))
                {
                    if (info.LanguagesInfos.Count >0)
                        info.LanguagesInfos.RemoveAt(info.LanguagesInfos.Count - 1);
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.BeginVertical();
                foreach (var languageInfo in info.LanguagesInfos)
                {
                    GUILayout.BeginVertical();
                    languageInfo.Language = (SystemLanguage)EditorGUILayout.EnumPopup(languageInfo.Language);
                    languageInfo.Url = EditorGUILayout.TextField("URL to file",languageInfo.Url);
                    languageInfo.IconPath = EditorGUILayout.TextField("Icon path",languageInfo.IconPath);
                    GUILayout.EndVertical();
                    EditorGUILayout.Separator();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            _infos.Version = EditorGUILayout.IntField("Version", _infos.Version);
            if (GUILayout.Button("SaveInFile"))
            {
                var path = EditorUtility.SaveFilePanel("Safe platforms","Assets","platforms","bytes");
                if (!File.Exists(path))
                {
                    File.Create(path).Close();    
                }
                var fileStream = File.Open(path, FileMode.Open);
                var data = _infos.Serialize();
                fileStream.Write(data,0,data.Length);
                fileStream.Close();
            }
        }
    }
}
