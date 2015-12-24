using UnityEngine;

namespace Assets.FlaImporter.Editor.Extentions
{
    public static class AnimationCurveExtention
    {
        public static void SetCurveLinear(this AnimationCurve curve)
        {
            for (int i = 0; i < curve.keys.Length; ++i)
            {
                float intangent = 0;
                float outtangent = 0;
                bool intangent_set = false;
                bool outtangent_set = false;
                Vector2 point1;
                Vector2 point2;
                Vector2 deltapoint;
                Keyframe key = curve[i];

                if (i == 0)
                {
                    intangent = 0; intangent_set = true;
                }

                if (i == curve.keys.Length - 1)
                {
                    outtangent = 0; outtangent_set = true;
                }

                if (!intangent_set)
                {
                    point1.x = curve.keys[i - 1].time;
                    point1.y = curve.keys[i - 1].value;
                    point2.x = curve.keys[i].time;
                    point2.y = curve.keys[i].value;

                    deltapoint = point2 - point1;

                    intangent = deltapoint.y / deltapoint.x;
                }
                if (!outtangent_set)
                {
                    point1.x = curve.keys[i].time;
                    point1.y = curve.keys[i].value;
                    point2.x = curve.keys[i + 1].time;
                    point2.y = curve.keys[i + 1].value;

                    deltapoint = point2 - point1;

                    outtangent = deltapoint.y / deltapoint.x;
                }

                key.inTangent = intangent;
                key.outTangent = outtangent;
                curve.MoveKey(i, key);
            }
        }
    }
}
