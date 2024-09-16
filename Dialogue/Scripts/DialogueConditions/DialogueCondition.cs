using TPlus.Dialogue;
using UnityEngine;
public abstract class DialogueCondition : ScriptableObject
{
    public abstract bool HasPassedCheck(PlayerConversant player);
}
