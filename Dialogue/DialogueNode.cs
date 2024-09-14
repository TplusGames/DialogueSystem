
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TPlus.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        public string UniqueID;
        public string DialogueText;
        public List<string> ChildNodes = new List<string>();
        public List<DialogueCondition> Conditions = new List<DialogueCondition>();

        public bool IsPlayerNode;

        public Rect Transform = new Rect(0, 0, 300, 250);

        public void AddChildNode(string name)
        {
            if (!ChildNodes.Contains(name))
            {
                ChildNodes.Add(name);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void RemoveChildNode(string name)
        {
            if (ChildNodes.Contains(name))
            {
                ChildNodes.Remove(name);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}

