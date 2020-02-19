using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float fov = 120f;
    public float fovVertical = 50f;
    public float viewDistance = 15f;
    // this is actually number of triangles
    public int rayCount = 40;
    public int fovCount = 5;
    public GameObject monster;

    private Mesh mesh;
    private Vector3 origin;
    private MeshCollider meshCol;
    private AIController script;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        monster = GameObject.FindGameObjectWithTag("Monster");
        script = monster.GetComponent<AIController>();
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
        Vector3 forward = monster.transform.forward;
        Vector3 right = monster.transform.right;
        Vector3 up = monster.transform.up;
        
        origin = monster.transform.position;
        //origin.y -= 1f;
        
        float angleIncHoriz = fov / rayCount;
        float angleVert = - fovVertical / 2f;
        float angleIncVert = fovVertical / fovCount;

        Vector3[] vertices = new Vector3[1 + (rayCount + 1) * fovCount];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3 * fovCount];

        vertices[0] = origin;

        int vertexIdx = 1;
        int triangleIdx = 0;
        script.playerInSight = false;
        for (int j = 0; j < fovCount; j++)
        {
            float angleHoriz = - fov / 2f;
            Vector3 curForward, curUp;
            curForward = Quaternion.AngleAxis(angleVert, right) * forward;
            curUp = Quaternion.AngleAxis(angleVert, right) * up;

            for (int i = 0; i <= rayCount; i++)
            {
                Vector3 vertex; 
                RaycastHit hitInfo; 
                bool raycastHit = Physics.Raycast(origin, GetVectorFromAngle(curForward, curUp, angleHoriz), out hitInfo, viewDistance);

                if (raycastHit){
                    // hit
                    vertex = hitInfo.point;
                    if (hitInfo.collider.tag == "Player")
                        script.playerInSight = true;
                }
                else {
                    // no hit
                    vertex = origin + GetVectorFromAngle(curForward, curUp, angleHoriz) * viewDistance;
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
                angleHoriz += angleIncHoriz;
            }
            angleVert += angleIncVert;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public Vector3 GetVectorFromAngle(Vector3 forward, Vector3 up, float angle)
    {
        // float angleRad = angle * (Mathf.PI / 180f);
        // return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
        return Quaternion.AngleAxis(angle, up) * forward;
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
