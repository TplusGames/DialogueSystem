using UnityEngine;

namespace TPlus.Dialogue
{
    public class DialogueEventManager : MonoBehaviour
    {
        public static DialogueEventManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            Debug.LogError($"Duplicate Dialogue Event Manager found on {gameObject.name}!! Destroying duplicate");
            Destroy(gameObject);
        }

        public delegate void DialogueActivated(Dialogue dialogue);
        public event DialogueActivated OnDialogueActivated;
        public void InvokeOnDialogueActivated(Dialogue dialogue)
        {
            OnDialogueActivated?.Invoke(dialogue);
        }

        public delegate void DialogueSelected(Dialogue dialogue);
        public event DialogueSelected OnDialogueSelected;
        public void InvokeOnDialogueSelected(Dialogue dialogue)
        {
            OnDialogueSelected?.Invoke(dialogue);
        }

        public delegate void DialogueUIClosed();
        public event DialogueUIClosed OnDialogueUIClosed;
        public void InvokeOnDialogueUIClosed()
        {
            OnDialogueUIClosed?.Invoke();
        }

        public delegate void DialogueTextNodeActivated(DialogueNode_Text textNode);
        public event DialogueTextNodeActivated OnDialogueTextNodeActivated;
        public void InvokeOnDialogueTextNodeActivated(DialogueNode_Text textNode)
        {
            OnDialogueTextNodeActivated?.Invoke(textNode);
        }

        public delegate void DialogueConditionNodeActivated(DialogueNode_Condition conditionNode);
        public event DialogueConditionNodeActivated OnDialogueConditionNodeActivated;
        public void InvokeOnDialogueConditionNodeActivated(DialogueNode_Condition conditionNode)
        {
            OnDialogueConditionNodeActivated?.Invoke(conditionNode);
        }

        public delegate void DialogueConditionCheckPerformed(PlayerConversant playerConversant);
        public event DialogueConditionCheckPerformed OnDialogueConditionCheckPerformed;
        public void InvokeOnDialogueConditionCheckPerformed(PlayerConversant playerConversant)
        {
            OnDialogueConditionCheckPerformed?.Invoke(playerConversant);
        }

        public delegate void NextNodeButtonToggled(bool isActive);
        public event NextNodeButtonToggled OnNextNodeButtonToggled;
        public void InvokeOnNextNodeButtonToggled(bool isActive)
        {
            OnNextNodeButtonToggled?.Invoke(isActive);
        }

        public delegate void NextNodeButtonClicked();
        public event NextNodeButtonClicked OnNextNodeButtonClicked;
        public void InvokeOnNextNodeButtonClicked()
        {
            OnNextNodeButtonClicked?.Invoke();
        }

        public delegate void PlayerDialogueChoiceSelected(DialogueNode_Text node);
        public event PlayerDialogueChoiceSelected OnPlayerDialogueChoiceSelected;
        public void InvokeOnPlayerDialogueChoiceSelected(DialogueNode_Text node)
        {
            OnPlayerDialogueChoiceSelected?.Invoke(node);
        }

        public delegate void NodeHasChoices(DialogueNode node);
        public event NodeHasChoices OnNodeHasChoices;
        public void InvokeOnNodeHasChoices(DialogueNode node)
        {
            OnNodeHasChoices?.Invoke(node);
        }
    }
}

