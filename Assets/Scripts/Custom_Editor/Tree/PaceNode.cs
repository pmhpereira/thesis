using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Pace", false)]
public class PaceNode : BaseNode
{
    public const string ID = "paceNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(1f, .4f, 0f);

    [HideInInspector]
    public int paceIndex;
    [HideInInspector]
    public int instancesCount;
    [SerializeField][HideInInspector]
    public List<int> patternsIndices;

    private int rectDefaultHeight = 100;

    public override Node Create(Vector2 pos)
    {
        PaceNode node = CreateInstance<PaceNode>();
        node.rect = new Rect(pos.x, pos.y, 150, rectDefaultHeight);
        node.name = "Pace";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        if(patternsIndices == null)
        {
            patternsIndices = new List<int>();
        }
        rect.height = rectDefaultHeight + patternsIndices.Count * 20;

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
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 70;
            paceIndex = EditorGUILayout.Popup("", paceIndex, Pace.values.ToArray(), GUILayout.MaxWidth(rect.width - 20));
            instancesCount = EditorGUILayout.IntField("Instances", instancesCount);
            instancesCount = Mathf.Max(1, instancesCount);
            EditorGUIUtility.labelWidth = 25;
            for(int i = 0; i < patternsIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("" + i, GUILayout.MaxWidth(25)))
                {
                    patternsIndices.RemoveAt(i);
                }
                patternsIndices[i] = EditorGUILayout.Popup("", patternsIndices[i], PatternManager.instance.patternsName.ToArray(), GUILayout.MaxWidth(rect.width - 20));
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Pattern"))
            {
                patternsIndices.Add(0);
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;
        #else
            GUILayout.Label(Pace.values[paceIndex]);
            GUILayout.Label("Instances: " + instancesCount);
            for(int i = 0; i < patternsIndices.Count; i++)
            {
                GUILayout.Label(i + ": " + PatternManager.instance.patternsName[patternsIndices[i]]);
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

        if(TreeManager.instance != null)
        {
            TreeManager.instance.UpdatePaceNode(this);
        }

        Outputs[0].SetValue<bool>(value);

        return true;
    }
}
