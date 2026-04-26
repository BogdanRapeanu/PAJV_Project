using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject NPC_Prefab;

    public static event Action<int> OnBlueScoreChange;
    public static event Action<int> OnRedScoreChange;

    [SerializeField] private Vector3 RedGoalLocation;
    [SerializeField] private Vector3 BlueGoalLocation;
    [SerializeField] private Vector3 BallLocation;

    [SerializeField] private List<Transform> jucatoriRosii = new List<Transform>();
    [SerializeField] private List<Transform> jucatoriAlbastri = new List<Transform>();

    private int scoreRed = 0;
    private int scoreBlue = 0;

    void OnEnable()
    {
        CheckGoal.OnGoalScored += UpdateScore;
    }

    void OnDisable()
    {
        CheckGoal.OnGoalScored -= UpdateScore;
    }

    void UpdateScore(string Team)
    {
        if (Team == "RedTeam")
        {
            scoreRed++;
            OnRedScoreChange?.Invoke(scoreRed);            
        }
        else if (Team == "BlueTeam")
        {
            scoreBlue++;
            OnBlueScoreChange?.Invoke(scoreBlue);
        }

        Debug.Log($"Score: Red {scoreRed} - Blue {scoreBlue}");
    }
}