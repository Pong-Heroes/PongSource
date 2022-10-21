using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class PaddlePlayer : BasePaddle
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
    public Transform joint;
    private bool[] isKeyboard=new bool[2];

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

        Rigidbody2D r_body = gameObject.GetComponent<Rigidbody2D>();
        PerformBodyChanges(r_body);
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
            if (base.Animator)
            {
                base.Animator.Play("Attack");
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Managers.Input.isActive)
        {
            // if (Managers.Input.inputType == InputMethod.KeyboardInput)
            //     KeyboardInput();
            // else if (Managers.Input.inputType == InputMethod.TouchLRInput)
            //     TouchLRInput();
            // else
            DragInput();

            // KeyboardInput();
        }
    }

    void KeyboardInput()
    {
        // float direction = Mathf.Clamp((Input.GetAxis("Mouse Y") + Input.GetAxis("Vertical")), -1, 1);
        // CheckMovementBlock(direction);
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
                case GameMode.CLASSIC:
                    directionY = Mathf.Clamp((Input.GetAxisRaw("Vertical")), -1, 1);
                    break;
                case GameMode.TOURNAMENT:
                    if (Constants.tournamentMode == TournamentMode.CLASSIC)
                    {
                        directionY = Mathf.Clamp((Input.GetAxisRaw("Vertical")), -1, 1);
                    } else if (Constants.tournamentMode == TournamentMode.FREESTYLE)
                    {
                        directionY = Mathf.Clamp((Input.GetAxisRaw("Vertical")), -1, 1);
                        directionX = Mathf.Clamp((Input.GetAxisRaw("Horizontal")), -1, 1);
                    }
                    break;
                case GameMode.FREESTYLE:
                    directionY = Mathf.Clamp((Input.GetAxisRaw("Vertical")), -1, 1);
                    directionX = Mathf.Clamp((Input.GetAxisRaw("Horizontal")), -1, 1);
                    break;
            }

            //Debug.Log(directionX + " " + directionY);
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
                        case GameMode.TOURNAMENT:
                            if (Constants.tournamentMode == TournamentMode.CLASSIC)
                            {
                                curPosition.y = Mathf.Clamp(curPosition.y + deltaY, -YBoundsRef.position.y, YBoundsRef.position.y);
                            } else if (Constants.tournamentMode == TournamentMode.FREESTYLE)
                            {
                                curPosition.y = Mathf.Clamp(curPosition.y + deltaY, -YBoundsRef.position.y, YBoundsRef.position.y);
                                curPosition.x = Mathf.Clamp(storedXCursor + deltaY, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                            }
                            break;
                        case GameMode.FREESTYLE:
                            curPosition.y = Mathf.Clamp(curPosition.y + deltaY, -YBoundsRef.position.y, YBoundsRef.position.y);
                            curPosition.x = Mathf.Clamp(storedXCursor + deltaY, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                            break;
                    }
                    joint.position = curPosition;
                }
            }
            else
            {

                if (Constants.Mode == GameMode.FREESTYLE) //retrict mouse on freestyle mode
                    return;

                if(Constants.Mode == GameMode.TOURNAMENT)
                {
                    if (Constants.tournamentMode == TournamentMode.FREESTYLE)
                        return;
                }

                    var hasMouseMoved = Mathf.RoundToInt(Mathf.Abs(curPosition.y - lastMousePos.y)) > 0 || Mathf.RoundToInt(Mathf.Abs(curPosition.x - lastMousePos.x)) > 0;
                if (!hasMouseMoved)
                    return;

                lastMousePos = Vector3.zero;

                switch (Constants.Mode)
                {
                    case GameMode.CLASSIC:
                        curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                        break;
                    case GameMode.TOURNAMENT:
                        if (Constants.tournamentMode == TournamentMode.CLASSIC)
                        {
                            curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                        }
                        else if (Constants.tournamentMode == TournamentMode.FREESTYLE)
                        {
                            curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                            curPosition.x = Mathf.Clamp(storedXCursor, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                        }
                        break;
                    case GameMode.FREESTYLE:
                        curPosition.y = Mathf.Clamp(curPosition.y, -YBoundsRef.position.y, YBoundsRef.position.y);
                        curPosition.x = Mathf.Clamp(storedXCursor, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                        break;
                }

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
            case GameMode.TOURNAMENT:
                if (Constants.tournamentMode == TournamentMode.CLASSIC)
                {
                    diff = YBoundsRef.position.y;
                    pos.y = Mathf.Clamp(pos.y, -diff, diff);
                }
                else if (Constants.tournamentMode == TournamentMode.FREESTYLE)
                {
                    diff = YBoundsRef.position.y;
                    pos.y = Mathf.Clamp(pos.y, -diff, diff);
                    pos.x = Mathf.Clamp(pos.x, XBoundsRef[1].position.x, XBoundsRef[0].position.x);
                }
                break;
            case GameMode.FREESTYLE:
                diff = YBoundsRef.position.y;
                pos.y = Mathf.Clamp(pos.y, -diff, diff);
                pos.x= Mathf.Clamp(pos.x, XBoundsRef[1].position.x, XBoundsRef[0].position.x); 
                break;
        }

        joint.position = pos;
    }
}
