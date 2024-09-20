using TPlus.Dialogue;
using UnityEngine;
public abstract class DialogueCondition : ScriptableObject
{
    public Dialogue affectedDialogue;
    public abstract bool HasPassedCheck(PlayerConversant player);
}
