using System.Linq;
using UnityEngine;

public class OutlineMeshMaker : MonoBehaviour
{
    [SerializeField]
    private Shader _outlineShader;
    
    [SerializeField]
    private float _outlineThickness = 0.002f;
    
    void Awake()
    {
        if (false)
        {
            var renderers = transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var r in renderers)
            {
                if (r.material.IsKeywordEnabled("_ALPHABLEND_ON"))
                    continue;

                var meshFilter = r.gameObject.GetComponent<MeshFilter>();
                
                var go = new GameObject(r.gameObject.name + "_Outline");
                go.transform.SetParent(r.transform, false);
                
                var outlineMeshFilter = go.AddComponent<MeshFilter>();
                outlineMeshFilter.mesh = meshFilter.mesh;
                
                var outlineMeshRenderer = go.AddComponent<MeshRenderer>();
                var outlineMaterial = new Material(_outlineShader);
                outlineMeshRenderer.material = outlineMaterial;
                outlineMaterial.mainTexture = r.material.mainTexture;
            }
        }

        {
            var renderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var r in renderers)
            {
                if (r.material.IsKeywordEnabled("_ALPHABLEND_ON"))
                    continue;

                var go = new GameObject(r.gameObject.name + "_Outline");
                go.transform.SetParent(r.transform, false);
                
                var outlineMeshRenderer = go.AddComponent<SkinnedMeshRenderer>();
                outlineMeshRenderer.sharedMesh = r.sharedMesh;
                outlineMeshRenderer.localBounds = r.localBounds;
                outlineMeshRenderer.rootBone = r.rootBone;
                outlineMeshRenderer.bones = r.bones.ToArray();
                for (int i = 0; i < r.sharedMesh.blendShapeCount; ++i)
                {
                    outlineMeshRenderer.SetBlendShapeWeight(i, r.GetBlendShapeWeight(i));
                }
                
                var outlineMaterial = new Material(_outlineShader);
                outlineMeshRenderer.material = outlineMaterial;
                outlineMaterial.mainTexture = r.material.mainTexture;
                outlineMaterial.SetFloat("_Thickness", _outlineThickness);
            }
        }
    }
}
