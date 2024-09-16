using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TPlus.Dialogue.UI
{
    public class PlayerChoiceButton : MonoBehaviour
    {
        private Button _button;
        private DialogueNode_Text _node;
        [SerializeField] private TextMeshProUGUI choiceText;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SelectChoice);
        }
        public void SetNode(DialogueNode_Text node)
        {
            _node = node;
            choiceText.text = node.Text;
        }

        private void SelectChoice()
        {
            DialogueEventManager.Instance.InvokeOnPlayerDialogueChoiceSelected(_node);
        }
    }
}

