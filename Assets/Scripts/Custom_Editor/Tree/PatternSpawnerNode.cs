using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Spawner/Challenge")]
public class PatternSpawnerNode : BaseNode
{
    public const string ID = "patternSpawnerNode";
    public override string GetID { get { return ID; } }

    [SerializeField][HideInInspector]
    public List<int> patternsIndices;
    public List<float> patternsWeights;

    private int rectDefaultHeight = 60;

    public override Node Create(Vector2 pos)
    {
        PatternSpawnerNode node = CreateInstance<PatternSpawnerNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 180, rectDefaultHeight);
        node.name = "Challenge Spawner";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.PatternSpawner;

        if(patternsIndices == null)
        {
            patternsIndices = new List<int>();
            patternsWeights = new List<float>();
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
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 25;
            for(int i = 0; i < patternsIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("" + i, GUILayout.MaxWidth(25)))
                {
                    patternsIndices.RemoveAt(i);
                    patternsWeights.RemoveAt(i);
                }

                if(patternsIndices.Count > 0)
                {
                    patternsIndices[i] = EditorGUILayout.Popup("", patternsIndices[i], PatternManager.instance.patternsName.ToArray(), GUILayout.MaxWidth(rect.width - 40));
                    patternsWeights[i] = EditorGUILayout.FloatField("", patternsWeights[i], GUILayout.MaxWidth(40));
                    patternsWeights[i] = Mathf.Max(0, patternsWeights[i]);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Pattern"))
            {
                patternsIndices.Add(0);
                patternsWeights.Add(1);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        #else
        {
            for(int i = 0; i < patternsIndices.Count; i++)
            {
                GUILayout.Label(i + ": " + PatternManager.instance.patternsName[patternsIndices[i]] + " | " + patternsWeights[i]);
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

        if(TreeManager.instance != null)
        {
            TreeManager.instance.UpdateNode(this);
        }

        Outputs[0].SetValue<bool>(value);

        return true;
    }
}
