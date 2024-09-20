using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TPlus.Dialogue.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueUI;
        [SerializeField] private Button nextNodeButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private TextMeshProUGUI aiText;
        [SerializeField] private Transform playerResponseHolder;
        [SerializeField] private Transform responseUI;
        [SerializeField] private Transform buttonHolder;
        [SerializeField] private GameObject playerResponseButtonPrefab;
        [SerializeField] private GameObject playerDialogueSelectButtonPrefab;

        private List<PlayerChoiceButton> playerResponseButtons = new List<PlayerChoiceButton>();
        private List<PlayerDialogueTreeSelectButton> playerDialogueSelectButtons = new List<PlayerDialogueTreeSelectButton>();
        
        private void Start()
        {
            ToggleCallbackConnections(true);
            nextNodeButton.onClick.AddListener(OnNextNodeButtonClicked);
            exitButton.onClick.AddListener(OnCloseButtonClicked);
            ToggleNextButtonActive(false);
            ToggleResponseUI(false);
            ToggleDialogueUI(false);
        }

        private void OnDisable()
        {
            ToggleCallbackConnections(false);
        }

        private void ToggleCallbackConnections(bool isActive)
        {
            if (isActive)
            {
                DialogueEventManager.Instance.OnDialogueTextNodeActivated += DisplayText;
                DialogueEventManager.Instance.OnNextNodeButtonToggled += ToggleNextButtonActive;
                DialogueEventManager.Instance.OnDialogueActivated += OnDialogueActivated;
                DialogueEventManager.Instance.OnDialogueUIClosed += OnDialogueClosed;
                return;
            }
            DialogueEventManager.Instance.OnDialogueTextNodeActivated -= DisplayText;
            DialogueEventManager.Instance.OnNextNodeButtonToggled -= ToggleNextButtonActive;
            DialogueEventManager.Instance.OnDialogueActivated -= OnDialogueActivated;
            DialogueEventManager.Instance.OnDialogueUIClosed -= OnDialogueClosed;
        }

        private void OnDialogueActivated(Dialogue dialogue)
        {
            ToggleDialogueUI(true);
        }

        private void OnDialogueClosed()
        {
            ToggleDialogueUI(false);
        }

        private void ToggleDialogueUI(bool isActive)
        {
            dialogueUI.SetActive(isActive);
        }

        private void ToggleResponseUI(bool isActive)
        {
            if (isActive)
            {
                buttonHolder.gameObject.SetActive(false);
                responseUI.gameObject.SetActive(true);
                return;
            }
            buttonHolder.gameObject.SetActive(true);
            responseUI.gameObject.SetActive(false);
        }

        private void DisplayText(DialogueNode_Text textNode)
        {
            ToggleResponseUI(false);
            if (textNode.IsPlayerNode)
            {
                ActivatePlayerTextNode(textNode);
            }
            else
            {
                ActivateNPCTextNode(textNode);
            }
        }

        private void ClearPlayerResponses()
        {
            for (int i = playerResponseButtons.Count - 1; i >= 0; i--)
            {
                Destroy(playerResponseButtons[i].gameObject);
            }
            playerResponseButtons.Clear();

            for (int i = playerDialogueSelectButtons.Count - 1; i >= 0; i--)
            {
                Destroy(playerDialogueSelectButtons[i].gameObject);
            }
            playerDialogueSelectButtons.Clear();
        }

        private void ActivateNPCTextNode(DialogueNode_Text textNode)
        {
            ClearPlayerResponses();
            aiText.text = textNode.Text;
        }

        private void ActivatePlayerTextNode(DialogueNode_Text textNode)
        {
            ToggleResponseUI(true);
            if (textNode is Dialogue dialogue)
            {
                var newDialogueButton = Instantiate(playerDialogueSelectButtonPrefab, playerResponseHolder).GetComponent<PlayerDialogueTreeSelectButton>();
                newDialogueButton.SetDialogue(dialogue);
                playerDialogueSelectButtons.Add(newDialogueButton);
                return;
            }
            var newChoiceButton = Instantiate(playerResponseButtonPrefab, playerResponseHolder).GetComponent<PlayerChoiceButton>();
            newChoiceButton.SetNode(textNode);
            playerResponseButtons.Add(newChoiceButton);
        }

        private void ToggleNextButtonActive(bool isActive)
        {
            nextNodeButton.gameObject.SetActive(isActive);
        }

        private void OnNextNodeButtonClicked()
        {
            DialogueEventManager.Instance.InvokeOnNextNodeButtonClicked();
        }

        private void OnCloseButtonClicked()
        {
            DialogueEventManager.Instance.InvokeOnDialogueUIClosed();
            MasterEventManager.Instance.InvokeOnGameStateChangeTriggered(EGameState.Standard);
        }
    }
}

