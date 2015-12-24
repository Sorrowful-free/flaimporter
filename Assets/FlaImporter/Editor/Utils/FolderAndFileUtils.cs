using System.IO;
using System.Linq;
using Assets.FlaImporter.Editor.Extentions;
using UnityEngine;

namespace Assets.FlaImporter.Editor.Utils
{
    public static class FolderAndFileUtils
    {
        public static string GetDirectoryPath(string filePath)
        {
            var path = filePath.Split('/', '\\');
            path = path.Take(path.Length - 1).ToArray();
            var stringPath = path.JoinToString("/");
            if (stringPath.EndsWith("/"))
            {
                stringPath = stringPath.Substring(0, stringPath.Length - 1);
            }
            return stringPath;
        }
        public static void CheckFolders(string folder)
        {
            if (!Directory.Exists(Application.dataPath + folder))
            {
                Directory.CreateDirectory(Application.dataPath + folder);
            }
        }

        public static string GetAssetFolder(string path) 
        {
            if (path.StartsWith("/") || path.StartsWith("\\"))
            {
                path = path.Substring(1, path.Length - 1);
            }
            if (!path.EndsWith("/"))
                path += "/";
            return "Assets/" + path;
        }

        public static string RemoveUnacceptable(string source)
        {
            return source.Replace("@", "")
                .Replace("#", "")
                .Replace("$", "")
                .Replace("%", "")
                .Replace("^", "")
                .Replace("&", "")
                .Replace("*", "")
                .Replace("/", "_")
                .Replace("\\", "_"); 
        }


        public static string RemoveExtention(string filePath)
        {
            var index = filePath.LastIndexOf(".");
            return filePath.Substring(0,index);
        }
    }
}
