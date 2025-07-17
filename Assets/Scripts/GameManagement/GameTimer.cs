using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using static UnityEngine.Tilemaps.Tilemap;

public class GameTimer : MonoBehaviourPun {
    [Header("Game Settings")]
    public float gameDuration = 60f; // Game duration in seconds
    private float remainingTime;
    private bool gameStarted = false;
    public bool gameEnded = false;

    private float syncTimer = 1f;

    [Header("UI References")]
    public TextMeshProUGUI timerText;

    public static GameTimer Instance;
    public GameObject gameTimerCanvas;
    public GameObject endGameScreen;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        remainingTime = gameDuration;
        if (gameTimerCanvas != null)
            gameTimerCanvas.SetActive(false);
        if (endGameScreen != null)
            endGameScreen.SetActive(false);

        gameEnded = false;
    }

    void Start() {
        if (PhotonNetwork.IsMasterClient) {
            Debug.Log("MasterClient: waiting for players...");
            // Only MasterClient will control the timer
            // Wait for a signal to start game (e.g. from RoomManager)
        }
        if(gameTimerCanvas!= null)
            gameTimerCanvas.SetActive(false);
    }

    public void TryStartGame(int currentPlayerCount, int requiredPlayers) {
        if (PhotonNetwork.IsMasterClient && !gameStarted && currentPlayerCount >= requiredPlayers) {
            Debug.Log("Starting game for all clients");
            photonView.RPC("StartGameRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void StartGameRPC() {
        Debug.Log("Game Started");
        gameStarted = true;
        gameEnded = false;
        remainingTime = gameDuration;

        if (gameTimerCanvas != null)
            gameTimerCanvas.SetActive(true);
    }

    void Update() {
        if (!gameStarted || gameEnded) return;

        if (PhotonNetwork.IsMasterClient) {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0f) remainingTime = 0f;

            // Sync with clients every second (to avoid flooding)
            syncTimer -= Time.deltaTime;
            if (syncTimer <= 0f) {
                photonView.RPC("UpdateTimeRPC", RpcTarget.Others, remainingTime);
                syncTimer = 1f; // sync every 1 second
            }

            if (remainingTime <= 0f && !gameEnded) {
                gameEnded = true;
                photonView.RPC("EndGameRPC", RpcTarget.All);
            }
        }

        UpdateTimerUI();
    }


    [PunRPC]
    void EndGameRPC() {
        Debug.Log("Game Over — showing leaderboard");

        gameStarted = false;
        gameEnded = true;

        gameTimerCanvas?.SetActive(false);
        endGameScreen?.SetActive(true);

        LeaderboardManager.Instance?.ShowLeaderboard();

    }


    public float GetRemainingTime() {
        return remainingTime;
    }

    [PunRPC]
    void UpdateTimeRPC(float syncedTime) {
        remainingTime = syncedTime;
    }

    void UpdateTimerUI() {
        if (timerText != null) {
            int min = Mathf.FloorToInt(remainingTime / 60f);
            int sec = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = $"{min:00}:{sec:00}";
        }
    }
}
