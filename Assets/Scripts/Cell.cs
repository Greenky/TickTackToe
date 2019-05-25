using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
	[SerializeField] private Sprite _Xsprite;
	[SerializeField] private Sprite _Osprite;
	[SerializeField] private Image _insideImage;
	private GameLogic _gl;
	public int xPos;
	public int yPos;
	public bool filledWithX;

	public bool filled;

	private void Start()
	{
		filledWithX = false;
		xPos = (int)transform.position.x;
		yPos = (int)transform.position.y;
		_gl = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		filled = false;
		//Debug.Log(transform.name + "  X: " + xPos + "   Y: " + yPos);
	}

	public void OnClick()
	{
		if (_gl.playerTurn)
		{
			FillWith(GameLogic.SignType.X_sign);
			_gl.playerTurn = false;
			if (_gl.CheckForWin(GameLogic.SignType.X_sign) == false)
				StartCoroutine(ChangeMove());
			else
				_gl.ShowWinText("Player");
		}
	}

	public void FillWith(GameLogic.SignType signType)
	{
		_insideImage.color = Color.white;
		_insideImage.type = Image.Type.Simple;
		if (signType == GameLogic.SignType.X_sign)
		{
			_insideImage.sprite = _Xsprite;
			filledWithX = true;
		}
		else
			_insideImage.sprite = _Osprite;
		GetComponent<Button>().interactable = false;
		filled = true;
	}

	private IEnumerator ChangeMove()
	{
		yield return new WaitForSeconds(0.5f);
		_gl.AIMove();
	}
}
