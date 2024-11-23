using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsMgr : MonoBehaviour
{
    public GameObject ballObject;
    private float ballHeight;

    private void Start()
    {
        ballHeight = 2.5f;
    }

    public void CreateBall(Vector2Int position)
    {
        Instantiate(ballObject, new Vector3(position.x, ballHeight, position.y), Quaternion.identity);
    }

    public void DeleteAllBalls()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach(GameObject ball in balls)
        {
            Destroy(ball);
        }
    }
}
