using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

class UIController : MonoBehaviour
{
    private UIDocument uiDocument;
    
    private Label L_RedTeam;
    private Label L_BlueTeam;
    private Label L_Goal;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        L_RedTeam = root.Q<Label>("L_RedTeam");
        L_BlueTeam = root.Q<Label>("L_BlueTeam");
        L_Goal = root.Q<Label>("L_Goal");
        L_Goal.visible = false;
    }

    private void OnEnable()
    {

        GameManager.OnRedScoreChange += UpdateRedScore;
        GameManager.OnBlueScoreChange += UpdateBlueScore;
        
    }
    private void OnDisable()
    {        
        GameManager.OnRedScoreChange -= UpdateRedScore;
        GameManager.OnBlueScoreChange -= UpdateBlueScore;
    }

    private void UpdateRedScore(int newScore)
    {
        L_RedTeam.text = $"Red: {newScore}";
        StartCoroutine(ShowGoalMessage());
    }
    private void UpdateBlueScore(int newScore)
    {
        L_BlueTeam.text = $"Blue: {newScore}";
        StartCoroutine(ShowGoalMessage());
    }

    IEnumerator ShowGoalMessage()
    {
        L_Goal.visible = true;
        yield return new WaitForSeconds(3);
        L_Goal.visible = false;
    }

}