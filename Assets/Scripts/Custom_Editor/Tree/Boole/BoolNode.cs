using UnityEngine;
using NodeEditorFramework;
using System;

public class BoolType : ITypeDeclaration
{
    public string name { get { return "Bool"; } }
    public Color col { get { return Color.cyan; } }
    public string InputKnob_TexPath { get { return "Textures/In_Knob.png"; } }
    public string OutputKnob_TexPath { get { return "Textures/Out_Knob.png"; } }
    public Type Type { get { return typeof(bool); } }
    public Color GetColor(object value)
    {
        return (bool)value ? Color.green : Color.red;
    }
}

[Node(false, "Boole/Input Node", false)]
public class BoolNode : BaseNode
{
    public const string ID = "boolNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor;

    public override Node Create(Vector2 pos)
    {
        BoolNode node = CreateInstance<BoolNode>();
        node.rect = new Rect(pos.x, pos.y, 50, 20);
        node.name = "Bool";
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

        Outputs[0].SetPosition(10, NodeSide.Right);
    }

    public override bool Calculate()
    {
        Outputs[0].SetValue<bool>(value);

        if (value)
        {
            nodeColor = new Color(.1f, .7f, 1f);
        }
        else
        {
            nodeColor = new Color(0f, .1f, .7f);
        }

        return true;
    }
}
