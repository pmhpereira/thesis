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
    public List<float> patternsSpawnWeights;
    public List<float> patternsMasteryWeights;

    private int rectDefaultHeight = 70;

    public override Node Create(Vector2 pos)
    {
        PatternSpawnerNode node = CreateInstance<PatternSpawnerNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 210, rectDefaultHeight);
        node.name = "Challenge Spawner";

        node.CreateInput("", "Bool");
        node.CreateInput("", "Blocker");
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
            patternsSpawnWeights = new List<float>();
            patternsMasteryWeights = new List<float>();
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
        if(patternsIndices.Count > 0)
        {
            GUILayout.Label("     Pattern           sW       mW");
        }
        #if UNITY_EDITOR
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 25;
            for(int i = 0; i < patternsIndices.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("x", GUILayout.MaxWidth(25)))
                {
                    patternsIndices.RemoveAt(i);
                    patternsSpawnWeights.RemoveAt(i);
                    patternsMasteryWeights.RemoveAt(i);

                    if(i == patternsIndices.Count)
                    {
                        break;
                    }
                }

                if(patternsIndices.Count > 0)
                {
                    patternsIndices[i] = EditorGUILayout.Popup("", patternsIndices[i], PatternManager.instance.patternsName.ToArray(), GUILayout.MaxWidth(rect.width - 40));
                    patternsSpawnWeights[i] = EditorGUILayout.FloatField("", patternsSpawnWeights[i], GUILayout.MaxWidth(40));
                    patternsSpawnWeights[i] = Mathf.Max(0, patternsSpawnWeights[i]);
                    patternsMasteryWeights[i] = EditorGUILayout.FloatField("", patternsMasteryWeights[i], GUILayout.MaxWidth(40));
                    patternsMasteryWeights[i] = Mathf.Max(0, patternsMasteryWeights[i]);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Pattern"))
            {
                patternsIndices.Add(0);
                patternsSpawnWeights.Add(1);
                patternsMasteryWeights.Add(1);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        #else
        {
            for(int i = 0; i < patternsIndices.Count; i++)
            {
                GUILayout.Label("".PadRight(4) + PatternManager.instance.patternsName[patternsIndices[i]].PadRight(19) + patternsSpawnWeights[i].ToString().PadRight(11) + patternsMasteryWeights[i]);
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
        value = Inputs[0].GetValue<bool>();
        value = value && !Inputs[1].GetValue<bool>();

        if(TreeManager.instance != null)
        {
            TreeManager.instance.UpdateNode(this);
        }

        Outputs[0].SetValue<bool>(value);

        return true;
    }
}
