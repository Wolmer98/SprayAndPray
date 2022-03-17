using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPDrop : MonoBehaviour
{
    public int XP = 1;

    private bool m_consumed = false;

    private void Update()
    {
        var player = GameManager.Instance.Player;
        if (player == null || m_consumed)
            return;

        var dir = player.transform.position - transform.position;
        var dist = Vector3.Distance(player.transform.position, transform.position);
        transform.position += 15 / Mathf.Max(Mathf.Pow(dist, 3.0f), 0.01f) * dir * Time.deltaTime;

        if (dist < 0.5f)
        {
            player.AddXP(XP);
            m_consumed = true;
            Destroy(gameObject);
        }
    }
}
