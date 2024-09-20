using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        private DialogueHolder _currentHolder;
        private Dialogue _currentDialogue;
        private DialogueNode _currentNode;

        private void Start()
        {
            ToggleCallbackConnections(true);
        }

        private void OnDisable()
        {
            ToggleCallbackConnections(false);
        }

        private void ToggleCallbackConnections(bool isActive)
        {
            if (isActive)
            {
                DialogueEventManager.Instance.OnDialogueConditionNodeActivated += PerformDialogueConditionCheck;
                DialogueEventManager.Instance.OnNextNodeButtonClicked += MoveToNextNode;
                DialogueEventManager.Instance.OnNodeHasChoices += OnNodeHasPlayerResponses;
                DialogueEventManager.Instance.OnPlayerDialogueChoiceSelected += OnChoiceSelected;
                DialogueEventManager.Instance.OnDialogueSelected += OpenDialogue;
                DialogueEventManager.Instance.OnDialogueUIClosed += CloseDialogue;
                return;
            }
            DialogueEventManager.Instance.OnDialogueConditionNodeActivated -= PerformDialogueConditionCheck;
            DialogueEventManager.Instance.OnNextNodeButtonClicked -= MoveToNextNode;
            DialogueEventManager.Instance.OnNodeHasChoices -= OnNodeHasPlayerResponses;
            DialogueEventManager.Instance.OnPlayerDialogueChoiceSelected -= OnChoiceSelected;
            DialogueEventManager.Instance.OnDialogueSelected -= OpenDialogue;
            DialogueEventManager.Instance.OnDialogueUIClosed -= CloseDialogue;
        }

        public void ActivateDialogue(Dialogue dialogue)
        {
            DialogueEventManager.Instance.InvokeOnDialogueActivated(dialogue);
            MasterEventManager.Instance.InvokeOnGameStateChangeTriggered(EGameState.Dialogue);

            if (dialogue is DialogueHolder holder)
            {
                OpenDialogueHolder(holder);
                return;
            }
            
            OpenDialogue(dialogue);
        }

        private void OpenDialogueHolder(DialogueHolder holder)
        {
            _currentHolder = holder;
            OpenDialogue(holder);
        }

        private void OpenDialogue(Dialogue dialogue)
        {
            _currentDialogue = dialogue;
            SetCurrentNode(dialogue.GetRootNode());
        }

        private void SetCurrentNode(DialogueNode currentNode)
        {
            _currentNode = currentNode;
            _currentNode.PerformNode();

            DialogueEventManager.Instance.InvokeOnNextNodeButtonToggled(IsNextNodeButtonNeeded(currentNode));
        }

        private bool IsNextNodeButtonNeeded(DialogueNode currentNode)
        {
            if (!HasNext() && _currentHolder == null)
            {
                return false;
            }
            
            if (currentNode is DialogueNode_Condition conditionNode)
            {
                return false;
            }

            var nodeChildren = new List<DialogueNode>(_currentDialogue.GetAllChildren(_currentNode));

            if (nodeChildren.Any() && nodeChildren[0] is DialogueNode_Text textNode)
            {
                if (textNode.IsPlayerNode)
                {
                    return false;
                }
            }

            return true;
        }

        private void MoveToNextNode()
        {
            var children = new List<DialogueNode>(_currentDialogue.GetAllChildren(_currentNode));

            if (!children.Any())
            {
                OnDialogueEnded();
                return;
            }
            
            SetCurrentNode(children[0]);
        }
        
        private void PerformDialogueConditionCheck(DialogueNode_Condition conditionNode)
        {
            var children = new List<DialogueNode>(conditionNode.Dialogue.GetAllChildren(conditionNode));

            DialogueNode_Text textNode = null;
            
            if (conditionNode.Conditions.Count <= 0)
            {
                textNode = children[0] as DialogueNode_Text;
                SetCurrentNode(textNode);
                return;
            }
            
            if (conditionNode.HasPassedCheck(this))
            {
                textNode = children[0] as DialogueNode_Text;
                SetCurrentNode(textNode);
                return;
            }

            if (conditionNode.ChildNodes.Count < 2) return;
            textNode = children[1] as DialogueNode_Text;
            SetCurrentNode(textNode);
        }

        public bool HasNext()
        {
            if (_currentNode.ChildNodes.Count <= 0)
            {
                return false;
            }

            return true;
        }

        private void OnChoiceSelected(DialogueNode_Text node)
        {
            if (node == null) Debug.Log("node is missing");
            if (_currentDialogue == null) Debug.Log("Dialogue is missing");

            if (_currentDialogue.GetFirstChildNodeOf(node) == null)
            {
                OnDialogueEnded();
                node.BroadcastEvents();
                return;
            }
            node.BroadcastEvents(); 
            SetCurrentNode(_currentDialogue.GetFirstChildNodeOf(node));
        }

        private void OnNodeHasPlayerResponses(DialogueNode node)
        {
            if (node.ChildNodes.Count < 1)
            {
                return;
            }
            
            foreach (var childNode in _currentDialogue.GetAllChildren(node))
            {
                if (childNode is DialogueNode_Text playerResponse)
                {
                    if (!playerResponse.IsPlayerNode) return;
                    playerResponse.PerformNode();
                }
                else if (childNode is DialogueNode_Condition conditionNode)
                {
                    childNode.PerformNode();
                }
            }
        }

        private void MoveToNextDialogue()
        {
            var nextDialogueNode = _currentHolder.GetFirstChildNodeOf(_currentDialogue);

            if (nextDialogueNode is Dialogue dialogue)
            {
                OpenDialogue(dialogue);
                return;
            }
            
            OpenDialogue(_currentHolder);
        }

        private void OnDialogueEnded()
        {
            if (_currentHolder == null)
            {
                return;
            }
            if (_currentDialogue.ChildNodes.Count > 0)
            {
                MoveToNextDialogue();
                return;
            }
            CloseDialogue();
        }

        private void CloseDialogue()
        {
            MasterEventManager.Instance.InvokeOnGameStateChangeTriggered(EGameState.Standard);
            _currentDialogue = null;
            _currentHolder = null;
            _currentNode = null;
        }
    }
}
