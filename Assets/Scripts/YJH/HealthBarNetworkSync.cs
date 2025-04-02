using UnityEngine;
using Photon.Pun;

public class HealthBarNetworkSync : MonoBehaviourPun
{
    private HealthBar healthBar;

    void Awake()
    {
        healthBar = GetComponent<HealthBar>();
    }

    void FixedUpdate()
    {
        if (healthBar == null || healthBar.target == null || healthBar.target.character == null)
            return;

        if (photonView.IsMine)
        {
            float cur = healthBar.target.character.CurHP;
            float max = healthBar.target.character.HP;
            photonView.RPC("UpdateHPBar", RpcTarget.Others, cur, max);
        }
    }

    [PunRPC]
    public void UpdateHPBar(float cur, float max)
    {
        if (healthBar != null)
        {
            healthBar.UpdateFromNetwork(cur, max);
        }
    }
}