﻿using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/OutputPointer", false)]
public class PointerOutputNode : BaseNode
{
    public const string ID = "pointerOutputNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(1f, .4f, 0f);

    [HideInInspector]
    public string pointerName = "";

    public override Node Create(Vector2 pos)
    {
        PointerOutputNode node = CreateInstance<PointerOutputNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 150, 60);
        node.name = "Pointer Output";

        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

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
        if(TreeManager.instance != null && !string.IsNullOrEmpty(pointerName))
        {
            Outputs[0].SetValue<bool>(TreeManager.instance.GetPointerValue(pointerName));
        }

        return true;
    }
}