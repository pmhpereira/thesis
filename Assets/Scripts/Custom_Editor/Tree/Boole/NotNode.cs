﻿using UnityEngine;
using NodeEditorFramework;

[Node(false, "Boole Node/Not", false)]
public class NotNode : BaseNode
{
    public const string ID = "notNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(.7f, .1f, .7f);

    public override Node Create(Vector2 pos)
    {
        NotNode node = CreateInstance<NotNode>();
        node.rect = new Rect(pos.x, pos.y, 50, 20);
        node.name = "Not";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");
        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        base.DrawNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        Inputs[0].SetPosition(10, NodeSide.Left);
        Outputs[0].SetPosition(10, NodeSide.Right);
    }

    public override bool Calculate()
    {
        if (Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }

        Outputs[0].SetValue<bool>(!value);

        return true;
    }
}