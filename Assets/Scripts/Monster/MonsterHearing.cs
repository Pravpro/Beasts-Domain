using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHearing : MonoBehaviour
{
    public GameObject monster;
    public GameObject player;

    public float[] hearRanges = {10f, 20f, 25f};
    public int rockHitHearLevel = 2;
    public int playerStandLevel = 0;
    public int playerMoveLevel = 1;
    public int playerRunLevel = 2;

    private PlayerController playerScript;
    private AIController aiScript;

    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        aiScript = monster.GetComponent<AIController>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aiScript.hp <= 0)
            return;
        if (HearPlayer() && !playerScript.IsInSafeZone())
            aiScript.Interrupt(player.transform.position, true);
    }

    bool HearPlayer()
    {
        float d = DistanceTo(player.transform.position);
        if (d < hearRanges[playerStandLevel])
            return true;
        if (playerScript.IsMoving() && d < hearRanges[playerMoveLevel])
            return true;
        if (playerScript.IsRunning() && d < hearRanges[playerRunLevel])
            return true;
        return false;
    }

    public void RockHit(Vector3 pos)
    {
        MakeSound(pos, rockHitHearLevel);
    }

    void MakeSound(Vector3 pos, int hearLevel = 0)
    {
        if (aiScript.hp <= 0)
            return;
        if (DistanceTo(pos) < hearRanges[hearLevel])
            aiScript.Interrupt(pos);
    }

    private float DistanceTo(Vector3 v, bool ignoreY = false)
    {
        Vector3 diff = transform.position - v;
        if (ignoreY)
            diff.y = 0;
        return diff.magnitude;
    }
}
