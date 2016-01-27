using UnityEngine;
using NodeEditorFramework;
using System;
using NodeEditorFramework.Utilities;

[Node(true, "")]
public class BaseNode : Node
{
    public bool value;
    public int creationId;

    private Texture2D outlineTexture;
    private static int id = 0;

    private int lineWidth = 2;

    public static int GetNextId()
    {
        return ++id;
    }

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
                outlineTexture = RTEditorGUI.Tint(Texture2D.whiteTexture, Color.green);
            }

            Rect outlineRect = new Rect(rect);
            outlineRect.width += lineWidth * 2;
            outlineRect.height += lineWidth * 2;
            outlineRect.x -= lineWidth;
            outlineRect.y -= lineWidth;
            outlineRect.position += NodeEditor.curEditorState.zoomPanAdjust;
            GUI.DrawTexture(outlineRect, outlineTexture);
        }

        base.DrawNode();
    }

    public override void NodeGUI()
    {
        if (NodeEditor.curEditorState.selectedNode == this)
        {
            if(this is BoolNode)
            {
                Event e = Event.current;

                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.Z)
                    {
                        value = !value;
                    }
                }
            }
        }
    }

    public bool GetValue()
    {
        return value;
    }
}
