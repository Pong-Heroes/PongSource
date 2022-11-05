using Photon.Pun;
using UnityEngine;
using System.Collections;
using Smooth;

public class PhotonBall : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public float speed;
    public float velocity;
    public float speedMultiplier;
    public ParticleSystem particle;
    public ParticleSystem hitParticle;
    public Rigidbody2D ballBody;
    [HideInInspector]
    public BasePaddle lastTouchedPaddle;
    private PhotonView lastPhotonView;
    private int GetMaxScores => (int)PhotonNetwork.CurrentRoom.CustomProperties[Constants.MAXSCORES_KEY];
    SmoothSyncPUN2 smoothSync;

    private int turn = 0;
    private Vector2 GetKickDirection
    {
        get
        {
            float r = Random.value;
            Vector2 _direction = new Vector2(turn, r);//(r >= 0.5f) ? new Vector2(1, r) : new Vector2(-1, r);
            return _direction;
        }
    }

    void Awake()
    {

    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(ResetBall), RpcTarget.All);
            photonView.RPC(nameof(_KickOffBall), RpcTarget.All, GetKickDirection);
        }
        // else
        // {
        //     ballBody.isKinematic = true;
        //     particle.gameObject.SetActive(true);
        // }

        smoothSync = GetComponent<SmoothSyncPUN2>();
        if (smoothSync)
        {
            // Set up a validation method to check incoming States to see if cheating may be happening. 
            smoothSync.validateStateMethod = validateStateOfPlayer;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (PhotonNetwork.IsMasterClient)
        {
            Events.OnBallSpeedChanged += OnBallSpeedChanged;
            Events.OnImpactMultiplierChanged += OnImpactMultiplierChanged;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (PhotonNetwork.IsMasterClient)
        {
            Events.OnBallSpeedChanged -= OnBallSpeedChanged;
            Events.OnImpactMultiplierChanged -= OnImpactMultiplierChanged;
        }
    }

    private void OnBallSpeedChanged(float value)
    {
        speed = value;
        ballBody.velocity = Vector3.ClampMagnitude(ballBody.velocity, speed);
    }
    private void OnImpactMultiplierChanged(float value) => speedMultiplier = value;

    void OnCollisionEnter2D(Collision2D other)
    {
        hitParticle.Play();
        // Managers.Audio.PlayCollisionSound();
        // StartCoroutine(Managers.Cam.shaker.Shake());
        if (other.gameObject.name.Equals("RightWall"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Player A scores

                photonView.RPC(nameof(ResetBall), RpcTarget.All);
                photonView.RPC(nameof(UpdatePlayerScoresA), RpcTarget.All, parameters: 1);

                if (PhotonScoresManager.GetPlayerScoresA >= GetMaxScores)
                {
                    Debug.Log("Player A wins.");
                    photonView.RPC(nameof(ConcludeGame), RpcTarget.All, PhotonGameManager.Instance.Players[0].OwnerActorNr);
                }
                else
                {
                    photonView.RPC(nameof(_KickOffBall), RpcTarget.All, GetKickDirection);
                }
            }
            else
            {
                // if (PhotonPlayerManager.LocalPlayerInstance.IsMine)
                //     ResetBall();
            }
        }
        else if (other.gameObject.name.Equals("LeftWall"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Player B scores

                photonView.RPC(nameof(ResetBall), RpcTarget.All);
                photonView.RPC(nameof(UpdatePlayerScoresB), RpcTarget.All, parameters: 1);

                if (PhotonScoresManager.GetPlayerScoresB >= GetMaxScores)
                {
                    Debug.Log("Player B wins.");
                    photonView.RPC(nameof(ConcludeGame), RpcTarget.All, PhotonGameManager.Instance.Players[1].OwnerActorNr);
                }
                else
                {
                    photonView.RPC(nameof(_KickOffBall), RpcTarget.All, GetKickDirection);
                }
            }
            else
            {
                // if (PhotonPlayerManager.LocalPlayerInstance.IsMine)
                //     ResetBall();
            }
        }
        else if (other.gameObject.CompareTag("PADDLE"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector2 curVelocity = ballBody.velocity;

                float x = HitFactor(transform.position, other.transform.position, other.collider.bounds.size.y);
                int temp = 0;
                temp = (other.transform.position.x > 1) ? -1 : 1;
                Vector2 dir = new Vector2(temp, x).normalized;
                Vector2 targetVel = dir * curVelocity.magnitude * speedMultiplier;

                PlayAttack(PhotonPlayerManager.LocalPlayerInstance.OwnerActorNr);
                AssignVelocity(targetVel, curVelocity);

                // ballBody.velocity = Vector2.zero;
                // photonView.RPC(nameof(PlayAttack), RpcTarget.All, photonView.OwnerActorNr);
                // PhotonNetwork.RaiseEvent(0, photonView.OwnerActorNr, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
                photonView.RPC(nameof(AssignVelocity), RpcTarget.Others, targetVel, curVelocity);

                lastTouchedPaddle = other.gameObject.GetComponent<BasePaddle>();
                lastPhotonView = other.gameObject.GetComponent<PhotonView>();
            }
            else
            {
                PlayAttack(other.gameObject.GetComponent<PhotonView>().OwnerActorNr);

                // if (photonView.IsMine)
                // {
                //     ballBody.velocity = Vector2.zero;
                // }
            }
        }
    }

    [PunRPC]
    private void PlayAttack(int ownerActorNumber)
    {
        var player = PhotonGameManager.Instance.Players.Find(x => x.OwnerActorNr == ownerActorNumber);
        if (player != null)
        {
            Debug.LogError("attack animation played!");
            player.GetComponent<PhotonPaddlePlayer>().PlayAttack();
        }
    }

    [PunRPC]
    private void AssignVelocity(Vector2 targetVelocity, Vector2 curVelocity)
    {
        ballBody.velocity = targetVelocity;
        this.velocity = curVelocity.magnitude * speedMultiplier;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // if (lastTouchedPaddle != null)
        //     other.gameObject.GetComponent<Powerup>().TriggerPowerup(lastTouchedPaddle);
    }

    [PunRPC]
    IEnumerator _KickOffBall(Vector2 _direction)
    {
        yield return new WaitForSeconds(3);
        KickOffBall(_direction);
    }

    public void KickOffBall(Vector2 _direction)
    {
        ballBody.angularVelocity = 0.0f;
        ballBody.AddForce(_direction * speed);
        particle.gameObject.SetActive(true);
    }

    [PunRPC]
    public void ResetBall()
    {
        turn = turn == 1 ? -1 : 1;
        ballBody.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        particle.gameObject.SetActive(false);
    }

    float HitFactor(Vector2 ballPosition, Vector2 paddlePosition, float paddleWidth)
    {
        return (ballPosition.y - paddlePosition.y) / paddleWidth;
    }

    public void ParticleRotation()
    {
        Vector3 directionOfMotion = new Vector3(0, ballBody.velocity.y, ballBody.velocity.x);
        Quaternion rotation = Quaternion.LookRotation(directionOfMotion);
        particle.transform.localRotation = rotation;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        this.transform.parent = PhotonGameManager.Instance.BallSpawnPoint;
        this.transform.localScale = Vector3.one;
        this.transform.localPosition = Vector3.zero;
    }

    [PunRPC]
    private void UpdatePlayerScoresA(int increment) => PhotonScoresManager.UpdatePlayerScoresA(increment);
    [PunRPC]
    private void UpdatePlayerScoresB(int increment) => PhotonScoresManager.UpdatePlayerScoresB(increment);
    [PunRPC]
    private void ConcludeGame(int winner)
    {
        ResetBall();
        PhotonGameManager.Instance.ConcludeGame(winner);
    }

    /// <summary>
    /// Custom validation method. 
    /// <remarks>
    /// Allows you to check variables to see if they are within allowed values. For example, position.
    /// This is for the server to check client owned objects to look for cheating like your players 
    /// modifying values beyond the game's intended limits. 
    /// </remarks>
    /// </summary>
    public static bool validateStateOfPlayer(StatePUN2 latestReceivedState, StatePUN2 latestValidatedState)
    {
        // Here I do a simple distance check using State.receivedOnServerTimestamp. This variable is updated
        // by Smooth Sync whenever a State is validated. If the object has gone more than 9000 units 
        // in less than a half of a second then I ignore the message. You might want to kick 
        // players here, add them to a ban list, or collect your own data to see if it keeps 
        // happening. 
        if (Vector3.Distance(latestReceivedState.position, latestValidatedState.position) > 9000.0f &&
            (latestReceivedState.ownerTimestamp - latestValidatedState.receivedOnServerTimestamp < .5f))
        {
            // Return false and refuse to accept the State. The State will not be added locally
            // on the server or sent out to other clients.
            return false;
        }
        else
        {
            // Return true to accept the State. The State will be added locally on the server and sent out 
            // to other clients.
            return true;
        }
    }
}
