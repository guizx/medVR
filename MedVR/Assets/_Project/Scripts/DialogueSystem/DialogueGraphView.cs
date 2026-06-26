using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView {
    public List<SpeakerData> AvailableSpeakers = new List<SpeakerData>();

    public DialogueGraphView() {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        this.AddManipulator(new ContextualMenuManipulator(menuEvent => 
            menuEvent.menu.AppendAction("Criar Nó", actionEvent => 
                CreateNode(actionEvent.eventInfo.localMousePosition))));
    }

    public DialogueNode CreateNode(Vector2 position, int defaultSpeakerIndex = 0) {
        var node = new DialogueNode {
            title = "Fala",
            GUID = System.Guid.NewGuid().ToString(),
            SpeakerIndex = defaultSpeakerIndex
        };

        // ENTRADA (Múltipla: várias escolhas podem levar aqui)
        var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        inputPort.portName = "Entrada";
        node.inputContainer.Add(inputPort);

        // SAÍDA DIRETTA (Sem texto, para fluxo contínuo)
        var defaultOutput = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        defaultOutput.portName = "Próximo (Direto)";
        node.outputContainer.Add(defaultOutput);

        UpdateSpeakerPopup(node);

        var dialogueField = new TextField("Texto:");
        dialogueField.multiline = true;
        dialogueField.RegisterValueChangedCallback(evt => node.DialogueText = evt.newValue);
        node.mainContainer.Add(dialogueField);

        var button = new Button(() => AddChoicePort(node)) { text = "Adicionar Escolha" };
        node.titleContainer.Add(button);

        node.SetPosition(new Rect(position, new Vector2(250, 150)));
        node.RefreshExpandedState();
        node.RefreshPorts();
        AddElement(node);
        return node;
    }

    public void UpdateSpeakerPopup(DialogueNode node) {
        if (node.SpeakerPopup != null) node.mainContainer.Remove(node.SpeakerPopup);

        List<string> speakerNames = AvailableSpeakers.Count > 0 
            ? AvailableSpeakers.Select(s => s != null ? s.Name : "Nulo").ToList() 
            : new List<string> { "Nenhum Autor" };

        node.SpeakerPopup = new PopupField<string>("Quem fala:", speakerNames, node.SpeakerIndex);
        node.SpeakerPopup.RegisterValueChangedCallback(evt => node.SpeakerIndex = node.SpeakerPopup.index);
        node.mainContainer.Insert(0, node.SpeakerPopup);
    }

    public void AddChoicePort(DialogueNode node, string overridePortName = "") {
        var port = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        var textField = new TextField { value = string.IsNullOrEmpty(overridePortName) ? "Texto da Escolha" : overridePortName };
        textField.style.flexGrow = 1;
        port.contentContainer.Add(textField);
        port.portName = "";

        var deleteButton = new Button(() => {
            if (port.connected) DeleteElements(port.connections);
            node.outputContainer.Remove(port);
            node.RefreshPorts();
        }) { text = "X" };
        port.contentContainer.Add(deleteButton);

        node.outputContainer.Add(port);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        return ports.ToList().Where(p => p.node != startPort.node && p.direction != startPort.direction).ToList();
    }
}