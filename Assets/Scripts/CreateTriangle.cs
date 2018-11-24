using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTriangle : MonoBehaviour {

    [SerializeField]
    private Material m_material;
    
    private Mesh m_mesh;

    // (1) 頂点座標（この配列のインデックスが頂点インデックス）
    private Vector3[] m_positions = new Vector3[]{
        new Vector3(0, 1, 0),
        new Vector3(1, -1, 0),
        new Vector3(-1, -1, 0)
    };

    // (2) ポリゴンを形成する頂点インデックスを順番に指定する
    private int[] m_triangles = new int[]{ 0, 1, 2 };

    // (3) 法線
    private Vector3[] m_normals = new Vector3[]{
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(0, 0, -1)
    };

    private void Awake () {
        m_mesh = new Mesh();

        // (4) Meshに頂点情報を代入
        m_mesh.vertices = m_positions;
        m_mesh.triangles = m_triangles;
        m_mesh.normals = m_normals;

        m_mesh.RecalculateBounds();
    }

    private void Update () {
        // (5) 描画
        Graphics.DrawMesh(m_mesh, Vector3.zero, Quaternion.identity, m_material, 0);
    }
    
    public Vector3 GetCenterOfGravity() {
        return new Vector3((m_positions[0].x+m_positions[1].x+m_positions[2] .x)/3,(m_positions[0].y+m_positions[1].y+m_positions[2] .y)/3, 0);
    }
    
    public void UpdateVertex(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) {
        
        m_positions[0] = vertex1;
        m_positions[1] = vertex2;
        m_positions[2] = vertex3;
        
        // (4) Meshに頂点情報を代入
        m_mesh.vertices = m_positions;
        
        Graphics.DrawMesh(m_mesh, Vector3.zero, Quaternion.identity, m_material, 0);
    }
}