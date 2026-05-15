using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Ball;

    public static event Action<int> OnBlueScoreChange;
    public static event Action<int> OnRedScoreChange;

    public BoxCollider RedTeamBoundSpawn;
    public BoxCollider BlueTeamBoundSpawn;
    [SerializeField] private BoxCollider BallBoundSpawn;

    [Header("Goals")]
    public Transform RedGoal;
    public Transform BlueGoal;

    private List<SoccerAgent> RedTeam  = new List<SoccerAgent>();
    private List<SoccerAgent> BlueTeam = new List<SoccerAgent>();

    private int scoreRed  = 0;
    private int scoreBlue = 0;

    void OnEnable()  => CheckGoal.OnGoalScored += UpdateScore;
    void OnDisable() => CheckGoal.OnGoalScored -= UpdateScore;

    void UpdateScore(TeamColor scoringTeam)
    {
        if (scoringTeam == TeamColor.Red)
        {
            scoreRed++;
            OnRedScoreChange?.Invoke(scoreRed);
        }
        else
        {
            scoreBlue++;
            OnBlueScoreChange?.Invoke(scoreBlue);
        }

        ResetBall();
        Debug.Log($"Score: Red {scoreRed} - Blue {scoreBlue}");
    }

    public void ResetBall()
    {
        Rigidbody rb = Ball.GetComponent<Rigidbody>();
        
        // Resetare poziție
        Ball.transform.position = GetRandomPosition(BallBoundSpawn);
        
        // Resetare completă a stării fizice
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep(); // Forțează motorul fizic să ignore obiectul pentru un cadru
        
        // Forțează Unity să actualizeze transformarea imediat
        Physics.SyncTransforms();
    }

    public Vector3 GetRandomPosition(BoxCollider box)
    {
        Bounds bounds = box.bounds;
        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.center.y;
        float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }

    public void RegisterAgent(SoccerAgent agent)
    {
        if (agent.team == TeamColor.Red)
        {
            agent.targetGoal = BlueGoal;
            agent.ownGoal    = RedGoal;
            RedTeam.Add(agent);
        }
        else
        {
            agent.targetGoal = RedGoal;
            agent.ownGoal    = BlueGoal;
            BlueTeam.Add(agent);
        }
    }

}
