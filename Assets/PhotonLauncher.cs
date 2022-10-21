using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using ExitGames.Client.Photon;
public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    private static PhotonLauncher instance;
    #region Private Serializable Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    private Text feedbackText;

    [Tooltip("The maximum number of players per room")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    [Tooltip("The UI Loader Anime")]
    [SerializeField]
    private LoaderAnime loaderAnime;

    [SerializeField] private CharacterInfo[] CharactersLobby;

    #endregion

    #region Private Fields
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";

    public static Hashtable GetCustomProperties()
    {
        var customProperties = new Hashtable
        {
            { Constants.MAXSCORES_KEY, Constants.MaxScore },
            { Constants.MODE_KEY, Constants.ModeIndex },
            { Constants.LEVEL_KEY, Constants.LevelIndex  },
            { Constants.CONTROLLER_KEY, Constants.ControllerIndex  },
            { Constants.BET_KEY, Constants.BetAmount  },
        };

        Debug.Log(customProperties.ToStringFull());
        return customProperties;
    }

    public static string[] GetCustomLobbyProperties()
    {
        string[] keys = new string[5]
        {
            Constants.MAXSCORES_KEY,
            Constants.MODE_KEY,
            Constants.LEVEL_KEY,
            Constants.CONTROLLER_KEY,
            Constants.BET_KEY
        };
        return keys;
    }

    public static void ConnectMaster() => instance.Connect();
    public static void DisconnectMaster() => instance.DisconnectPhoton();

    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        if (loaderAnime == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.", this);
        }

        instance = this;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
        feedbackText.text = "";

        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = true;

        // hide the Play button for visual consistency
        controlPanel.SetActive(false);

        // start the loader animation for visual effect.
        if (loaderAnime != null)
        {
            loaderAnime.StartLoaderAnimation();
        }

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            LogFeedback("Joining Room...");
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {

            LogFeedback("Connecting...");

            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.gameVersion;
        }
    }

    public void Return()
    {
        AudioManager.Audio.PlayClickSound();
        AudioManager.Audio.PlayLobbyMusic();
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
        }
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }

    /// <summary>
    /// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
    /// </summary>
    /// <param name="message">Message.</param>
    void LogFeedback(string message)
    {
        // we do not assume there is a feedbackText defined.
        if (feedbackText == null)
        {
            return;
        }

        // add new messages as a new line and at the bottom of the log.
        //feedbackText.text += System.Environment.NewLine + message;
        feedbackText.text = message;
    }

    #endregion

    #region MonoBehaviourPunCallbacks CallBacks
    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks
    public void CreateRoom()
    {
        var roomCode = Random.Range(10000, 99999);
        string roomName = roomCode.ToString();

        var customProperties = GetCustomProperties();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        roomOptions.CustomRoomPropertiesForLobby = GetCustomLobbyProperties();
        roomOptions.CustomRoomProperties = customProperties;

        LogFeedback("Creating room...");
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    private void JoinRoomRandom(byte expectedMaxPlayers = 2)
    {
        LogFeedback("finding room...");
        Hashtable expectedCustomRoomProperties = GetCustomProperties();
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        LogFeedback("ERROR: " + message);
        Invoke(nameof(CreateRoom), 1.5f);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        LogFeedback("find room failed!");
        Constants.PrintLog("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 6}, null);");
        Invoke(nameof(CreateRoom), 1.5f);
    }

    public void DisconnectPhoton()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        for (int i = 0; i < CharactersLobby.Length; i++)
        {
            CharactersLobby[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called after the connection to the master is established and authenticated
    /// </summary>
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            Constants.GameStarted = false;
            LogFeedback("Connected with master");
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");


            if (PhotonNetwork.InLobby)
                LobbyConnection();
            else
                PhotonNetwork.JoinLobby();
        }
    }

    public void LobbyConnection()
    {
        JoinRoomRandom(maxPlayersPerRoom);
    }

    public override void OnJoinedLobby()
    {
        LobbyConnection();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log("new player entered: " + newPlayer.NickName);
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // Add a button for each player in the room.
            // You can use p.NickName to access the player's nick name.
            CharactersLobby[p.ActorNumber - 1].gameObject.SetActive(true);
            CharactersLobby[p.ActorNumber - 1].chaarcterName.text = p.NickName;
            CharactersLobby[p.ActorNumber - 1].winRate.text = "0";//((int)p.CustomProperties["winrate"]).ToString();
            CharactersLobby[p.ActorNumber - 1].characterImage.runtimeAnimatorController = Databases.CharactersDatabase.GetCharacterOfIndex((int)p.CustomProperties["character"]).AnimatorController;
        }
        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersPerRoom)
        {
            feedbackText.text = " Starting game...";
            //feedbackText.text += System.Environment.NewLine + " Starting game...";
            // #Critical
            // Load the Room Level. 
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                
                Invoke(nameof(LoadArena), 3f);
            }
        }
        else
        {
            feedbackText.text = " Waiting for 1 more player to join.";
            //feedbackText.text += System.Environment.NewLine + " Waiting for 1 more player to join.";
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!Constants.GameStarted)
        {
            CancelInvoke(nameof(LoadArena));
            Events.DoReportMessage(new messageInfo("other player has left."));
            LauncherSceneUIManager.Instance.GoBackButton_MultiplayerConnection();
        }

    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        LogFeedback("Disconnected");
        Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

        // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
        loaderAnime.StopLoaderAnimation();

        isConnecting = false;
        feedbackText.text = "";
        controlPanel.SetActive(true);
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
    /// enough players are in the room to start playing.
    /// </remarks>
    public override void OnJoinedRoom()
    {
        LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // Add a button for each player in the room.
            // You can use p.NickName to access the player's nick name.
            CharactersLobby[p.ActorNumber - 1].gameObject.SetActive(true);
            CharactersLobby[p.ActorNumber - 1].chaarcterName.text = p.NickName;
            CharactersLobby[p.ActorNumber - 1].winRate.text = "0";//((int)p.CustomProperties["winrate"]).ToString();
            CharactersLobby[p.ActorNumber - 1].characterImage.runtimeAnimatorController = Databases.CharactersDatabase.GetCharacterOfIndex((int)p.CustomProperties["character"]).AnimatorController;
        }

        int mode = ((int)PhotonNetwork.CurrentRoom.CustomProperties[Constants.CONTROLLER_KEY]);
        Constants.Mode = ((GameMode)mode);
        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount < maxPlayersPerRoom)
        {
            feedbackText.text = " Waiting for 1 more player to join.";
            //feedbackText.text += System.Environment.NewLine + " Waiting for 1 more player to join.";
        }
        else
        {
            feedbackText.text = " Starting Game...";
            //feedbackText.text += System.Environment.NewLine + " Starting Game...";
        }
    }

    private void LoadArena()
    {
        PhotonNetwork.LoadLevel("PhotonGamePlay");
    }

    #endregion
}
