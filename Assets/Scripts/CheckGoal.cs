using UnityEngine;
using System;

public class CheckGoal : MonoBehaviour
{
    public static event Action<string> OnGoalScored;

    [Header("Settings Goal")]
    public string TeamLostPoint;
    public string TeamWinPoint; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            OnGoalScored?.Invoke(TeamWinPoint);

            ResetGame(other.gameObject);
        }
    }

    void ResetGame(GameObject minge)
    {
        minge.transform.position = new Vector3(0, 1, 0);
        if (minge.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}