using System;
using UnityEngine;

namespace Assets.FlaImporter.FlaImporter.Geom
{
    [Serializable]
    public struct FlaMatrix2D
    {
        [SerializeField] public Vector4 ABCD;
        private Vector4 _OldABCD;
        [SerializeField] public Vector2 TXTY;
        private Vector2 _OldTXTY;


        private Vector2 _position;
        private Vector2 _scale;
        private Vector2 _skew;

        public FlaMatrix2D(Vector4 abcd, Vector2 txty)
        {
            _OldABCD = ABCD = abcd;
            _OldTXTY = TXTY = txty;
            _position = Vector2.zero;
            _scale = Vector2.one;
            _skew = Vector2.zero;
        }

        public bool UpdateMatrix()
        {
            var flag = ABCD != _OldABCD || TXTY != _OldTXTY;
            if (flag)
            {
                _OldABCD = ABCD;
                _OldTXTY = TXTY;
            }
            return flag;
        }

        public Vector2 GetSkew()
        {
            _skew.Set(-Mathf.Asin(ABCD.z) * (180.0f / Mathf.PI), Mathf.Asin(ABCD.y) * (180.0f / Mathf.PI));
            return _skew; // -a for convert angles to unity
        }
        
        public float GetAngle()
        {
            var skew = GetSkew();
            //Debug.Log(string.Format("a={0},sx={1},sy={2}", (skew.x + skew.y) / 2.0f,skew.x,skew.y));
            return (skew.x + skew.y) / 2.0f;
        }

        public Vector2 GetPosition()
        {
            _position.Set(TXTY.x,-TXTY.y);
            return _position; // Vector2(,-flaMatrix.TY);//-ty for convert to unity
        }

        public Vector2 GetScale() 
        {
            var sx = Mathf.Sqrt(Mathf.Pow(ABCD.x, 2) + Mathf.Pow(ABCD.z, 2));
            var sy = Mathf.Sqrt(Mathf.Pow(ABCD.y, 2) + Mathf.Pow(ABCD.w, 2));
            _scale.Set(sx,sy);
            return new Vector2(sx,sy);
        }
    }
}
