using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathDebugger : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.AI.NavMeshAgent agentToDebug;

    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agentToDebug.hasPath)
        {
            lineRenderer.positionCount = agentToDebug.path.corners.Length;
            lineRenderer.SetPositions(agentToDebug.path.corners);
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
