using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
	//--------------------------------------------------------------------------------
	// Make sure this class is a singleton
	//--------------------------------------------------------------------------------
	private static ScoreManager instance;
	public static ScoreManager Instance { get => instance; }

	[SerializeField] TextMeshProUGUI scoreText;

	private int score = 0;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
			Destroy(this);
	}

	// Laat de score van 0 op het scherm zien.
	private void Start()
	{
		scoreText.text = ("Score: " + score);
	}

	// Voegt een punt toe aan de score als deze aangeroepen word, en laat het zien.
	public void AddToScore()
	{
		score++;
		scoreText.text = ("Score: " + score);
	}
}
