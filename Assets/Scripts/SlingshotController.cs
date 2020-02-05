using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotController : MonoBehaviour
{
    public GameObject throwObject, player;
    public LaunchArcMesh launchArc;
    public int angleAdd;
    GameObject arc = null;


    private void Update()
    {
        // Activate slingshot
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Create the Launch Arc
            arc = Instantiate(launchArc.gameObject,
                              transform.position,
                              player.transform.rotation) as GameObject;
        }
        // Make arc follow slingshot
        if (arc != null)
        {
            arc.transform.position = transform.position;
            arc.transform.rotation = player.transform.rotation;
        }

        // Release Slinghot
        if (Input.GetKeyUp(KeyCode.V))
        {
            // Create the throwable object
            GameObject m_rock = Instantiate(throwObject,
                                            transform.position,
                                            player.transform.rotation) as GameObject;
            Rigidbody m_rb = m_rock.GetComponent<Rigidbody>();

            //force = LaunchArcMesh.velocity * m_rb.mass * 35;
            // m_rb.AddForce((player.transform.forward + Vector3.up) * force);
            // param of AddForce Forcemode.impulse

            LaunchArcMesh arcScript = arc.GetComponent<LaunchArcMesh>();
            m_rb.AddForce(Quaternion.AngleAxis(arcScript.angle, arc.transform.right) * arc.transform.up * arcScript.velocity, ForceMode.Impulse);
            //currentRigidbody.AddForce(Quaternion.AngleAxis((launchArcRenderer.angle % 180f) - 90, launchArcRenderer.transform.forward) * launchArcRenderer.transform.up * launchArcRenderer.velocity * throwMultiplier, ForceMode.Impulse);

            // Destroy Arc when slingsot launched
            Destroy(arc);
        }
    }

}
