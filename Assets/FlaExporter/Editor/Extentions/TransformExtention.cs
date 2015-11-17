using System.Collections.Generic;
using System.Linq;
using Assets.BundleExporter.Editor.Helpers;
using UnityEngine;

namespace Assets.FlaExporter.Editor.Extentions
{
    public static class TransformExtention
    {
        public static string GetTransformPath(this Transform transform)
        {
            var names = new List<string>();
            
            var parent = transform.parent;
            names.Add(transform.name);
            while (parent != null)
            {
                names.Add(parent.name);
                parent = parent.parent;
            }
            names.Reverse();
            return names.JoinToString("/");
        }
    }
}
