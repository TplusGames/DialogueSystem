using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class DialogueNode_Text : DialogueNode
    {
        public Dialogue ParentDialogue;
        public string Text;
        public bool IsPlayerNode;
        public bool CanSay { get; private set; }

        public List<DialogueEvent> Events = new List<DialogueEvent>();
        
        public override void PerformNode()
        {
            DialogueEventManager.Instance.InvokeOnDialogueTextNodeActivated(this);

            if (ChildNodes.Any())
            {
                DialogueEventManager.Instance.InvokeOnNodeHasChoices(this);
            }

            if (IsPlayerNode) return;
            BroadcastEvents();
        }

        public void BroadcastEvents()
        {
            if (!Events.Any()) return;

            foreach (var dialogueEvent in Events)
            {
                dialogueEvent.BroadcastEvent(ParentDialogue);
            }
        }

        public void SetCanSay(bool canSay)
        {
            CanSay = canSay;
        }
    }
}

