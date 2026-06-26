using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueNode : Node {
    public string GUID;
    public int SpeakerIndex;
    public string DialogueText;
    public PopupField<string> SpeakerPopup;
}

[Serializable]
public class DialogueNodeData {
    public string Guid;
    public int SpeakerIndex;
    public string DialogueText;
    public Vector2 Position;
}

[Serializable]
public class NodeLinkData {
    public string BaseNodeGuid;
    public string PortName; // Se vazio, é um link direto
    public string TargetNodeGuid;
}

public class DialogueContainer : ScriptableObject {
    public List<SpeakerData> Speakers = new List<SpeakerData>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
}
