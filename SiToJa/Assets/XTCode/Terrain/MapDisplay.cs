using UnityEngine;

namespace XTCode.Terrain {
    public class MapDisplay : MonoBehaviour {
        public Renderer     textureRender;
        public MeshFilter   meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;

        public void DrawTexture(Texture2D texture) {
            this.textureRender.sharedMaterial.mainTexture = texture;
            this.textureRender.transform.localScale       = new Vector3( texture.width, 1, texture.height );
            this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;
        }

        public void DrawMesh(MeshDate meshDate, Texture2D texture) {
            this.meshFilter.sharedMesh                   = meshDate.CreateMesh();
            this.meshRenderer.sharedMaterial.mainTexture = texture;
            this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;
        }
    }
}