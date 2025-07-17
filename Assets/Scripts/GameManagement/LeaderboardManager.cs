using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LeaderboardManager : MonoBehaviourPunCallbacks {
    [System.Serializable]
    public struct PlayerData {
        public string name;
        public int kills;
        public int deaths;

        public PlayerData(string name, int kills, int deaths) {
            this.name = name;
            this.kills = kills;
            this.deaths = deaths;
        }
    }

    public GameObject leaderboardRowPrefab;
    public Transform leaderboardParent;

    public static LeaderboardManager Instance;

    private List<PlayerData> leaderboardEntries = new List<PlayerData>();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdatePlayerData(string name, int kills, int deaths) {
        int index = leaderboardEntries.FindIndex(p => p.name == name);

        kills /= 2;  // Patch fix for double add

        if (index >= 0)
            leaderboardEntries[index] = new PlayerData(name, kills, deaths);
        else
            leaderboardEntries.Add(new PlayerData(name, kills, deaths));

        // Send data to all clients using primitive arrays
        if (PhotonNetwork.IsMasterClient && photonView != null) {
            List<string> names = new List<string>();
            List<int> killList = new List<int>();
            List<int> deathList = new List<int>();

            foreach (var entry in leaderboardEntries) {
                names.Add(entry.name);
                killList.Add(entry.kills);
                deathList.Add(entry.deaths);
            }

            photonView.RPC("SyncLeaderboard", RpcTarget.All, names.ToArray(), killList.ToArray(), deathList.ToArray());
        }
    }

    [PunRPC]
    void SyncLeaderboard(string[] names, int[] kills, int[] deaths) {
        leaderboardEntries.Clear();
        for (int i = 0; i < names.Length; i++) {
            leaderboardEntries.Add(new PlayerData(names[i], kills[i], deaths[i]));
        }

        Debug.Log($"[SYNC] Received leaderboard of {names.Length} players.");
        ShowLeaderboard();
    }

    public void ShowLeaderboard() {
        foreach (Transform child in leaderboardParent) {
            Destroy(child.gameObject);
        }

        leaderboardEntries.Sort((a, b) => b.kills.CompareTo(a.kills));

        int rank = 1;
        foreach (PlayerData player in leaderboardEntries) {
            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardParent);
            var ui = row.GetComponent<LeaderboardRowUI>();
            if (ui != null)
                ui.Setup(player.name, player.kills, player.deaths, rank++);
        }
    }
    [PunRPC]
    public void ReceiveStatUpdate(string name, int kills, int deaths) {
        UpdatePlayerData(name, kills, deaths);
        Debug.Log("Updating Client Player data");
    }

}
