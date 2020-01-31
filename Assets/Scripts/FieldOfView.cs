using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float fov;
    public float viewDistance;
    public GameObject obj;

    private Mesh mesh;
    private Vector3 origin;
    private MeshCollider meshCol;
    private AIController script;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        obj = GameObject.FindGameObjectWithTag("Monster");
        script = obj.GetComponent<AIController>();
        ComputeFOV();
        //GetComponent<MeshCollider>().sharedMesh = mesh;
        //gameObject.AddComponent<MeshCollider>();
    }

    // Start is called before the first frame update
    void FixedUpdate()
    {
        ComputeFOV();
    }

    void ComputeFOV()
    {
        origin = obj.transform.position;
        origin.y = 0.1f;
        int rayCount = 60;
        float angle = GetAngleFromVector(obj.transform.forward) + fov / 2f;
        float angleIncrement = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIdx = 1;
        int triangleIdx = 0;
        script.playerInSight = false;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex; 
            RaycastHit hitInfo; 
            bool raycastHit = Physics.Raycast(origin, GetVectorFromAngle(angle), out hitInfo, viewDistance);

            if (raycastHit){
                // hit
                vertex = hitInfo.point;
                if (hitInfo.collider.tag == "Player")
                    script.playerInSight = true;
            }
            else {
                // no hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }

            vertices[vertexIdx] = vertex;
            if (i > 0) 
            {
                triangles[triangleIdx + 0] = 0;
                triangles[triangleIdx + 1] = vertexIdx - 1;
                triangles[triangleIdx + 2] = vertexIdx;
                triangleIdx += 3;
            }
            vertexIdx++;
            angle -= angleIncrement;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

    public float GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;
        return n;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // void OnCollisionEnter(Collision col)
    // {
    //     if (col.collider.tag == "Player")
    //     {
    //         script.playerInSight = true;
    //         Debug.Log("Player in sight");
    //     }
    // }

    // void OnCollisionExit(Collision col)
    // {
    //     if (col.collider.tag == "Player")
    //     {
    //         script.playerInSight = false;
    //         Debug.Log("Player leave sight");
    //     }
    // }
}
