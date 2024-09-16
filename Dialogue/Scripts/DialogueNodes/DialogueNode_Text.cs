using System.Linq;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class DialogueNode_Text : DialogueNode
    {
        public string Text;
        public bool IsPlayerNode;
        public bool CanSay { get; private set; }
        
        public override void PerformNode()
        {
            Debug.Log("Performing text node");
            DialogueEventManager.Instance.InvokeOnDialogueTextNodeActivated(this);

            if (ChildNodes.Any())
            {
                DialogueEventManager.Instance.InvokeOnNodeHasChoices(this);
            }
        }

        public void SetCanSay(bool canSay)
        {
            CanSay = canSay;
        }
    }
}

