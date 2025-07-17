using UnityEngine;
using Photon.Pun;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviourPun {
    public int kills = 0;
    public int deaths = 0;
    public string playerName;

    [Header("UI")]
    public TextMeshProUGUI killText;
    public TextMeshProUGUI deathText;
    public GameObject statsUI;
    public Slider healthBar;

    void Start() {
        playerName = GetComponent<PlayerStateMachine>().PlayerName;

        if (photonView.IsMine) {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("kills", out object k))
                kills = (int)k;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("deaths", out object d))
                deaths = (int)d;

            if (statsUI != null) statsUI.SetActive(true);
        }
        else {
            if (statsUI != null) statsUI.SetActive(false);
        }

        StartCoroutine(DelayedLeaderboardInit());
    }

    System.Collections.IEnumerator DelayedLeaderboardInit() {
        yield return new WaitUntil(() => LeaderboardManager.Instance != null);

        if (PhotonNetwork.IsMasterClient) {
            LeaderboardManager.Instance.UpdatePlayerData(playerName, kills, deaths);
        }
    }

    private void Update() {
        if (killText != null) {
            killText.text = (kills/2).ToString();
        }
        if (deathText != null) {
            deathText.text = deaths.ToString();
        }

        if (healthBar != null && TryGetComponent(out PlayerHealth ph)) {
            healthBar.value = ph.health;
        }
    }

    [PunRPC]
    public void AddKill() {
        if (!photonView.IsMine) return;
        kills++;
        SyncStats();
    }

    [PunRPC]
    public void AddDeath() {
        if (!photonView.IsMine) return;
        deaths++;
        SyncStats();
    }

    void SyncStats() {
        PhotonView managerView = PhotonView.Get(LeaderboardManager.Instance);

        if (PhotonNetwork.IsMasterClient) {
            LeaderboardManager.Instance?.UpdatePlayerData(playerName, kills, deaths);
        }
        else if (managerView != null) {
            managerView.RPC("ReceiveStatUpdate", RpcTarget.MasterClient, playerName, kills, deaths);
        }
        else {
            Debug.Log("ManagerView is null!");
        }

            // Local cache for reconnecting or reloading
            Hashtable stats = new Hashtable {
        { "kills", kills },
        { "deaths", deaths }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(stats);
    }
}
