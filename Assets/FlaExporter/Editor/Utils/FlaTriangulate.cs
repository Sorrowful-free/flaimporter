using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.FlaExporter.Editor.Utils
{
    public static class FlaTriangulate
    {
        private const float Epsilon = 0.0001f;
               

        public static float Area(List<Vector2> contour)
        {
            var area = 0.0f;
            var n = contour.Count;
            var p = n-1;
            var q = 0;
            for(; q<n ; p=q++)
            {
                area+= contour[p].x * contour[q].y - contour[q].x * contour[p].y;
            }

            return area *0.5f;
        }

        public static bool InsideTriangle(Vector2 vecA, Vector2 vecB, Vector2 vecC, Vector2 p)
        {
            var a = Vector2.zero;
            var b = Vector2.zero;
            var c = Vector2.zero;
            var ap = Vector2.zero;
            var bp = Vector2.zero;
            var cp = Vector2.zero;

            a = vecC - vecB;
            b = vecA - vecC;
            c = vecB - vecA;
            ap = p - vecA;
            bp = p - vecB;
            cp = p - vecC;

            var aCROSSbp = a.x * bp.y - a.y * bp.x;
            var cCROSSap = c.x * ap.y - c.y * ap.x;
            var bCROSScp= b.x * cp.y - b.y * cp.x;

            return ((aCROSSbp >= 0.0) && (bCROSScp >= 0.0) && (cCROSSap >= 0.0));
        }

        public static bool Snip(List<Vector2> contour, int u, int v,int w,int n, List<int> indexes)
        {
            var _p = 0;
            var a = contour[indexes[u]];
            var b = contour[indexes[v]];
            var c = contour[indexes[w]];
            var p = Vector2.zero;

            if(Epsilon > (((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x)))) return false;
            for(_p = 0; _p < n; _p++)
            {
                if( (_p == u) || (_p == v) || (_p == w) ) continue;
                p = contour[indexes[_p]];
                if(InsideTriangle(a,b,c,p)) return false;
            }
            return true;
        }

        public static List<int> Process(List<Vector2>contour)
        {
            var n = contour.Count;
            var result= new List<int>();

            if(n < 3) return null;

            var indexes = new List<int>();
            var v = 0;

            if (Area(contour) > 0)
            {
                for (; v < n; v++)
                {
                    indexes.Add(v);
                }
            }
            else
            {
                for (; v < n; v++)
                {
                    indexes.Add((n-1) - v);
                }
            }

            var nv = n;
            var count= 2*nv;
            var m = 0;
            v = nv-1;
            for(;nv > 2;)
            {
                if ((count--) <= 0)
                {
                    return null;
                }
                var u = v;
                if (nv <= u)
                {
                    u = 0;
                }
                v = u+1;
                if (nv <= v)
                {
                    v = 0;
                }
                var w = v+1;
                if (nv <= w)
                {
                    w = 0;
                }

                if(Snip(contour,u,v,w,nv,indexes))
                {
                    result.Add(indexes[u]);
                    result.Add(indexes[v]);
                    result.Add(indexes[w]);

                    m++;

                    var s = v;
                    var t = v+1;

                    for (; t < nv;)
                    {
                        indexes[s++] = indexes[t++];
                    }
                    nv--;
                    count = 2*nv;
                }
            }

            return result;

        }
      
    }
}
