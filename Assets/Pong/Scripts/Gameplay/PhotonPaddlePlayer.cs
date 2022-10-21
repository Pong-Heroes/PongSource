using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Photon.Pun;
using Smooth;

public class PhotonPaddlePlayer : BasePaddle, IPunObservable, IPunInstantiateMagicCallback
{
    public Vector2 DefaultPosition;
    public RectTransform YBoundsRef;
    public Transform[] XBoundsRef;
    public float diff;

    [DllImport("__Internal")]
    private static extern bool IsMobile();
    private CharactersDatabase Database => Databases.CharactersDatabase;

    private float deltaY;
    private float deltaX;
    private float directionX = 0;
    private float directionY = 0;
    private Vector3 pos = Vector3.zero;
    private float storedXCursor;
    private Vector3 lastMousePos;
    private bool[] isKeyboard = new bool[2];

    [SerializeField] private Transform charContainer, textContainer;
    [SerializeField] private Text charNameText;
    public Transform joint;
    SmoothSyncPUN2 smoothSync;

    public bool isMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
         return IsMobile();
#endif
        return false;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        this.YBoundsRef = PhotonGameManager.Instance.YBounds;

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

        if (base.Animator)
        {
            base.Animator.runtimeAnimatorController = Database.GetSelectedCharacter.AnimatorController;
            base.Animator.GetComponent<RectTransform>().sizeDelta = Database.GetSelectedCharacter.GetImageSize;
        }
        else
            Debug.LogError("Animator not assigned.");

