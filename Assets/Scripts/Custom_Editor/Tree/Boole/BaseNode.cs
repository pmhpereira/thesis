using UnityEngine;
using System.Collections;
using NodeEditorFramework;
using System;

[Node(true, "", false)]

public class BaseNode : Node
{
    public bool value;
    private Texture2D outlineTexture;

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

    public void DrawOutlinedNode()
    {
        if(value)
        {
            if(outlineTexture == null)
            {
                outlineTexture = NodeEditorGUI.Tint(Texture2D.whiteTexture, Color.green);
            }

            Rect outlineRect = new Rect(rect);
            outlineRect.width += 2;
            outlineRect.height += 2;
            outlineRect.x -= 1;
            outlineRect.y -= 1;
            outlineRect.position += NodeEditor.curEditorState.zoomPanAdjust;
            GUI.DrawTexture(outlineRect, outlineTexture);
        }

        base.DrawNode();
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
