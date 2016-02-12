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

public class BlockerType : ITypeDeclaration
{
    public string name { get { return "Blocker"; } }
    public Color col { get { return Color.red; } }
    public string InputKnob_TexPath { get { return "Textures/In_Knob.png"; } }
    public string OutputKnob_TexPath { get { return "Textures/Out_Knob.png"; } }
    public Type Type { get { return typeof(bool); } }
    public Color GetColor(object value)
    {
        return (bool)value ? Color.green : Color.red;
    }
}

[Node(false, "Boole Node/Input")]
public class BoolNode : BaseNode
{
    public const string ID = "boolNode";
    public override string GetID { get { return ID; } }

    public override Node Create(Vector2 pos)
    {
        BoolNode node = CreateInstance<BoolNode>();
        node.rect = new Rect(pos.x, pos.y, 50, 20);
        node.name = "Bool";
        node.CreateOutput("", "Bool");
        node.value = true;

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.Input;

        base.DrawOutlinedNode();

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
        
        return true;
    }
}
