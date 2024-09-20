using UnityEngine;

namespace TPlus.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Conditions/New Player Dialogue Completed event")]
    public class DE_PlayerCompletedDialogue : DialogueEvent
    {
        public override void BroadcastEvent(object obj)
        {
            var dialogue = obj as Dialogue;
            BlackboardItem newItem = new BlackboardItem
            {
                Key = new HashString()
            };
            newItem.Key.SetHashStringText(dialogue.name);
            MasterEventManager.Instance.InvokeOnPlayerCompletedDialogueBlackboardUpdated(newItem);
        }
    }
}

