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

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Condition")]
public class DialogueCondition_RockPaperScissors : DialogueCondition
{
    public override bool HasPassedCheck<T>(T info)
    {
        return HasWon(info as RPSInfo);
    }

    private bool HasWon(RPSInfo info)
    {
        switch(info.PlayerChoice)
        {
            case ERPSChoice.Rock:
                if (info.NPCChoice == ERPSChoice.Paper || info.NPCChoice == ERPSChoice.Rock)
                {
                    return false;
                }
                if (info.NPCChoice == ERPSChoice.Scissors)
                {
                    return true;
                }
                break;
            case ERPSChoice.Paper:
                if (info.NPCChoice == ERPSChoice.Paper || info.NPCChoice == ERPSChoice.Scissors)
                {
                    return false;
                }
                if (info.NPCChoice == ERPSChoice.Rock)
                {
                    return true;
                }
                break;
            case ERPSChoice.Scissors:
                if (info.NPCChoice == ERPSChoice.Rock || info.NPCChoice == ERPSChoice.Scissors)
                {
                    return false;
                }
                if (info.NPCChoice == ERPSChoice.Paper)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}
