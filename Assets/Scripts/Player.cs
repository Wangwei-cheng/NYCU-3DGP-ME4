using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public Rigidbody playerRigid;
    public float walkSpeed;
    public Transform playerTransform;
    public Animator playerAnimator;

    public Camera mainCamera;

    public BallsMgr ballsMgr;

    private Dictionary<string, int> states;
    private Dictionary<char, float> directions;

    public List<Path> actions;
    private int actionIndex;

    private string curState;
    private float curDirection;
    private Path curAction;

    private AStar aStar;

    private static Player instance;
    public static Player Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
        aStar = new AStar();
    }

    private void Start()
    {
        directions = new Dictionary<char, float>();
        directions.Add('N', 0);
        directions.Add('E', 90);
        directions.Add('S', 180);
        directions.Add('W', 270);

        states = new Dictionary<string, int>();
        states.Add("Idle", 0);
        states.Add("Walking", 1);
        states.Add("Rotating", 2);

        actionIndex = 0;
        curDirection = playerTransform.eulerAngles.y;
        curState = "Idle";
        curAction = actions[actionIndex];
    }

    private void FixedUpdate()
    {
        float step = walkSpeed * Time.deltaTime;
        if (states[curState] == states["Walking"])
        {
            playerTransform.localPosition = Vector3.MoveTowards(playerTransform.localPosition, new Vector3(curAction.goal.x, 2, curAction.goal.y), step);
        }
    }

    private void Update()
    {
        curDirection = playerTransform.eulerAngles.y;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                int x = (int)((hit.point.x + 2.5) / 5) * 5;
                int y = (int)((hit.point.z + 2.5) / 5) * 5;
                NewActions(new Vector2Int(x, y));
            }
        }

        if (states[curState] != states["Idle"])
        {
            if (playerTransform.position == new Vector3(actions[actionIndex].goal.x, 2, actions[actionIndex].goal.y))
            {
                actionIndex++;
                if (actionIndex == actions.Count)
                {
                    curState = "Idle";
                    playerAnimator.ResetTrigger("Walk");
                    playerAnimator.SetTrigger("Idle");
                }
                else if (actionIndex < actions.Count)
                {
                    curAction = actions[actionIndex];
                    if (curDirection != directions[curAction.direction])
                    {
                        StartCoroutine(Rotate(directions[curAction.direction]));
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartMoving();
        }
    }

    private void NewActions(Vector2Int goal)
    {
        actions = aStar.FindPath(curAction.goal, goal, curAction.direction);
        actionIndex = 0;
        ballsMgr.DeleteAllBalls();
        foreach(Path action in actions)
        {
            ballsMgr.CreateBall(action.goal);
        }
        StartMoving();
    }

    private void StartMoving()
    {
        if (states[curState] == states["Idle"])
        {
            curState = "Walking";
            playerAnimator.ResetTrigger("Idle");
            playerAnimator.SetTrigger("Walk");
        }
    }

    IEnumerator Rotate(float goalAngle)
    {
        curState = "Rotating";
        float startRotation = curDirection;
        float endRotation = goalAngle;
        float duration = 0.5f;
        float t = 0f;

        if (endRotation - startRotation > 180)  endRotation -= 360;
        if (endRotation - startRotation < -180) endRotation += 360;

        while(t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration);
            playerTransform.eulerAngles = new Vector3(playerTransform.eulerAngles.x, yRotation, playerTransform.eulerAngles.z);

            yield return null;
        }
        
        curState = "Walking";
    }
}
