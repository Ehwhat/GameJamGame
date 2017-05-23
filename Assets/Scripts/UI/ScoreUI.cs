using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {

    public Text text;

    void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        text.text = "Score: " + score.ToString();
    }

}
