using NodeEditorFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TreeManager : MonoBehaviour
{
    public static TreeManager instance;

    public Dictionary<string, bool> tags;
    public Dictionary<string, bool> patterns;

    [HideInInspector]
    public List<TagNode> tagNodes;
    [HideInInspector]
    public List<PatternNode> patternNodes;
    [HideInInspector]
    public List<MasteryNode> masteryNodes;
    [HideInInspector]
    public List<PaceNode> paceNodes;
    [HideInInspector]
    public List<PaceSpawnerNode> paceSpawnerNodes;

    private RuntimeNodeEditor nodeEditor;
    private NodeCanvas nodeCanvas;
    private bool canvasStart;

    public Dropdown assetBrowser;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        nodeEditor = GetComponent<RuntimeNodeEditor>();
        InitializeAssetBrowser();
    }

    private string rootPath = NodeEditor.editorPath + "Resources/Saves";

    void InitializeAssetBrowser()
    {
        string path = rootPath;
        path = path.Split('.')[0];
        path = path.Split(new string[] { "Resources/" }, StringSplitOptions.None)[1];
        NodeCanvas[] objects = Resources.LoadAll<NodeCanvas> (path);

        assetBrowser.options = new List<Dropdown.OptionData>();

        assetBrowser.options.Add(new Dropdown.OptionData("<none>"));
        foreach (NodeCanvas obj in objects)
        {
            assetBrowser.options.Add(new Dropdown.OptionData(obj.name));
        }

        assetBrowser.value = 0;
        OnValueChanged();
    }

    public void OnValueChanged()
    {
        if(assetBrowser.value == 0)
        {
            tags = null;
            tagNodes = new List<TagNode>(); ;
            patterns = null;
            patternNodes = new List<PatternNode>();
            masteryNodes = null;
            nodeEditor.canvas = null;
        }
        else
        {
            InitializeTags();
            InitializePatterns();
            InitializeMasteries();
            InitializePaces();

            nodeEditor.CanvasString = rootPath + "/" + assetBrowser.options[assetBrowser.value].text + ".asset";
            nodeEditor.LoadNodeCanvas(nodeEditor.CanvasString);
        }

        canvasStart = false;
        assetBrowser.captionText.text = assetBrowser.options[assetBrowser.value].text;
    }

    void Update()
    {
        if(!canvasStart)
        {
            canvasStart = true;
            nodeCanvas = nodeEditor.canvas;
        }

        if (nodeCanvas != null)
        {
            NodeEditor.RecalculateAll(nodeCanvas);
        }

        assetBrowser.gameObject.SetActive(GameManager.instance.debugMode);
    }
    
    public void UpdateNode(BaseNode node)
    {
        if(node is PatternNode)
        {
            UpdateNode((PatternNode)node);
        }
        else if(node is TagNode)
        {
            UpdateNode((TagNode)node);
        }
        else if(node is PaceNode)
        {
            UpdateNode((PaceNode)node);
        }
        else if(node is MasteryNode)
        {
            UpdateNode((MasteryNode)node);
        }
        else if(node is PatternSpawnerNode)
        {
            UpdateNode((PatternSpawnerNode)node);
        }
        else if(node is PaceSpawnerNode)
        {
            UpdateNode((PaceSpawnerNode)node);
        }
    }

    public void RemoveNode(BaseNode node)
    {
        if(node is PatternNode)
        {
            throw new NotImplementedException();
        }
        else if(node is TagNode)
        {
            throw new NotImplementedException();
        }
        else if(node is PaceNode)
        {
            RemoveNode((PaceNode) node);
        }
        else if(node is MasteryNode)
        {
            throw new NotImplementedException();
        }
        else if(node is PatternSpawnerNode)
        {
            throw new NotImplementedException();
        }
        else if(node is PaceSpawnerNode)
        {
            throw new NotImplementedException();
        }
    }

    #region Tags
    private void InitializeTags()
    {
        tags = new Dictionary<string, bool>();

        foreach (string tag in Tag.values)
        {
            tags.Add(tag, false);
        }
    }

    public bool IsMechanicEnabled(string tag)
    {
        if(tags == null)
        {
            return true;
        }

        if(!tags[tag])
        {
            Debug.Log("Unresolved dependencies for tag: " + tag);
        }

        return tags[tag];
    }

    private void UpdateNode(TagNode node)
    {
        if (tags != null && node.tag != Tag.None)
        {
            tags[node.tag] = node.value;
        }
    }

    public string GetTagMastery(string tag)
    {
        return TagsManager.instance.tagsInfo[tag].GetMastery();
    }
    #endregion

    #region Patterns
    private void InitializePatterns()
    {
        patterns = new Dictionary<string, bool>();

        foreach (string pattern in PatternManager.instance.patternsName)
        {
            patterns.Add(pattern, false);
        }
    }

    public bool IsPatternEnabled(string pattern)
    {
        if(patterns == null)
        {
            return true;
        }

        return patterns[pattern];
    }

    private void UpdateNode(PatternNode node)
    {
        if(patterns != null)
        {
            patterns[node.pattern] = node.value;
        }
    }

    private void UpdateNode(PatternSpawnerNode node)
    {
        // TODO
        throw new NotImplementedException();
    }

    public PatternSpawnerNode GetRandomPatternSpawnerNode()
    {
        // TODO
        return null;
        throw new NotImplementedException();
    }

    public string GetPatternMastery(string pattern)
    {
        return PatternManager.instance.patternsInfo[pattern].GetMastery();
    }
    #endregion

    #region Mastery
    private void InitializeMasteries()
    {
        masteryNodes = new List<MasteryNode>();
    }

    private void UpdateNode(MasteryNode node)
    {
        if(node.tagNode == null && node.patternNode == null)
        {
            return;
        }

        if(node.tagNode != null)
        {
            UpdateNode(node.tagNode);
        }
        else if(node.patternNode != null)
        {
            UpdateNode(node.patternNode);
        }
    }

    public void AddMasteryNode(MasteryNode node)
    {
        if (!masteryNodes.Contains(node))
        {
            masteryNodes.Add(node);
        }
    }

    public bool IsMasteryResolved(MasteryNode masteryNode, PatternNode patternNode)
    {
        return MasteryComparison.Compare(MasteryComparison.values[masteryNode.masteryComparisonIndex], PatternManager.instance.patternsInfo[patternNode.pattern].GetMastery(), masteryNode.mastery);
    }

    public bool IsMasteryResolved(MasteryNode masteryNode, TagNode tagNode)
    {
        // TODO: implement IsMasteryResolved
        throw new NotImplementedException();
    }
    #endregion

    #region Pace
    private void InitializePaces()
    {
        paceNodes = new List<PaceNode>();
        paceSpawnerNodes = new List<PaceSpawnerNode>();
    }
    
    private void UpdateNode(PaceNode paceNode)
    {
        int index = paceNodes.IndexOf(paceNode);

        if(index < 0)
        {
            paceNodes.Add(paceNode);
        }
        else
        {
            paceNodes[index] = paceNode;
        }
    }

    private void RemoveNode(PaceNode paceNode)
    {
        int paceIndex = paceNodes.IndexOf(paceNode);

        foreach (PaceSpawnerNode paceSpawner in paceSpawnerNodes)
        {
            for(int i = 0; i < paceSpawner.pacesIndices.Count; i++)
            {
                if(paceSpawner.pacesIndices[i] == paceIndex)
                {
                    paceSpawner.pacesIndices.RemoveAt(i);
                    paceSpawner.pacesWeights.RemoveAt(i);
                    i--;
                }
                else if(paceSpawner.pacesIndices[i] > paceIndex)
                {
                    paceSpawner.pacesIndices[i] = paceSpawner.pacesIndices[i] - 1;
                }
            }
        }

        paceNodes.RemoveAt(paceIndex);
    }

    private void UpdateNode(PaceSpawnerNode paceSpawnerNode)
    {
        int index = paceSpawnerNodes.IndexOf(paceSpawnerNode);

        if(index < 0)
        {
            paceSpawnerNodes.Add(paceSpawnerNode);
        }
        else
        {
            paceSpawnerNodes[index] = paceSpawnerNode;
        }
    }

    public PaceSpawnerNode GetRandomPaceSpawnerNode()
    {
        // TODO
        return null;
        throw new NotImplementedException();
    }

    #endregion

    public void ToogleNodeEditor()
    {
        nodeEditor.ToogleSplitscreen();
    }
}
