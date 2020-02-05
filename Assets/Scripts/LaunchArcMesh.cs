using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes sure we always have a LineRenderer
[RequireComponent(typeof(MeshFilter))]
public class LaunchArcMesh : MonoBehaviour
{
    Mesh mesh;

    public float meshWidth;
    public float velocity, angle;
    // For controling how many segments arc will have (influences smoothness/sharpness)
    public int resolution = 10;

    float g; // Force of Gravity on y axis
    float radianAngle;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    // When the value inside the inspector of this class changes, run this code
    void OnValidate()
    {
        // Check if lr is not null and that game is playing
        if (mesh != null && Application.isPlaying)
        {
            MakeArcMesh(CalculateArcArray());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        velocity = 6;
        angle = 45;
        MakeArcMesh(CalculateArcArray());
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.U))
        {
            if (velocity < 9)
            {
                velocity += 0.15f;
                MakeArcMesh(CalculateArcArray());
            }
        }
        if (Input.GetKey(KeyCode.I))
        {
            if (velocity > 4)
            {
                velocity -= 0.15f;
                MakeArcMesh(CalculateArcArray());
            }
        }
        
    }

    // Populate LineRenderer with appropriate settings
    void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2];

        for (int i = 0; i <= resolution; i++)
        {
            // set vertices
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].x);
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].x);

            // set triangles
            if (i != resolution)
            {
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = (i + 1) * 2;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = i * 2 + 1;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                triangles[i * 12 + 6] = i * 2;
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = i * 2 + 1;
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = (i + 1) * 2;
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }
    }

    // Create Array od Vector 3 positions for arc
    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            // Get time t (0 < t < 1)
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    // Calculate height and distance of each vertex
    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * Mathf.Pow(velocity * Mathf.Cos(radianAngle), 2)));

        return new Vector3(x, y);
    }


}
