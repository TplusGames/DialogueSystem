using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class DialogueHolder : Dialogue
    {
        public static DialogueHolder CreateNewDialogueHolder(string name)
        {
            var newDialogue = CreateInstance<DialogueHolder>();
            newDialogue.CreateRootNode();
            return newDialogue;
        }
    }
}
