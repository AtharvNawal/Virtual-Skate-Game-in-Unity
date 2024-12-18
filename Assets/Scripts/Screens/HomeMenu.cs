
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenu : MonoBehaviour, IScreen
{
    private GameController _game;

    public string routeName
    {
        get
        {
            return "Home";
        }
    }

    public void init(GameController game)
    {
        _game = game;
        enableMe();
    }

    public void enableMe()
    {
        gameObject.SetActive(true);
    }

    public void disableMe()
    {
        gameObject.SetActive(false);
    }

    public void onStartBtn()
    {
        _game.resolveRoute("Game");
    }
	
}

/*
using UnityEngine;
using UnityEngine.UI; // or TMPro if using TextMeshPro

public class HomeMenu : MonoBehaviour
{
    private GameController _game;

    // Reference to the UI Text component
    public Text titleText; // Use TextMeshProUGUI if using TextMeshPro

    public string routeName
    {
        get
        {
            return "Home";
        }
    }

    public void init(GameController game)
    {
        _game = game;
        enableMe();
    }

    public void enableMe()
    {
        gameObject.SetActive(true);
        // Change the text to whatever you want
        titleText.text = "New Skateboarding Title"; // Set your desired text here
    }

    public void disableMe()
    {
        gameObject.SetActive(false);
    }

    public void onStartBtn()
    {
        _game.resolveRoute("Game");
    }
}
*/