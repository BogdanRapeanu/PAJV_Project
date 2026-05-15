using UnityEngine;
using System;

public class CheckGoal : MonoBehaviour
{
    public static event Action<TeamColor> OnGoalScored;

    [Header("Settings Goal")]
    public TeamColor teamWinPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
            OnGoalScored?.Invoke(teamWinPoint);
    }
}
