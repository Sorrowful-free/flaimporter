using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Assets.BundleExporter.Editor.Helpers;
using UnityEngine;

namespace Assets.FlaExporter.Editor.Utils
{
    public static class FlaMathUtils
    {
        public static int ParseFlaInteger(string flaNumber)
        {
            if (flaNumber.StartsWith("#"))
            {
                return ParseFlaHex(flaNumber)/256;
            }
            var indexOfS = flaNumber.ToLower().IndexOf("s");
            if (indexOfS >= 0)
            {
                return int.Parse(flaNumber.Substring(0, indexOfS));
            }
            var indexOfPoint = flaNumber.ToLower().IndexOf(".");
            if (indexOfPoint >= 0)
            {
                flaNumber = flaNumber.Substring(0, indexOfPoint);
            }
            
            return int.Parse(flaNumber);
        }

        public static int ParseFlaHex(string flaHex)
        {
            var parsedStrings = flaHex.Substring(1, flaHex.Length-1).Split('.');
            if (parsedStrings.Length > 1)
            {
                if (parsedStrings[1].Length < 2)
                {
                    parsedStrings[1] += "0";
                }
                if (parsedStrings[1].Length > 2)
                {
                    parsedStrings[1] = parsedStrings[1].Substring(0, 2);
                }
            }
            var parsedStr = parsedStrings[0] + parsedStrings[1];
            return int.Parse((parsedStr).ToLower(),NumberStyles.HexNumber);
        }

        public static Vector2 CalculateQuadCurvePoint(Vector2 p1,Vector2 p2,Vector2 c, float delta)
        {
            //опорный вектор 0-ого порядка
            var v1 = c - p1;
            v1 *= delta;
            v1 += p1;

            //опорный вектор 0-ого порядка
            var v2 = p2 - c;
            v2 *= delta;
            v2 += c;

            //опорный вектор 1-ого порядка и собственно результат
            var v3 = v2 - v1;
            v3 *= delta;
            v3 += v1;
            return v3;

        }
    }
}
