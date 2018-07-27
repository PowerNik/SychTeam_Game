using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType { Move, Dialogue };

public class InputManager : MonoBehaviour 
{
	private InputType currentInput;

	private Dictionary<InputType, InputController> inputs = new Dictionary<InputType, InputController>();

	public MoveInputController MoveInput
	{
		get { return (MoveInputController)inputs[InputType.Move]; }
	}
	
	void Awake () 
	{
		inputs[InputType.Move] = new MoveInputController();
		inputs[InputType.Dialogue] = new DialogueInputController();

		currentInput = InputType.Move;
	}
	
	void Update () 
	{
		inputs[currentInput].HandleInput();
	}

	public void ChangeInput(InputType type)
	{
		StartCoroutine(ChangeInputCoroutine(type));
	}

	private IEnumerator ChangeInputCoroutine(InputType type)
	{
		yield return null;

		inputs[currentInput].StopHandleInput();
		currentInput = type;
	}
}
