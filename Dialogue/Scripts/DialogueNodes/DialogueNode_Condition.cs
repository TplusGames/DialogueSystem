using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPlus.Dialogue
{
     public class DialogueNode_Condition : DialogueNode
    {
        public Dialogue Dialogue;
        public List<DialogueCondition> Conditions = new List<DialogueCondition>();
        
        public override void PerformNode()
        {
            DialogueEventManager.Instance.InvokeOnDialogueConditionNodeActivated(this);
        }

        public bool HasPassedCheck(PlayerConversant player)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                var condition = Conditions[i];

                if (!condition.HasPassedCheck(player))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

