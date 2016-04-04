using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Pointer/Input")]
public class PointerInputNode : BaseNode
{
    public const string ID = "pointerInputNode";
    public override string GetID { get { return ID; } }

    [HideInInspector]
    public string pointerName = "";

    public override Node Create(Vector2 pos)
    {
        PointerInputNode node = CreateInstance<PointerInputNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 150, 60);
        node.name = "Pointer Input";

        node.CreateInput("", "Bool");

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.Pointer;

        base.DrawOutlinedNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        #if UNITY_EDITOR
            pointerName = EditorGUILayout.TextField("", pointerName, GUILayout.MaxWidth(rect.width - 20));
        #else
            GUILayout.Label(pointerName);
        #endif
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        for (int i = 0; i < Outputs.Count; i++)
        {
            Outputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    public override bool Calculate()
    {
        if (Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }

        if(TreeManager.instance != null)
        {
            TreeManager.instance.UpdateNode(this);
        }

        return true;
    }
}
