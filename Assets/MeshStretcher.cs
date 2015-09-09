using UnityEngine;
using System.Collections;

public class MeshStretcher : MonoBehaviour {

    public float Factor = 1;

    private Vector3[] OriginalVertices;

    private Mesh _mesh;
    private Mesh Mesh
    {
        get
        {
            if (_mesh == null)
            {
                _mesh = GetComponent<MeshFilter>().sharedMesh;
                OriginalVertices = _mesh.vertices;
            }
            return _mesh;
        }
    }

	public void Stretch ()
    {
        Vector3[] vertices = Mesh.vertices;
        for (int i = 0; i < vertices.Length; i++ )
        {
            vertices[i] *= Factor;
        }

        Mesh.vertices = vertices;
        Mesh.RecalculateBounds();
    }

    public void Reset ()
    {
        Mesh.vertices = OriginalVertices;
    }
}
