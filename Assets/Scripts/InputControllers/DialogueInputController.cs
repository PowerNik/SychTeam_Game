using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInputController : InputController
{
    public Action Continued;
    public Action<int> ChoiceChanged;

    public override void UpdateHandleInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Continued();
        }

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChoiceChanged(-1);
            }

        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChoiceChanged(1);
            }
    }

    public override void StopHandleInput()
    {
        
    }
}
