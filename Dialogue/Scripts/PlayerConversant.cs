using System.Collections.Generic;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue;
        private DialogueNode _currentNode;

        private void Start()
        {
            ToggleCallbackConnections(true);
            ActivateDialogue(dialogue);
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
                return;
            }
            DialogueEventManager.Instance.OnDialogueConditionNodeActivated -= PerformDialogueConditionCheck;
            DialogueEventManager.Instance.OnNextNodeButtonClicked -= MoveToNextNode;
            DialogueEventManager.Instance.OnNodeHasChoices -= OnNodeHasPlayerResponses;
            DialogueEventManager.Instance.OnPlayerDialogueChoiceSelected -= OnChoiceSelected;
        }

        public void ActivateDialogue(Dialogue dialogue)
        {
            DialogueEventManager.Instance.InvokeOnDialogueActivated();
            this.dialogue = dialogue;
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
            if (!HasNext())
            {
                return false;
            }
            
            if (currentNode is DialogueNode_Condition conditionNode)
            {
                return false;
            }

            var nodeChildren = new List<DialogueNode>(dialogue.GetAllChildren(_currentNode));

            if (nodeChildren[0] is DialogueNode_Text textNode)
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
            var children = new List<DialogueNode>(dialogue.GetAllChildren(_currentNode));
            SetCurrentNode(children[0]);
        }
        
        private void PerformDialogueConditionCheck(DialogueNode_Condition conditionNode)
        {
            var children = new List<DialogueNode>(dialogue.GetAllChildren(conditionNode));

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

        private void OnChoiceSelected(DialogueNode node)
        {
            var nextNode = dialogue.GetFirstChildNodeOf(node);
            SetCurrentNode(nextNode);
        }

        private void OnNodeHasPlayerResponses(DialogueNode node)
        {
            if (node.ChildNodes.Count < 1)
            {
                return;
            }
            
            foreach (var childNode in dialogue.GetAllChildren(node))
            {
                if (childNode is not DialogueNode_Text playerResponse) continue;
                if (!playerResponse.IsPlayerNode) continue;
                
                playerResponse.PerformNode();
            }
        }
    }
}
