using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.FlaImporter.Editor.Data.RawData.Geom;
using UnityEngine;

namespace Assets.FlaImporter.Editor.Extentions.FlaExtentionsRaw
{
    public static class FlaPointRawExtention
    {
        public static Vector2 ToVector2(this FlaPointRaw point)
        {
            return new Vector2(point.X,point.Y);
        }
    }
}
