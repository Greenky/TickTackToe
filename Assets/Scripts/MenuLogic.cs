using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
	[SerializeField] Slider difficultySlider;
	[SerializeField] Text diffText;

	private void Awake()
	{
		difficultySlider.value = PlayerPrefs.GetInt("Difficulty");
		switch ((int)difficultySlider.value) {
			case 0:
				diffText.text = "Easy";
				break;
			case 1:
				diffText.text = "Normal";
				break;
			case 2:
				diffText.text = "Insane";
				break;
		}
	}

	public void OnStartClick()
	{
		SceneManager.LoadScene("Game");
	}

	public void OnExitClick()
	{
		Application.Quit();
	}

	public void OnClearClick()
	{
		PlayerPrefs.SetInt("Player Wins", 0);
		PlayerPrefs.SetInt("AI Wins", 0);
	}

	public void OnChangeSliderValue()
	{
		PlayerPrefs.SetInt("Difficulty", (int)difficultySlider.value);
		switch ((int)difficultySlider.value)
		{
			case 0:
				diffText.text = "Easy";
				break;
			case 1:
				diffText.text = "Normal";
				break;
			case 2:
				diffText.text = "Insane";
				break;
		}
	}
}
