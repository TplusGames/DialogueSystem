using TPlus.Dialogue;
using UnityEngine;

public class RPSInfo
{
    public ERPSChoice PlayerChoice;
    public ERPSChoice NPCChoice;
}

public enum ERPSChoice
{
    Rock,
    Paper,
    Scissors
}

[CreateAssetMenu(menuName = "Dialogue/Condition/New Rock Paper Scissors Condition")]
public class DialogueCondition_RockPaperScissors : DialogueCondition
{
    public override bool HasPassedCheck(PlayerConversant player)
    {
        throw new System.NotImplementedException();
    }
}
