using UnityEngine;
using NodeEditorFramework;

[Node(false, "Boole Node/Memory", false)]
public class MemoryNode : BaseNode
{
    public const string ID = "memoryNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(.7f, .1f, .7f);

    private bool realValue = false;

    public override Node Create(Vector2 pos)
    {
        MemoryNode node = CreateInstance<MemoryNode>();
        node.rect = new Rect(pos.x, pos.y, 60, 20);
        node.name = "Memory";

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

        realValue = realValue || value;

        Outputs[0].SetValue<bool>(realValue);

        return true;
    }
}
