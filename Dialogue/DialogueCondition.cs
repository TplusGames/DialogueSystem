using UnityEngine;
public abstract class DialogueCondition : ScriptableObject
{
    public abstract bool HasPassedCheck<T>(T info) where T : class;
}
