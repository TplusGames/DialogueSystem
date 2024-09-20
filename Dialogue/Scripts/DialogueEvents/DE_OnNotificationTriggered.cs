using UnityEngine;

namespace TPlus.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Events/New Dialogue Notification Event")]
    public class DE_OnNotificationTriggered : DialogueEvent
    {
        public string text;
        public override void BroadcastEvent(object obj)
        {
            MasterEventManager.Instance.InvokeOnNotificationTriggered(text);
        }
    }
}

