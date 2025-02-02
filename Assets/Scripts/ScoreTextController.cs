﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextController : MonoBehaviour {

    private string _trickText;
    private Text textUI;
    private Color originalColor;
    private int currentScore = 0;

    void Start()
    {
        textUI = GetComponent<Text>();
        originalColor = textUI.color;
        if (!textUI)
        {
            Debug.LogError("No Text Component on this GameObject");
        }
    }

    public void AddScore(int _score)
    {
        currentScore += _score;
        _trickText = currentScore.ToString();
        SetText();
    }

    public void setScoreFail()
    {
        textUI.color = Color.red;
    }

    private void SetText()
    {
        textUI.text = _trickText;
    }

    public void ClearText()
    {
        textUI.color = originalColor;
        currentScore = 0;
        textUI.text = "";
        _trickText = "";
    }

	// Update is called once per frame
	void Update () {
		
	}
}
