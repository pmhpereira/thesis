using BaseNodeExtensions;
using NodeEditorFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeManager : MonoBehaviour
{
    public static TreeManager instance;

    public Dictionary<string, bool> tags;
    public Dictionary<string, bool> patterns;

    [HideInInspector]
    public List<BaseNode> tagNodes;
    [HideInInspector]
    public List<BaseNode> patternNodes;
    [HideInInspector]
    public List<BaseNode> masteryNodes;
    [HideInInspector]
    public List<BaseNode> paceNodes;
    [HideInInspector]
    public List<BaseNode> paceSpawnerNodes;
    [HideInInspector]
    public List<BaseNode> patternSpawnerNodes;

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
            tagNodes = new List<BaseNode>(); ;
            patterns = null;
            patternNodes = new List<BaseNode>();
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
            throw new NotImplementedException();
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
        patternSpawnerNodes = new List<BaseNode>();

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

        int index = patternNodes.IndexOf(node);

        if(index < 0)
        {
            patternNodes.Add(node);
            patternNodes.SortByCreation();
        }
        else
        {
            patternNodes[index] = node;
        }
    }

    private void UpdateNode(PatternSpawnerNode node)
    {
        int index = patternSpawnerNodes.IndexOf(node);

        if(index < 0)
        {
            patternSpawnerNodes.Add(node);
            patternSpawnerNodes.SortByCreation();
        }
        else
        {
            patternSpawnerNodes[index] = node;
        }
    }

    public PatternSpawnerNode GetRandomPatternSpawnerNode()
    {
        List<PatternSpawnerNode> activePatternSpawners = new List<PatternSpawnerNode>();
        foreach(PatternSpawnerNode patternSpawner in patternSpawnerNodes)
        {
            if(patternSpawner.value)
            {
                activePatternSpawners.Add(patternSpawner);
            }
        }

        if(activePatternSpawners.Count == 0)
        {
            return null;
        }

        return activePatternSpawners[UnityEngine.Random.Range(0, activePatternSpawners.Count)];
    }

    public PatternNode GetRandomPatternNode(PatternSpawnerNode patternSpawner)
    {
        List<PatternNode> activePatternNodes = new List<PatternNode>();
        List<float> weights = new List<float>();
        float totalWeights = 0;

        for(int i = 0; i < patternSpawner.patternsIndices.Count; i++)
        {
            PatternNode currentPatternNode = (PatternNode) patternNodes[patternSpawner.patternsIndices[i]];
            if(currentPatternNode.value)
            {
                activePatternNodes.Add(currentPatternNode);
                weights.Add(patternSpawner.patternsWeights[i]);
                totalWeights += patternSpawner.patternsWeights[i];
            }
        }

        float randomWeight = UnityEngine.Random.Range(0, totalWeights);

        int index = 0;
        float cumulativeWeight = 0;
        for(index = 0; cumulativeWeight < randomWeight ; index++)
        {
            cumulativeWeight += weights[index];
        }

        return activePatternNodes[index - 1];
    }

    public string GetPatternMastery(string pattern)
    {
        return PatternManager.instance.patternsInfo[pattern].GetMastery();
    }
    #endregion

    #region Mastery
    private void InitializeMasteries()
    {
        masteryNodes = new List<BaseNode>();
    }

    public void AddMasteryNode(MasteryNode node)
    {
        if (!masteryNodes.Contains(node))
        {
            masteryNodes.Add(node);
            masteryNodes.SortByCreation();
        }
    }

    public bool IsMasteryResolved(MasteryNode masteryNode, BaseNode node)
    {
        if(node is PatternNode)
        {
            return IsMasteryResolved(masteryNode, (PatternNode)node);
        }
        else if(node is TagNode)
        {
            return IsMasteryResolved(masteryNode, (TagNode)node);
        }
        else if(node is PaceNode)
        {
            return IsMasteryResolved(masteryNode, (PaceNode)node);
        }
        else if(node is MasteryNode)
        {
            throw new NotImplementedException();
        }
        else if(node is PatternSpawnerNode)
        {
            return IsMasteryResolved(masteryNode, (PatternSpawnerNode)node);
        }
        else if(node is PaceSpawnerNode)
        {
            return IsMasteryResolved(masteryNode, (PaceSpawnerNode)node);
        }

        return false;
    }

    private bool IsMasteryResolved(MasteryNode masteryNode, PatternNode node)
    {
        return MasteryComparison.Compare(MasteryComparison.values[masteryNode.masteryComparisonIndex], PatternManager.instance.patternsInfo[node.pattern].GetMastery(), masteryNode.mastery);
    }

    private bool IsMasteryResolved(MasteryNode masteryNode, TagNode node)
    {
        return MasteryComparison.Compare(MasteryComparison.values[masteryNode.masteryComparisonIndex], TagsManager.instance.tagsInfo[node.tag].GetMastery(), masteryNode.mastery);
    }
    
    private bool IsMasteryResolved(MasteryNode masteryNode, PaceNode node)
    {
        return MasteryComparison.Compare(MasteryComparison.values[masteryNode.masteryComparisonIndex], PaceManager.instance.pacesInfo[node.paceName].GetMastery(), masteryNode.mastery);
    }

    private bool IsMasteryResolved(MasteryNode masteryNode, PatternSpawnerNode node)
    {
        bool isResolved = (node.patternsIndices.Count > 0) ? true : false;

        for(int i = 0; i < node.patternsIndices.Count; i++)
        {
            isResolved = isResolved && IsMasteryResolved(masteryNode, (PatternNode)patternNodes[node.patternsIndices[i]]);
        }

        return isResolved;
    }

    private bool IsMasteryResolved(MasteryNode masteryNode, PaceSpawnerNode node)
    {
        bool isResolved = (node.pacesIndices.Count > 0) ? true : false;

        for(int i = 0; i < node.pacesIndices.Count; i++)
        {
            isResolved = isResolved && IsMasteryResolved(masteryNode, paceNodes[node.pacesIndices[i]]);
        }

        return isResolved;
    }
    #endregion

    #region Pace
    private void InitializePaces()
    {
        paceNodes = new List<BaseNode>();
        paceSpawnerNodes = new List<BaseNode>();
    }
    
    private void UpdateNode(PaceNode node)
    {
        int index = paceNodes.IndexOf(node);

        if(index < 0)
        {
            paceNodes.Add(node);
            PaceManager.instance.SetPacesInfo(node.paceName, new PaceInfo(node.paceName, node.instancesCount)); 
            paceNodes.SortByCreation();
        }
        else
        {
            paceNodes[index] = node;
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

    private void UpdateNode(PaceSpawnerNode node)
    {
        int index = paceSpawnerNodes.IndexOf(node);

        if(index < 0)
        {
            paceSpawnerNodes.Add(node);
            paceSpawnerNodes.SortByCreation();
        }
        else
        {
            paceSpawnerNodes[index] = node;
        }
    }

    public PaceSpawnerNode GetRandomPaceSpawnerNode()
    {
        List<PaceSpawnerNode> activePaceSpawners = new List<PaceSpawnerNode>();
        foreach(PaceSpawnerNode paceSpawner in paceSpawnerNodes)
        {
            if(paceSpawner.value)
            {
                activePaceSpawners.Add(paceSpawner);
            }
        }

        if(activePaceSpawners.Count == 0)
        {
            return null;
        }

        return activePaceSpawners[UnityEngine.Random.Range(0, activePaceSpawners.Count)];
    }

    public PaceNode GetRandomPaceNode(PaceSpawnerNode paceSpawner)
    {
        List<PaceNode> activePaceNodes = new List<PaceNode>();
        List<float> weights = new List<float>();
        float totalWeights = 0;

        for(int i = 0; i < paceSpawner.pacesIndices.Count; i++)
        {
            PaceNode currentPaceNode = (PaceNode) paceNodes[paceSpawner.pacesIndices[i]];
            if(currentPaceNode.value)
            {
                activePaceNodes.Add(currentPaceNode);
                weights.Add(paceSpawner.pacesWeights[i]);
                totalWeights += paceSpawner.pacesWeights[i];
            }
        }

        float randomWeight = UnityEngine.Random.Range(0, totalWeights);

        int index = 0;
        float cumulativeWeight = 0;
        for(index = 0; cumulativeWeight < randomWeight ; index++)
        {
            cumulativeWeight += weights[index];
        }

        return activePaceNodes[index - 1];
    }
    #endregion

    public void ToogleNodeEditor()
    {
        nodeEditor.ToogleSplitscreen();
    }
}
