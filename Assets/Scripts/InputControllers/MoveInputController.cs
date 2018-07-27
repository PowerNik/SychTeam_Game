using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInputController : InputController 
{
	public event Action Interact;
	public event Action<Vector2> Move;
	private Vector2 moveDirection = Vector2.zero;

    protected KeyCode interactKey = KeyCode.Space;

	protected KeyCode upKey = KeyCode.W;
	protected KeyCode leftKey = KeyCode.A;
	protected KeyCode downKey = KeyCode.S;
	protected KeyCode rightKey = KeyCode.D;

    #region InputController
    public override void StartHandleInput()
    {
        
    }

    public override void UpdateHandleInput()
    {
        MoveInput();
        InteractInput();
    }

    public override void StopHandleInput()
    {
        Move(Vector2.zero);
    }

    #endregion

    private void InteractInput()
    {
        if(Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void MoveInput()
    {
        moveDirection = Vector2.zero;

        if (Input.GetKey(upKey))
        {
            moveDirection += Vector2.up;
        }
        if (Input.GetKey(leftKey))
        {
            moveDirection += Vector2.left;
        }
        if (Input.GetKey(downKey))
        {
            moveDirection += Vector2.down;
        }
        if (Input.GetKey(rightKey))
        {
            moveDirection += Vector2.right;
        }

        Move(moveDirection);
    }
}
