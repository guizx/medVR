using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DialogueDebugPlayer : MonoBehaviour {
    public DialogueContainer dialogueData;
    private DialogueNodeData _currentNode;
    private bool _waitingForChoice = false;

    void Start() {
        if (dialogueData == null || dialogueData.DialogueNodeData.Count == 0) return;
        var allTargets = dialogueData.NodeLinks.Select(l => l.TargetNodeGuid).ToList();
        _currentNode = dialogueData.DialogueNodeData.FirstOrDefault(n => !allTargets.Contains(n.Guid)) ?? dialogueData.DialogueNodeData[0];
        DisplayNode();
    }

    void Update() {
        if (_currentNode == null) return;
        if (!_waitingForChoice && Input.GetKeyDown(KeyCode.Space)) Advance(null);
        else if (_waitingForChoice) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectChoice(2);
        }
    }

    void DisplayNode() {
        var speaker = (_currentNode.SpeakerIndex < dialogueData.Speakers.Count) ? dialogueData.Speakers[_currentNode.SpeakerIndex] : null;
        string name = speaker ? speaker.Name : "???";
        string col = speaker ? ColorUtility.ToHtmlStringRGB(speaker.SpeakerColor) : "FFFFFF";
        
        Debug.Log($"<color=#{col}><b>[{name}]</b></color>: {_currentNode.DialogueText}");

        var choices = dialogueData.NodeLinks.Where(l => l.BaseNodeGuid == _currentNode.Guid && !string.IsNullOrEmpty(l.PortName)).ToList();
        if (choices.Count > 0) {
            _waitingForChoice = true;
            string t = "Escolhas: ";
            for (int i = 0; i < choices.Count; i++) t += $"[{i+1}] {choices[i].PortName} ";
            Debug.Log(t);
        } else _waitingForChoice = false;
    }

    void SelectChoice(int i) {
        var choices = dialogueData.NodeLinks.Where(l => l.BaseNodeGuid == _currentNode.Guid && !string.IsNullOrEmpty(l.PortName)).ToList();
        if (i < choices.Count) Advance(choices[i]);
    }

    void Advance(NodeLinkData link) {
        if (link == null) link = dialogueData.NodeLinks.FirstOrDefault(l => l.BaseNodeGuid == _currentNode.Guid && string.IsNullOrEmpty(l.PortName));
        if (link != null) {
            _currentNode = dialogueData.DialogueNodeData.FirstOrDefault(n => n.Guid == link.TargetNodeGuid);
            DisplayNode();
        } else {
            Debug.Log("Fim.");
            _currentNode = null;
        }
    }
}