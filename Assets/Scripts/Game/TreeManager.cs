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
    [HideInInspector]
    public List<BaseNode> pointerInputNodes;

    private RuntimeNodeEditor nodeEditor;
    private bool canvasStart;

    public Button refreshButton;
    public Dropdown assetBrowser;
    public Button validateButton;
    
    public int oldAssetValue;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        nodeEditor = GetComponent<RuntimeNodeEditor>();
        nodeEditor.canvas = NodeEditor.curNodeCanvas;
        InitializeAssetBrowser();
    }

    private string rootPath = NodeEditor.editorPath + "Resources/Saves";

    public void RefreshAssetBrowser()
    {
        string oldAssetName = assetBrowser.options[assetBrowser.value].text;
        InitializeAssetBrowser();

        int newAssetIndex = 0;
        for(int i = 0; i < assetBrowser.options.Count; i++)
        {
            if(assetBrowser.options[i].text == oldAssetName)
            {
                newAssetIndex = i;
                break;
            }
        }

        assetBrowser.value = newAssetIndex;
        OnValueChanged();
    }

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
            nodeEditor.enabled = false;
        }
        else
        {
            if(oldAssetValue != 0)
            {
                GameManager.instance.SaveSnapshot(0);
            }

            nodeEditor.canvasPath = rootPath + "/" + assetBrowser.options[assetBrowser.value].text + ".asset";
            nodeEditor.Start();
            nodeEditor.enabled = true;

            InitializeTags();
            InitializePatterns();
            InitializeMasteries();
            InitializePaces();
            InitializePointers();

            if(oldAssetValue != 0)
            {
                GameManager.instance.LoadSnapshot(0);
            }
        }

        canvasStart = false;
        assetBrowser.captionText.text = assetBrowser.options[assetBrowser.value].text;
        oldAssetValue = assetBrowser.value;
    }

    void Update()
    {
        if(!canvasStart)
        {
            canvasStart = true;
        }

        if (nodeEditor.canvas != null)
        {
            UpdateNodeEditorRect();
            NodeEditor.RecalculateAll(nodeEditor.canvas);
        }

        refreshButton.gameObject.SetActive(GameManager.instance.debugMode);
        assetBrowser.gameObject.SetActive(GameManager.instance.debugMode);
        validateButton.gameObject.SetActive(GameManager.instance.debugMode);
    }
    
    public void ValidateCanvas()
    {
        if(assetBrowser.value == 0)
        {
            return;
        }

        bool invalid = false;

        if(paceSpawnerNodes.Count == 0)
        {
            Debug.Log("Missing: Game Node > Spawner > Pace");
            invalid = true;
        }
        else if(paceNodes.Count == 0)
        {
            Debug.Log("Missing: Game Node > Pace");
            invalid = true;
        }

        if(patternSpawnerNodes.Count == 0)
        {
            Debug.Log("Missing: Game Node > Spawner > Challenge");
            invalid = true;
        }
        else if(patternNodes.Count == 0)
        {
            Debug.Log("Missing: Game Node > Challenge");
            invalid = true;
        }
        
        if(patternNodes.Count == 0)
        {
            Debug.Log("Missing: Game Node > Challenge");
            invalid = true;
        }

        if(tagNodes.Count == 0)
        {
            Debug.Log("Missing: Game Node > Mechanic");
            invalid = true;
        }

        if(!invalid)
        {
            Debug.Log("Everything looks fine.");
        }
    }

    public void UpdateNode(BaseNode node)
    {
        if(node is PatternNode)
        {
            UpdateNode((PatternNode)node);
            InsertOrUpdateNode(ref patternNodes, ref node);
        }
        else if(node is TagNode)
        {
            UpdateNode((TagNode)node);
            InsertOrUpdateNode(ref tagNodes, ref node);
        }
        else if(node is PaceNode)
        {
            UpdateNode((PaceNode)node);
            InsertOrUpdateNode(ref paceNodes, ref node);
        }
        else if(node is PatternSpawnerNode)
        {
            InsertOrUpdateNode(ref patternSpawnerNodes, ref node);
        }
        else if(node is PaceSpawnerNode)
        {
            InsertOrUpdateNode(ref paceSpawnerNodes, ref node);
        }
        else if(node is PointerInputNode)
        {
            InsertOrUpdateNode(ref pointerInputNodes, ref node);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void RemoveNode(BaseNode node)
    {
        if(node is PaceNode)
        {
            RemoveNode((PaceNode) node);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    #region Tags
    private void InitializeTags()
    {
        tags = new Dictionary<string, bool>();
        tagNodes = new List<BaseNode>();

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
        if (tags != null && node.tag != null)
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
        if(patterns != null && node.pattern != null)
        {
            patterns[node.pattern] = node.value;
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
            PatternNode currentPatternNode = null;
            foreach(PatternNode patternNode in patternNodes)
            {
                if(patternNode.patternIndex == patternSpawner.patternsIndices[i])
                {
                    currentPatternNode = patternNode;
                    break;
                }
            }

            if(currentPatternNode != null && currentPatternNode.value)
            {
                activePatternNodes.Add(currentPatternNode);
                weights.Add(patternSpawner.patternsSpawnWeights[i]);
                totalWeights += patternSpawner.patternsSpawnWeights[i];
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

        throw new NotImplementedException();
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
        float sumWeights = 0;
        float sumMasteries = 0;

        for(int i = 0; i < node.patternsIndices.Count; i++)
        {
            string patternName = PatternManager.instance.patternsName[node.patternsIndices[i]];
            string mastery = PatternManager.instance.patternsInfo[patternName].GetMastery();

            sumMasteries += Mastery.values.IndexOf(mastery) * node.patternsMasteryWeights[i];
            sumWeights += node.patternsMasteryWeights[i];
        }

        int masteryIndex = (int) Mathf.Round(sumMasteries / sumWeights);

        return MasteryComparison.Compare(MasteryComparison.values[masteryNode.masteryComparisonIndex], Mastery.values[masteryIndex], masteryNode.mastery);
    }

    private bool IsMasteryResolved(MasteryNode masteryNode, PaceSpawnerNode node)
    {
        float sumWeights = 0;
        float sumMasteries = 0;

        string[] paceNames = new string[paceNodes.Count];
        for(int p = 0; p < paceNodes.Count; p++)
        {
            paceNames[p] = ((PaceNode) paceNodes[p]).paceName;
        }

        for(int i = 0; i < node.pacesIndices.Count; i++)
        {
            string paceName = paceNames[node.pacesIndices[i]];
            string mastery = PaceManager.instance.pacesInfo[paceName].GetMastery();

            sumMasteries += Mastery.values.IndexOf(mastery) * node.pacesMasteryWeights[i];
            sumWeights += node.pacesMasteryWeights[i];
        }

        int masteryIndex = (int) Mathf.Round(sumMasteries / sumWeights);

        return MasteryComparison.Compare(MasteryComparison.values[masteryNode.masteryComparisonIndex], Mastery.values[masteryIndex], masteryNode.mastery);
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
        int index = -1;

        for(int i = 0; i < paceNodes.Count; i++)
        {
            if(paceNodes[i].creationId == node.creationId)
            {
                index = i;
                break;
            }
        }

        if(index == -1)
        {
            PaceManager.instance.SetPacesInfo(node.paceName, new PaceInfo(node.paceName, node.instancesCount)); 
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
                    paceSpawner.pacesSpawnWeights.RemoveAt(i);
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
                weights.Add(paceSpawner.pacesSpawnWeights[i]);
                totalWeights += paceSpawner.pacesSpawnWeights[i];
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
    
    private void InitializePointers()
    {
        pointerInputNodes = new List<BaseNode>();
    }

    public bool GetPointerValue(string pointerName)
    {
        foreach(PointerInputNode pointerInputNode in pointerInputNodes)
        {
            if(pointerInputNode.pointerName == pointerName)
            {
                return pointerInputNode.value;
            }
        }

        return false;
    }

    private enum Splitscreen
    {
        None,
        Vertical,
    }

    private Splitscreen splitscreen = Splitscreen.None;

    public void ToogleNodeEditor()
    {
        switch(splitscreen)
        {
            case Splitscreen.None:
                splitscreen = Splitscreen.Vertical;
                break;
            case Splitscreen.Vertical:
                splitscreen = Splitscreen.None;
                break;
        }
    }

    private void UpdateNodeEditorRect()
    {
        if (splitscreen == Splitscreen.None)
        {
            nodeEditor.specifiedRootRect.width = 0;
            nodeEditor.specifiedRootRect.height = 0;
            
            nodeEditor.specifiedRootRect.x = 0;
            nodeEditor.specifiedRootRect.y = 0;
        }
        else if(splitscreen == Splitscreen.Vertical)
        {
            nodeEditor.specifiedRootRect.width = Screen.width;
            nodeEditor.specifiedRootRect.height = Screen.height / 2;

            nodeEditor.specifiedRootRect.x = Screen.width - nodeEditor.specifiedRootRect.width;
            nodeEditor.specifiedRootRect.y = Screen.height - nodeEditor.specifiedRootRect.height;
        }

        nodeEditor.specifiedCanvasRect = nodeEditor.specifiedRootRect;
        nodeEditor.specifiedCanvasRect.y = 0;
    }

    private void InsertOrUpdateNode(ref List<BaseNode> nodeArray, ref BaseNode node)
    {
        int index = -1;

        for(int i = 0; i < nodeArray.Count; i++)
        {
            if(nodeArray[i].creationId == node.creationId)
            {
                index = i;
                break;
            }
        }

        if(index < 0)
        {
            nodeArray.Add(node);
            nodeArray.SortByCreation();
        }
        else
        {
            nodeArray[index] = node;
        }
    }

    bool isPausedAux = true;
    void OnApplicationFocus(bool focusStatus)
    {
        nodeEditor.enabled = focusStatus;

        if(focusStatus)
        {
            RefreshAssetBrowser();
            GameManager.instance.SetPause(isPausedAux);
        }
        else
        {
            isPausedAux = GameManager.instance.isPaused;
            GameManager.instance.SetPause(true);
        }
    }
}
