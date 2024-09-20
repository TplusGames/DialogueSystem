using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class DC_HasNotCompletedDialogue : DialogueCondition
    {
        public override bool HasPassedCheck(PlayerConversant player)
        {
            return !BlackboardManager.Instance.PlayerHasCompletedDialogue(HashString.GetHashCode(affectedDialogue.name));
        }
    }
}

