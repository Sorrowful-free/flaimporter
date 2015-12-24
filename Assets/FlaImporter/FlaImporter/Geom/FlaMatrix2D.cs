using System;
using UnityEngine;

namespace Assets.FlaImporter.FlaImporter.Geom
{
    [Serializable]
    public struct FlaMatrix2D
    {
        [SerializeField]
        public Vector4 ABCD;
        private Vector4 _OldABCD;
        [SerializeField]
        public Vector2 TXTY;
        private Vector2 _OldTXTY;

        public FlaMatrix2D(Vector4 abcd, Vector2 txty)
        {
            _OldABCD = ABCD = abcd;
            _OldTXTY = TXTY = txty;
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
    }
}
