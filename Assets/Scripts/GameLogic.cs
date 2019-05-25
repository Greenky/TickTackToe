using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
	public bool playerTurn;
	[SerializeField] private Text _playerWinsText;
	[SerializeField] private Text _AIWinsText;
	[SerializeField] private Text _turnText;
	[SerializeField] private Text _winText;
	[SerializeField] private GameObject _pausePanel;
	enum CellsType { empty, filled };
	public enum Difficulty { Easy, Middle, Hard };
	public enum SignType { X_sign, O_sign };

	void Start()
	{
		_playerWinsText.text = "Player: " + PlayerPrefs.GetInt("Player Wins");
		_AIWinsText.text = "AI: " + PlayerPrefs.GetInt("AI Wins");
		_pausePanel.SetActive(false);
		playerTurn = true;
		if (_turnText == null)
			Debug.LogError("No turn text");
	}

	private void Update()
	{
		if (playerTurn)
			_turnText.text = "Turn: Player";
		else
			_turnText.text = "Turn: AI";

		if (Input.GetKeyDown(KeyCode.Escape))
			_pausePanel.SetActive(!_pausePanel.activeSelf);
	}

	public void AIMove()
	{
		ArrayList emptyCells = FindCells(CellsType.empty);

		if (emptyCells.Count == 0)
			ShowWinText("None");
		else
		{
			if (PlayerPrefs.GetInt("Difficulty") == (int)Difficulty.Easy)
			{
				// easy bot algorythm
				EasyBotLogic(emptyCells);
			}
			else if (PlayerPrefs.GetInt("Difficulty") == (int)Difficulty.Middle)
			{
				// middle bot algorithm
				MiddleBotLogic(emptyCells);
			}
			else
			{
				// hard bot algorythm
				HardBotLogic(emptyCells);
			}

			if (CheckForWin(SignType.O_sign) == true)
				ShowWinText("AI");
			else
				playerTurn = true;
		}
	}

	private ArrayList FindCells(CellsType toFind)
	{
		ArrayList searchingCells = new ArrayList();
		GameObject[] allCells = GameObject.FindGameObjectsWithTag("Cell");
		foreach (GameObject cell in allCells)
		{
			if (toFind == CellsType.empty && cell.GetComponent<Cell>().filled == false)
				searchingCells.Add(cell);
			else if (toFind == CellsType.filled && cell.GetComponent<Cell>().filled == true)
				searchingCells.Add(cell);
		}
		return searchingCells;
	}

	// easy bot
	private void EasyBotLogic(ArrayList emptyCells)
	{
		Debug.Log("Random");
		int randValue = Random.Range(0, emptyCells.Count);
		GameObject cellToFill = (GameObject)emptyCells[randValue];
		cellToFill.GetComponent<Cell>().FillWith(SignType.O_sign);
	}

	// middle bot
	private void MiddleBotLogic(ArrayList emptyCells)
	{
		ArrayList filledCells = FindCells(CellsType.filled);
		ArrayList XArray = new ArrayList();
		ArrayList OArray = new ArrayList();
		foreach (GameObject cell in filledCells)
		{
			if (cell.GetComponent<Cell>().filledWithX)
				XArray.Add(cell);
			else
				OArray.Add(cell);
		}

		if (XArray.Count < 2)
			EasyBotLogic(emptyCells);
		else
		{
			if (AttackDefenceLogic(emptyCells, XArray, OArray) == true)
				return;
			else
				EasyBotLogic(emptyCells);
		}
	}

	// hard bot
	private void HardBotLogic(ArrayList emptyCells)
	{
		ArrayList filledCells = FindCells(CellsType.filled);
		ArrayList XArray = new ArrayList();
		ArrayList OArray = new ArrayList();
		foreach (GameObject cell in filledCells)
		{
			if (cell.GetComponent<Cell>().filledWithX)
				XArray.Add(cell);
			else
				OArray.Add(cell);
		}

		// Defence or Attack if Needed
		if (AttackDefenceLogic(emptyCells, XArray, OArray) == true)
			return;

		// fill middle if not filled or corners
		if (GameObject.Find("Button5").GetComponent<Cell>().filled == false)
		{
			Debug.Log("Middle fill");
			GameObject.Find("Button5").GetComponent<Cell>().FillWith(SignType.O_sign);
			return;
		}
		else
		{
			Debug.Log("Corners");
			for (int i = 1; i < 10; i += 2)
			{
				string buttonName = "Button" + i;
				if (GameObject.Find(buttonName).GetComponent<Cell>().filled == false)
				{
					GameObject.Find(buttonName).GetComponent<Cell>().FillWith(SignType.O_sign);
					return;
				}
			}
			EasyBotLogic(emptyCells);	
		}
	}

	private bool AttackDefenceLogic(ArrayList emptyCells, ArrayList XArray, ArrayList OArray)
	{
		// Attack logic
		foreach (GameObject cell1 in OArray)
		{
			foreach (GameObject cell2 in OArray)
			{
				if (cell1 == cell2)
					continue;
				foreach (GameObject cell3 in emptyCells)
				{
					if (VerticalCheck(cell1, cell2, cell3) ||
					HorizontalCheck(cell1, cell2, cell3) ||
					DiagonalCheck(cell1, cell2, cell3))
					{
						Debug.Log("Attack");
						cell3.GetComponent<Cell>().FillWith(SignType.O_sign);
						return true;
					}
				}
			}
		}
		// ----------------

		// Defence logic
		foreach (GameObject cell1 in XArray)
		{
			foreach (GameObject cell2 in XArray)
			{
				if (cell1 == cell2)
					continue;
				foreach (GameObject cell3 in emptyCells)
				{
					if (VerticalCheck(cell1, cell2, cell3) ||
					HorizontalCheck(cell1, cell2, cell3) ||
					DiagonalCheck(cell1, cell2, cell3))
					{
						Debug.Log("Defence");
						cell3.GetComponent<Cell>().FillWith(SignType.O_sign);
						return true;
					}
				}
			}
		}
		// ---------------
		return false;
	}

	// cheking player win or not
	public bool CheckForWin(SignType sign)
	{
		ArrayList filledCells = FindCells(CellsType.filled);
		ArrayList neededArray = new ArrayList();

		foreach (GameObject cell in filledCells)
		{
			if (sign == SignType.X_sign && cell.GetComponent<Cell>().filledWithX)
				neededArray.Add(cell);
			else if (sign == SignType.O_sign && !cell.GetComponent<Cell>().filledWithX)
				neededArray.Add(cell);
		}

		if (neededArray.Count < 3)
			return false;
	
		foreach (GameObject xCell1 in neededArray)
		{
			foreach (GameObject xCell2 in neededArray)
			{
				if (xCell1 == xCell2)
					continue;
				foreach (GameObject xCell3 in neededArray)
				{
					if (VerticalCheck(xCell1, xCell2, xCell3) ||
					HorizontalCheck(xCell1, xCell2, xCell3) ||
					DiagonalCheck(xCell1, xCell2, xCell3))
						return true;
				}
			}
		}
		return false;
	}

	// Check if cells forms horizottal line
	private bool HorizontalCheck(GameObject cell1, GameObject cell2, GameObject cell3)
	{
		if (cell1 == cell3 || cell2 == cell3)
			return false;
		
		if (cell1.GetComponent<Cell>().xPos == cell2.GetComponent<Cell>().xPos &&
		cell2.GetComponent<Cell>().xPos == cell3.GetComponent<Cell>().xPos)
			return true;
		else
			return false;
	}

	// Check if cells forms vertical line
	private bool VerticalCheck(GameObject cell1, GameObject cell2, GameObject cell3)
	{
		if (cell1 == cell3 || cell2 == cell3)
			return false;

		if (cell1.GetComponent<Cell>().yPos == cell2.GetComponent<Cell>().yPos &&
		cell2.GetComponent<Cell>().yPos == cell3.GetComponent<Cell>().yPos)
			return true;
		else
			return false;
	}

	// Check if cells forms diaginal line
	private bool DiagonalCheck(GameObject cell1, GameObject cell2, GameObject cell3)
	{
		if (cell1 == cell3 || cell2 == cell3)
			return false;

		// sorting cells from right to left
		GameObject[] cellList = { cell1, cell2, cell3 };

		for (int i = 0; i < cellList.Length - 1; i++)
		{
			if (cellList[i].GetComponent<Cell>().xPos > cellList[i + 1].GetComponent<Cell>().xPos)
			{
				GameObject buff = cellList[i];
				cellList[i] = cellList[i + 1];
				cellList[i + 1] = buff;
				i = -1;
			}
			else if (cellList[i].GetComponent<Cell>().xPos == cellList[i + 1].GetComponent<Cell>().xPos ||
			cellList[i].GetComponent<Cell>().yPos == cellList[i + 1].GetComponent<Cell>().yPos)
				return false;
		}
		// -------------------------

		if ((cellList[0].GetComponent<Cell>().yPos < cellList[1].GetComponent<Cell>().yPos &&
			cellList[1].GetComponent<Cell>().yPos < cellList[2].GetComponent<Cell>().yPos) ||
			(cellList[0].GetComponent<Cell>().yPos > cellList[1].GetComponent<Cell>().yPos &&
			cellList[1].GetComponent<Cell>().yPos > cellList[2].GetComponent<Cell>().yPos))
		{
			return true;
		}
		return false;
	}

	public void ShowWinText(string who)
	{
		_winText.text = who + " win";
		if (who == "AI")
			PlayerPrefs.SetInt("AI Wins", PlayerPrefs.GetInt("AI Wins") + 1);
		else if (who == "Player")
			PlayerPrefs.SetInt("Player Wins", PlayerPrefs.GetInt("Player Wins") + 1);
		else
			_winText.text = "Draw";
		_pausePanel.SetActive(true);
	}

	public void ClickOnRestart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ClickOnExit()
	{
		SceneManager.LoadScene("Menu");
	}
}
