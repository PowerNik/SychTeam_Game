using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController
{
	public abstract void StartHandleInput();

	public abstract void UpdateHandleInput();

	public abstract void StopHandleInput();
}
