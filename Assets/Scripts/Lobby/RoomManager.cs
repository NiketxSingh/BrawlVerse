using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    // Start is called before the first frame update
    [Header("Player Object")]
    public GameObject player;

    [Header("Player Spawn Point")]
    public Transform spanPoint;

    [Header("Free Look Camera")]
    public CinemachineFreeLook freeLook;

    [Header("Camera UI")]
    public GameObject roomCam;

    [Header("UI")]
    public GameObject nickNameUI;
    public GameObject connectingUI;

    [Header("Room Name")]
    public string roomName = "test";

    [Header("Timer")]
    public GameObject gameTimerObject;
    public GameObject gameTimerCanvas;

    string nickName = "unnamed";

    private GameTimer gameTimer;
    private void Awake()
    {
        Instance = this;
        gameTimer = FindObjectOfType<GameTimer>();
    }
    public void SetNickname(string _name)
    {
        nickName = _name;
    }
    public void OnJoinButtonPressed()
    {
        Debug.Log(message: "Connecting. . . ");
        Debug.Log(roomName);
        PhotonNetwork.JoinOrCreateRoom(roomName, new Photon.Realtime.RoomOptions(), null);

        nickNameUI.SetActive(false);
        connectingUI.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        roomCam.SetActive(false);
        SpawnPlayer();

        //if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1) {
        //    StartGame();
        //}
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        //Debug.Log("Am I MasterClient? " + PhotonNetwork.IsMasterClient);
        Debug.Log("Player Joined: " + newPlayer.NickName);
        Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        CheckStartCondition();
    }
    void CheckStartCondition() {
        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            if (GameTimer.Instance != null) {
                GameTimer.Instance.TryStartGame(PhotonNetwork.CurrentRoom.PlayerCount, 2);
            }
            else {
                Debug.LogWarning("GameTimer.Instance is null. Retrying in 1 second...");
                StartCoroutine(WaitAndRetryStart());
            }
        }
    }
    private IEnumerator WaitAndRetryStart() {
        yield return new WaitForSeconds(1f); // wait a second for GameTimer to initialize

        if (GameTimer.Instance != null) {
            GameTimer.Instance.TryStartGame(PhotonNetwork.CurrentRoom.PlayerCount, 2);
        }
        else {
            Debug.LogError("GameTimer still not found after delay!");
        }
    }
    public void SpawnPlayer()
    {
        if (gameTimer.gameEnded == true) return;

        GameObject _player = PhotonNetwork.Instantiate(player.name, spanPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerHealth>().isLocalPlayer = true;
        PhotonView view = _player.GetComponent<PhotonView>();
        view.RPC("SetPlayerName", RpcTarget.AllBuffered, nickName);
        if (view != null && view.IsMine && freeLook != null)
        {
            Transform lookAt = _player.transform.GetChild(1);
            freeLook.Follow = lookAt;
            freeLook.LookAt = lookAt;
        }
    }

    //Respawning after death
    public void StartRespawnCountdown() {
        StartCoroutine(RespawnAfterDelay(5f));
    }

    private IEnumerator RespawnAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        SpawnPlayer();
    }
}
