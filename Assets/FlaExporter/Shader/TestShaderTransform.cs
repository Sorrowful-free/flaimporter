using UnityEngine;

namespace Assets.FlaExporter.Shader
{
    public class TestShaderTransform : MonoBehaviour
    {
        public float aspect;
        public Vector2 txty;
        public Vector2 scale;
        private void OnValidate()
        {
            if (GetComponent<MeshRenderer>().material == null)
            {
                GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().sharedMaterial;
            }
            GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_TextureAspect", aspect);
            GetComponent<MeshRenderer>().sharedMaterial.SetVector("_TextureMatrixABCD", new Vector4(scale.x, 0, 0, scale.y));
            GetComponent<MeshRenderer>().sharedMaterial.SetVector("_TextureMatrixTXTY", txty);
            
        }
    }
}

