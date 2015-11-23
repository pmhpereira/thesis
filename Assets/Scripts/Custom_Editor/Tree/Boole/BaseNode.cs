using UnityEngine;
using System.Collections;
using NodeEditorFramework;
using System;

[Node(true, "", false)]

public class BaseNode : Node
{
    public bool value;

    public override string GetID
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override bool Calculate()
    {
        throw new NotImplementedException();
    }

    public override Node Create(Vector2 pos)
    {
        throw new NotImplementedException();
    }

    public override void NodeGUI()
    {
        if (NodeEditor.curEditorState.selectedNode == this)
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Space)
                {
                    value = !value;
                }
            }
        }
    }

    public bool GetValue()
    {
        return value;
    }
}
