using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _score;

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            _scoreText.text = $"Score: {_score}";
        }
    }

    private void Start()
    {
        Score = 0;
    }

    //Use this void to add score
    public void AddScore(int score)
    {
        Score += score;
    }
}
