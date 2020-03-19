using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossController : MonoBehaviour
{
    public GameObject mossAreaObj;
    public float mossDisappearTime = 30f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "Wall_025x3x1 (4)")//(col.tag != "Monster" && col.tag != "Moss" && col.name != "Arena")
        {
            // Debug.Log("moss hit: " + col.name);
            Vector3 pos = transform.position;
            pos.y = -0.1f;
            GameObject mossArea = Instantiate(mossAreaObj, pos, transform.rotation);
            Destroy(mossArea, mossDisappearTime);
            Destroy(gameObject);
        }
    }
}
