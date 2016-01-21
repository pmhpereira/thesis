using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Spawner|Pace", false)]
public class PaceSpawnerNode : BaseNode
{
    public const string ID = "paceSpawnerNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(1f, .4f, 0f);

    [SerializeField][HideInInspector]
    public List<int> pacesIndices;
    public List<float> pacesWeights;

    private int rectDefaultHeight = 60;

    public override Node Create(Vector2 pos)
    {
        PaceSpawnerNode node = CreateInstance<PaceSpawnerNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 180, rectDefaultHeight);
        node.name = "Pace Spawner";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        if(pacesIndices == null)
        {
            pacesIndices = new List<int>();
            pacesWeights = new List<float>();
        }
        rect.height = rectDefaultHeight + pacesIndices.Count * 20;

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
        BaseNode[] paceNodes = TreeManager.instance.paceNodes.ToArray();
        string[] paceNames = new string[paceNodes.Length];
        for(int p = 0; p < paceNodes.Length; p++)
        {
            paceNames[p] = ((PaceNode) paceNodes[p]).paceName;
        }

        #if UNITY_EDITOR
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 25;
            for (int i = 0; i < pacesIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("" + i, GUILayout.MaxWidth(25)))
                {
                    pacesIndices.RemoveAt(i);
                    pacesWeights.RemoveAt(i);
                }

                if(paceNodes.Length > 0)
                {
                    pacesIndices[i] = EditorGUILayout.Popup("", pacesIndices[i], paceNames, GUILayout.MaxWidth(rect.width - 40));
                    pacesWeights[i] = EditorGUILayout.FloatField("", pacesWeights[i], GUILayout.MaxWidth(40));
                    pacesWeights[i] = Mathf.Max(0, pacesWeights[i]);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Pace"))
            {
                pacesIndices.Add(0);
                pacesWeights.Add(1);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        #else
        {
            for(int i = 0; i < patternsIndices.Count; i++)
            {
                GUILayout.Label(i + ": " + PatternManager.instance.patternsName[patternsIndices[i]] + " | " + pacesWeights[i]);
            }
        }
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
        else
        {
            value = true;
        }

        if(TreeManager.instance != null)
        {
            TreeManager.instance.UpdateNode(this);
        }

        Outputs[0].SetValue<bool>(value);

        return true;
    }
}
