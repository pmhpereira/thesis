using UnityEngine;
using NodeEditorFramework;

[Node(false, "Boole/And Node", false)]
public class AndNode : BaseNode
{
    public const string ID = "andNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor;

    public override Node Create(Vector2 pos)
    {
        AndNode node = CreateInstance<AndNode>();
        node.rect = new Rect(pos.x, pos.y, 55, 50); 
        node.name = "And";

        node.CreateInput("", "Bool");
        node.CreateInput("", "Bool");
        node.CreateInput("", "Bool");
        node.CreateInput("", "Bool");
        node.CreateInput("", "Bool");
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

        Inputs[0].SetPosition(5, NodeSide.Top);
        Inputs[1].SetPosition(20, NodeSide.Top);
        Inputs[2].SetPosition(35, NodeSide.Top);
        Inputs[3].SetPosition(10, NodeSide.Left);
        Inputs[4].SetPosition(25, NodeSide.Left);
        Inputs[5].SetPosition(40, NodeSide.Left);

        Outputs[0].SetPosition(25, NodeSide.Right);
    }

    public override bool Calculate()
    {
        bool hasConnection = false;
        bool outputValue = true;

        for(int i = 0; i < Inputs.Count; i++)
        {
            if(Inputs[i].connection)
            {
                hasConnection = true;
                outputValue = outputValue && Inputs[i].GetValue<bool>();
            }
        }

        if(hasConnection)
        {
            value = outputValue;
        }

        Outputs[0].SetValue<bool>(value);

        if (value)
        {
            nodeColor = new Color(0f, 1f, 0f);
        }
        else
        {
            nodeColor = new Color(0f, .65f, 0f);
        }

        return true;
    }
}
