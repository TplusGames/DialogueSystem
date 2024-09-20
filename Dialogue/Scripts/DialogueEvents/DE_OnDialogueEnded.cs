using UnityEngine;

namespace TPlus.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Events/New Dialogue Ended Event", fileName = "DE_DialogueEnded")]
    public class DE_OnDialogueEnded : DialogueEvent
    {
        public override void BroadcastEvent(object obj)
        {
            DialogueEventManager.Instance.InvokeOnDialogueUIClosed();
        }
    }
}

