using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {
    // Components
    [SerializeField] private MeshRenderer meshRenderer=null;
    [SerializeField] private MeshFilter meshFilter=null;
    // References
    [SerializeField] private Material mat=null;
    
    
    
    private void Start() {
        Mesh mesh = new Mesh();
        
        float w = 5;
        float h = 10;
        Vector3[] vs = new Vector3[4];
        vs[0] = new Vector3(-w, -h);
        vs[1] = new Vector3(-w,  h);
        vs[2] = new Vector3( w,  h);
        vs[3] = new Vector3( w, -h);
        
        mesh.vertices = vs;
        mesh.triangles = new int[] {0,1,2, 0,2,3};
        
        meshRenderer.material = mat;
        meshFilter.mesh = mesh;
    }

}
