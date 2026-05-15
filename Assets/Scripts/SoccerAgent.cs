using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SoccerAgent : Agent
{
    public Transform ball;
    [HideInInspector] public Transform targetGoal;
    [HideInInspector] public Transform ownGoal;
    public float moveSpeed         = 2000f;
    public float dashForce         = 2000f;
    public int   dashCooldownSteps = 10;
    public TeamColor team;

    private Rigidbody agentRb;
    private bool  hasHitBall;
    private int   dashCooldownRemaining;
    private float prevBallDist;

    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
        GetComponentInChildren<Renderer>().material.color =
            team == TeamColor.Red ? Color.red : Color.blue;
        FindAnyObjectByType<GameManager>().RegisterAgent(this);
    }

    public override void OnEpisodeBegin()
    {
        GameManager gm = FindAnyObjectByType<GameManager>();

        BoxCollider zone = team == TeamColor.Red
            ? gm.RedTeamBoundSpawn
            : gm.BlueTeamBoundSpawn;

        transform.position      = gm.GetRandomPosition(zone);
        transform.rotation      = Quaternion.identity;
        agentRb.linearVelocity  = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        hasHitBall              = false;
        dashCooldownRemaining   = 0;
        gm.ResetBall();

        prevBallDist = Vector3.Distance(transform.position, ball.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toBall = ball.position - transform.position;
        sensor.AddObservation(toBall.normalized);
        
        sensor.AddObservation(toBall.magnitude / 20f); 

        sensor.AddObservation(agentRb.linearVelocity / 20f);
        
        sensor.AddObservation(ball.GetComponent<Rigidbody>().linearVelocity / 20f);
        
        sensor.AddObservation((targetGoal.position - transform.position).normalized);

        sensor.AddObservation(dashCooldownRemaining > 0 ? 1f : 0f);
        
    }

    private Vector3 movementActions;
    private bool    dashRequested;

    public override void OnActionReceived(ActionBuffers actions)
    {
        movementActions = new Vector3(
            actions.ContinuousActions[0], 0f,
            actions.ContinuousActions[1]);
        
        dashRequested = actions.DiscreteActions[0] == 1;
    }

    void FixedUpdate()
    {
        agentRb.AddForce(movementActions * moveSpeed);

        if (dashCooldownRemaining > 0)
            dashCooldownRemaining--;

        if (dashRequested && dashCooldownRemaining == 0)
        {
            Vector3 dashDir = movementActions.normalized;
            if (dashDir.magnitude > 0.1f)
            {
                agentRb.AddForce(dashDir * dashForce, ForceMode.Impulse);
                dashCooldownRemaining = dashCooldownSteps;
                AddReward(-0.05f);
            }
            dashRequested = false;
        }

        float currBallDist = Vector3.Distance(transform.position, ball.position);
        float delta = prevBallDist - currBallDist;
        
        if (delta > 0f) { AddReward(delta * 0.2f); } 
        else { AddReward(delta * 0.1f); } 
        
        if (currBallDist < 2.0f)
        {
            AddReward(0.01f * (2.0f - currBallDist));
        }

        prevBallDist = currBallDist;

        AddReward(-0.001f);
        if (agentRb.linearVelocity.magnitude < 0.1f)
            AddReward(-0.01f * Time.fixedDeltaTime);

        RequestDecision();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;
        c[0] =  Input.GetAxis("Vertical");
        c[1] = -Input.GetAxis("Horizontal");
        var d = actionsOut.DiscreteActions;
        d[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public void OnBallHit()
    {
        if (!hasHitBall)
        {
            AddReward(5f); 
            hasHitBall = true;
            Academy.Instance.StatsRecorder.Add("Custom/BallHit", 1);
        }

        Rigidbody ballRb  = ball.GetComponent<Rigidbody>();
        Vector3 ballVel   = ballRb.linearVelocity.normalized;
        Vector3 toGoal    = (targetGoal.position - ball.position).normalized;
        Vector3 toOwnGoal = (ownGoal.position    - ball.position).normalized;

        float alignTarget  = Vector3.Dot(ballVel, toGoal);
        float alignOwnGoal = Vector3.Dot(ballVel, toOwnGoal);

        AddReward(0.15f * Mathf.Max(0f, alignTarget));

        if (alignOwnGoal > 0.6f)
            AddReward(-0.1f * alignOwnGoal);

        EndEpisode();
    }

    new void OnEnable()
    {
        base.OnEnable();
        CheckGoal.OnGoalScored += HandleGoal;
    }

    new void OnDisable()
    {
        base.OnDisable();
        CheckGoal.OnGoalScored -= HandleGoal;
    }

    void HandleGoal(TeamColor scoringTeam)
    {
        bool isMyGoal = scoringTeam == team;
        AddReward(isMyGoal ? +1f : -2f);

        if (isMyGoal) Academy.Instance.StatsRecorder.Add("Custom/GoalScored", 1);
        else Academy.Instance.StatsRecorder.Add("Custom/GoalConceded", 1);

        EndEpisode();
    }
}