        //Rigidbody2D r_body = gameObject.GetComponent<Rigidbody2D>();
        //PerformBodyChanges(r_body);
    }

    public void PerformBodyChanges(Rigidbody2D r_body)
    {
        if (Constants.Mode == GameMode.CLASSIC)
        {
            r_body.mass = 0.9f;
            r_body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else if (Constants.Mode == GameMode.FREESTYLE)
        {
            r_body.mass = 0;
            r_body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else if (Constants.Mode == GameMode.TOURNAMENT)
        {
            if (Constants.tournamentMode == TournamentMode.CLASSIC)
            {
                r_body.mass = 0.9f;
                r_body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            else if (Constants.tournamentMode == TournamentMode.FREESTYLE)
            {
                r_body.mass = 0;
                r_body.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ball")
        {

        }
    }

    public void PlayAttack()
    {
        if (base.Animator)
        {
            base.Animator.Play("Attack");
        }
    }

    [PunRPC]
    private void OnAttack()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (photonView.IsMine)
        {
            DragInput();
        }
    }

    void TouchLRInput()
    {
        float direction = 0;

        if (ControlFreak2.CF2Input.GetMouseButton(0))
            direction = (ControlFreak2.CF2Input.mousePosition.y > Screen.width / 2) ? 1 : -1;

        //CheckMovementBlock(direction);
    }

    void DragInput()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        storedXCursor = curPosition.x;
        curPosition.x = joint.position.x;
        curPosition.z = joint.position.z;

        isKeyboard[0] = Mathf.RoundToInt(Input.GetAxisRaw("Vertical")) != 0;
        isKeyboard[1] = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != 0;
        if (isKeyboard[0] || isKeyboard[1])
        {
            if (!lastMousePos.Equals(curPosition))
                lastMousePos = curPosition;

            directionX = 0;
            directionY = 0;

            switch (Constants.Mode)
            {
                case GameMode.FREESTYLE:
                    // directionY = Mathf.Clamp((Input.GetAxisRaw("Vertical")), -1, 1);
                    directionX = Mathf.Clamp((Input.GetAxisRaw("Horizontal")), -1, 1);
                    break;
            }

            Debug.Log(directionX + " " + directionY);
            directionY = Mathf.Clamp((Input.GetAxisRaw("Vertical")), -1, 1);
            CheckMovementBlock(directionX, directionY);
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    deltaY = (joint.position.y - curPosition.y);
                    deltaX = (joint.position.x - storedXCursor);
                }

                if (Input.GetMouseButton(0))
                {
                    switch (Constants.Mode)
                    {
                        case GameMode.CLASSIC:
                            curPosition.y = Mathf.Clamp(curPosition.y + deltaY, -YBoundsRef.position.y, YBoundsRef.position.y);
                            break;
                        case GameMode.FREESTYLE:
                            curPosition.y = Mathf.Clamp(curPosition.y + deltaY, -YBoundsRef.position.y, YBoundsRef.position.y);
                            curPosition.x = Mathf.Clamp(storedXCursor + deltaY, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                            break;
                    }

                    curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                    joint.position = curPosition;
                }
            }
            else
            {
                if (Constants.Mode == GameMode.FREESTYLE) //retrict mouse on freestyle mode
                    return;

                var hasMouseMoved = Mathf.RoundToInt(Mathf.Abs(curPosition.y - lastMousePos.y)) > 0 || Mathf.RoundToInt(Mathf.Abs(curPosition.x - lastMousePos.x)) > 0;
                if (!hasMouseMoved)
                    return;

                lastMousePos = Vector3.zero;

                switch (Constants.Mode)
                {
                    case GameMode.CLASSIC:
                        curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                        break;
                    case GameMode.FREESTYLE:
                        curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                        curPosition.x = Mathf.Clamp(storedXCursor, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                        break;
                }

                curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                joint.position = curPosition;
            }
        }
    }

    void CheckMovementBlock(float dirX, float dirY)
    {
        joint.Translate(new Vector2(dirX, dirY) * speed * Time.deltaTime);
        pos = joint.position;

        switch (Constants.Mode)
        {
            case GameMode.CLASSIC:
                diff = YBoundsRef.position.y;
                pos.y = Mathf.Clamp(pos.y, -diff, diff);
                break;
            case GameMode.FREESTYLE:
                diff = YBoundsRef.position.y;
                pos.y = Mathf.Clamp(pos.y, -diff, diff);
                pos.x = Mathf.Clamp(pos.x, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                break;
        }

        joint.position = pos;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // throw new System.NotImplementedException();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("ID: " + info.photonView.CreatorActorNr);
        if (info.photonView.CreatorActorNr == 1)
        {

            this.joint = PhotonGameManager.Instance.PlayerJointA.transform;
            if (photonView.IsMine)
            {
                if (info.photonView.Owner.NickName == "")
                {
                    info.photonView.Owner.NickName = "Guest-A [YOU]";
                }

                XBoundsRef = PhotonGameManager.Instance.XboundsPlayerA;
                PhotonGameManager.Instance.PlayerJointA.connectedBody = this._rigidBody;
            }
            else
            {
                this._rigidBody.isKinematic = true;
                if (info.photonView.Owner.NickName == "")
                {
                    info.photonView.Owner.NickName = "Guest-A";
                }
            }

            PhotonGameManager.Instance.Players.Add(info.photonView);
            info.photonView.transform.SetParent(PhotonGameManager.Instance.PlayerSpawnPointA);
            info.photonView.transform.SetPositionAndRotation(PhotonGameManager.Instance.PlayerSpawnPointA.position, PhotonGameManager.Instance.PlayerSpawnPointA.rotation);
            info.photonView.transform.localScale = Vector3.one;
        }
        else
        {
            this.joint = PhotonGameManager.Instance.PlayerJointB.transform;
            if (photonView.IsMine)
            {
                if (info.photonView.Owner.NickName == "")
                {
                    info.photonView.Owner.NickName = "Guest-B [YOU]";
                }

                XBoundsRef = PhotonGameManager.Instance.XboundsPlayerB;
                PhotonGameManager.Instance.PlayerJointB.connectedBody = this._rigidBody;
            }
            else
            {
                this._rigidBody.isKinematic = true;

                if (info.photonView.Owner.NickName == "")
                {
                    info.photonView.Owner.NickName = "Guest-B";
                }
            }

            PhotonGameManager.Instance.Players.Add(info.photonView);
            info.photonView.transform.SetParent(PhotonGameManager.Instance.PlayerSpawnPointB);
            info.photonView.transform.SetPositionAndRotation(PhotonGameManager.Instance.PlayerSpawnPointB.position, PhotonGameManager.Instance.PlayerSpawnPointB.rotation);
            info.photonView.transform.localScale = Vector3.one;

            this.charContainer.localScale = new Vector3(-1, 1, 1);
            this.textContainer.localScale = new Vector3(-1, 1, 1);
        }


        if (Constants.Mode == GameMode.CLASSIC)
            _rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        this.charNameText.text = info.photonView.Owner.NickName;

        if (base.Animator)
        {
            base.Animator.runtimeAnimatorController = Database.GetCharacterOfIndex((int)info.photonView.Controller.CustomProperties["character"]).AnimatorController;
            base.Animator.GetComponent<RectTransform>().sizeDelta = Database.GetCharacterOfIndex((int)info.photonView.Controller.CustomProperties["character"]).GetImageSize;
        }
        else
            Debug.LogError("Animator not assigned.");
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
