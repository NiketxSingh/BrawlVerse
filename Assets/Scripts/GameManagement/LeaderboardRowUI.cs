using UnityEngine;
using TMPro;

public class LeaderboardRowUI : MonoBehaviour {
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI deathText;
    public TextMeshProUGUI rankText;

    public void Setup(string playerName, int kills, int deaths, int rank) {
        nameText.text = playerName;
        killText.text = kills.ToString();
        deathText.text = deaths.ToString();
        rankText.text = $"#{rank}";
    }
}
