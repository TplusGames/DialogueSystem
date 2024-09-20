using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class NPCConversant : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<Dialogue> oneOffDialogues;
        [SerializeField] private DialogueHolder standardDialogueHolder;
        [SerializeField] private string interactionText;
        
        public void Interact(MonoBehaviour trigger)
        {
            foreach (var dialogue in oneOffDialogues.Where(dialogue =>
                         !BlackboardManager.Instance.PlayerHasCompletedDialogue(HashString.GetHashCode(dialogue.name))))
            {
                trigger.GetComponent<PlayerConversant>().ActivateDialogue(dialogue);
                return;
            }

            if (standardDialogueHolder == null) return;
            trigger.GetComponent<PlayerConversant>().ActivateDialogue(standardDialogueHolder);
        }

        public void OpenInteractionPrompt()
        {
            MasterEventManager.Instance.InvokeOnInteractionFound(this);
        }

        public void CloseInteractionPrompt()
        {
            MasterEventManager.Instance.InvokeOnInteractionLost();
        }

        public string GetInteractionText()
        {
            return interactionText;
        }
    }
}

