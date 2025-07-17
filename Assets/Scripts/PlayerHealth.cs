using System.Collections;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPun
{
    public float health = 100f;
    public PlayerStateMachine _playerStateMachine;  // Reference to your existing shield script
    public bool isLocalPlayer;

    private bool isDead = false;
    void Start()
    {
        if (_playerStateMachine == null)
        {
            _playerStateMachine = GetComponent<PlayerStateMachine>(); // Try auto-assign if on same object
        }
    }

    [PunRPC]
    public void TakeDamage(int damageAmount, int attackerViewID) {
        if (isDead) return; // Already dead, don't continue

        if (_playerStateMachine != null && _playerStateMachine.isShieldActive) {
            Debug.Log("Shield is active! No damage taken.");
            return;
        }

        health -= damageAmount;
        AudioManager.instance.PlaySFX(AudioManager.instance.damage);

        Debug.Log("Player health: " + health);

        if (health <= 0 && !isDead) {
            isDead = true; // Set flag to prevent multiple triggers
            Debug.Log("Player died!");

            PhotonView attackerView = PhotonView.Find(attackerViewID);

            if (attackerView != null && attackerView.TryGetComponent(out PlayerStats killerStats)) {
                attackerView.RPC("AddKill", RpcTarget.All);
            }

            if (TryGetComponent(out PlayerStats victimStats)) {
                photonView.RPC("AddDeath", RpcTarget.All);
            }

            Die();
        }
    }


    void Die() {
        if (photonView.IsMine) {
            RoomManager.Instance.StartRespawnCountdown(); 
            PhotonNetwork.Destroy(gameObject);            
        }
    }
}
