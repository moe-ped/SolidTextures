using UnityEngine;
using System.Collections;

public class MeshStretcher : MonoBehaviour 
{
    // TODO: rename
    public Vector3 Factor = Vector3.one;

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
            vertices[i].x = OriginalVertices[i].x * Factor.x;
            vertices[i].y = OriginalVertices[i].y * Factor.y;
            vertices[i].z = OriginalVertices[i].z * Factor.z;
        }

        Mesh.vertices = vertices;
        Mesh.RecalculateBounds();
    }

    public void Reset ()
    {
        Mesh.vertices = OriginalVertices;
    }
}
