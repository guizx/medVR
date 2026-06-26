using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System.Collections.Generic;

public class DialogueGraphEditor : EditorWindow {
    private DialogueGraphView _graphView;
    private DialogueContainer _currentAsset;

    [MenuItem("Graph/Dialogue Editor")]
    public static void Open() => GetWindow<DialogueGraphEditor>("Dialogue Graph");

    private void OnEnable() {
        ConstructGraph();
        GenerateToolbar();
    }

    private void ConstructGraph() {
        _graphView = new DialogueGraphView { name = "Dialogue Graph" };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar() {
        var toolbar = new Toolbar();
        toolbar.Add(new Button(() => SaveData()) { text = "Salvar" });
        toolbar.Add(new Button(() => OpenLoadPanel()) { text = "Carregar" });
        rootVisualElement.Add(toolbar);
    }

    private void OpenLoadPanel() {
        string path = EditorUtility.OpenFilePanel("Selecionar Diálogo", "Assets", "asset");
        if (string.IsNullOrEmpty(path)) return;
        path = "Assets" + path.Substring(Application.dataPath.Length);
        DialogueContainer container = AssetDatabase.LoadAssetAtPath<DialogueContainer>(path);
        if (container != null) {
            _currentAsset = container;
            LoadData(container);
        }
    }

    private void SaveData() {
        if (_currentAsset == null) {
            string path = EditorUtility.SaveFilePanelInProject("Salvar", "NovoDialogo", "asset", "");
            if (string.IsNullOrEmpty(path)) return;
            _currentAsset = ScriptableObject.CreateInstance<DialogueContainer>();
            AssetDatabase.CreateAsset(_currentAsset, path);
        }

        _currentAsset.DialogueNodeData.Clear();
        _currentAsset.NodeLinks.Clear();

        var nodes = _graphView.nodes.ToList().Cast<DialogueNode>();
        foreach (var node in nodes) {
            _currentAsset.DialogueNodeData.Add(new DialogueNodeData {
                Guid = node.GUID, SpeakerIndex = node.SpeakerIndex,
                DialogueText = node.DialogueText, Position = node.GetPosition().position
            });
        }

        foreach (var edge in _graphView.edges.ToList()) {
            var outNode = edge.output.node as DialogueNode;
            var inNode = edge.input.node as DialogueNode;
            var textField = edge.output.Query<TextField>().First();
            string choiceText = (textField != null) ? textField.value : "";

            _currentAsset.NodeLinks.Add(new NodeLinkData {
                BaseNodeGuid = outNode.GUID, PortName = choiceText, TargetNodeGuid = inNode.GUID
            });
        }
        EditorUtility.SetDirty(_currentAsset);
        AssetDatabase.SaveAssets();
    }

    private void LoadData(DialogueContainer container) {
        foreach (var node in _graphView.nodes.ToList()) _graphView.RemoveElement(node);
        foreach (var edge in _graphView.edges.ToList()) _graphView.RemoveElement(edge);
        
        _graphView.AvailableSpeakers = container.Speakers;
        var nodesMap = new Dictionary<string, DialogueNode>();

        foreach (var data in container.DialogueNodeData) {
            var node = _graphView.CreateNode(data.Position, data.SpeakerIndex);
            node.GUID = data.Guid;
            node.DialogueText = data.DialogueText;
            node.mainContainer.Query<TextField>().First().value = data.DialogueText;
            nodesMap.Add(data.Guid, node);
            
            // Limpa portas de saída automáticas para recriar as do save
            var outPorts = node.outputContainer.Query<Port>().ToList();
            outPorts.ForEach(p => node.outputContainer.Remove(p));
        }

        foreach (var link in container.NodeLinks) {
            var baseNode = nodesMap[link.BaseNodeGuid];
            var targetNode = nodesMap[link.TargetNodeGuid];
            
            if (string.IsNullOrEmpty(link.PortName)) {
                // Recria porta direta
                var port = baseNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
                port.portName = "Próximo (Direto)";
                baseNode.outputContainer.Insert(0, port);
                Connect(port, targetNode.inputContainer.Query<Port>().First());
            } else {
                // Recria porta de escolha
                _graphView.AddChoicePort(baseNode, link.PortName);
                Connect(baseNode.outputContainer.Query<Port>().ToList().Last(), targetNode.inputContainer.Query<Port>().First());
            }
        }
    }

    private void Connect(Port outP, Port inP) {
        var edge = new Edge { output = outP, input = inP };
        edge.input.Connect(edge); edge.output.Connect(edge);
        _graphView.AddElement(edge);
    }
}