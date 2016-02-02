using UnityEngine;
using NodeEditorFramework;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Spawner/Pace")]
public class PaceSpawnerNode : BaseNode
{
    public const string ID = "paceSpawnerNode";
    public override string GetID { get { return ID; } }

    [SerializeField][HideInInspector]
    public List<int> pacesIndices;
    public List<float> pacesSpawnWeights;
    public List<float> pacesMasteryWeights;

    private int rectDefaultHeight = 70;

    public override Node Create(Vector2 pos)
    {
        PaceSpawnerNode node = CreateInstance<PaceSpawnerNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 210, rectDefaultHeight);
        node.name = "Pace Spawner";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.PaceSpawner;

        if(pacesIndices == null)
        {
            pacesIndices = new List<int>();
            pacesSpawnWeights = new List<float>();
            pacesMasteryWeights = new List<float>();
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

        if(pacesIndices.Count > 0)
        {
            GUILayout.Label("#   Pace              sW       mW");
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
                    pacesSpawnWeights.RemoveAt(i);
                    pacesMasteryWeights.RemoveAt(i);

                    if(i == pacesIndices.Count)
                    {
                        break;
                    }
                }

                if(paceNodes.Length > 0)
                {
                    pacesIndices[i] = EditorGUILayout.Popup("", pacesIndices[i], paceNames, GUILayout.MaxWidth(rect.width - 40));
                    pacesSpawnWeights[i] = EditorGUILayout.FloatField("", pacesSpawnWeights[i], GUILayout.MaxWidth(40));
                    pacesSpawnWeights[i] = Mathf.Max(0, pacesSpawnWeights[i]);
                    pacesMasteryWeights[i] = EditorGUILayout.FloatField("", pacesMasteryWeights[i], GUILayout.MaxWidth(40));
                    pacesMasteryWeights[i] = Mathf.Max(0, pacesMasteryWeights[i]);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Pace"))
            {
                pacesIndices.Add(0);
                pacesSpawnWeights.Add(1);
                pacesMasteryWeights.Add(1);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        #else
        {
            for(int i = 0; i < pacesIndices.Count; i++)
            {
                GUILayout.Label(i.ToString().PadRight(4) + paceNames[pacesIndices[i]].PadRight(19) + pacesSpawnWeights[i].ToString().PadRight(11) + pacesMasteryWeights[i]);
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
