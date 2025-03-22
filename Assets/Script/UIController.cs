using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtScore;
    private int score;

    private void OnEnable()
    {
        txtScore.text = "0";
        ObserverManager<EventID>.AddRegisterEvent(EventID.UpdateScore, param => UpdateViewScore((int)param));
    }

    private void OnDisable()
    {
        ObserverManager<EventID>.RemoveAddListener(EventID.UpdateScore, param => UpdateViewScore((int)param));
    }

    private void UpdateViewScore(int value)
    {
        score += value;
        txtScore.text = score.ToString();
    }
}
