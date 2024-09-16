using UnityEngine;

namespace TPlus.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/Condition/New Random Choice Condition")]
    public class DC_RandomChoice : DialogueCondition
    {
        public override bool HasPassedCheck(PlayerConversant player)
        {
            var randomInt = Random.Range(1, 3);

            Debug.Log("Random int = " + randomInt);
            
            if (randomInt == 1)
            {
                return false;
            }

            return true;
        }
    }
}

