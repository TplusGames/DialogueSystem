using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace TPlus.Dialogue
{
    public class Dialogue : DialogueNode_Text
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodeDictionary = new Dictionary<string, DialogueNode>();
        public float EditorZoomAmount { get; set; } = 1f;
        public Vector2 EditorScrollPosition { get; set; }

        private void OnEnable()
        {
            UpdateDictionary();
        }

        protected void UpdateDictionary()
        {
            nodeDictionary.Clear();
            foreach (DialogueNode node in nodes)
            {
                if (string.IsNullOrEmpty(node.UniqueID))
                {
                    Debug.LogError("Node has no ID!!");
                    continue;
                }

                if (nodeDictionary.ContainsKey(node.UniqueID))
                {
                    Debug.LogError("Node dictionary already containes a node with key " + node.UniqueID + "!!");
                    continue;
                }
                nodeDictionary[node.UniqueID] = node;
            }
        }


        public virtual DialogueNode CreateRootNode()
        {
            nodes.Clear();
            var rootNode = CreateNewTextNode(); 
            return rootNode;
        }

        public DialogueNode CreateNewTextNode()
        {
            DialogueNode_Text node = CreateInstance(nameof(DialogueNode_Text)) as DialogueNode_Text;
            node.ParentDialogue = this;
            InitializeNodeAsset(node);
            IsPlayerNode = true;
            return node;
        }

        public DialogueNode_Condition CreateNewConditionNode()
        {
            DialogueNode_Condition node = CreateInstance(nameof(DialogueNode_Condition)) as DialogueNode_Condition;
            node.Dialogue = this;
            InitializeNodeAsset(node);
            return node;
        }

        private void InitializeNodeAsset(DialogueNode node)
        {
            var randomID = Guid.NewGuid().ToString();
            node.UniqueID = randomID;
            node.name = randomID;
            nodes.Add(node);
            UpdateDictionary();
        }

        public void DeleteNode(DialogueNode node)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (node.UniqueID == nodes[i].UniqueID)
                {
                    continue;
                }

                if (nodes[i].ChildNodes.Contains(node.UniqueID))
                {
                    nodes[i].RemoveChildNode(node.UniqueID);
                }
            }
            nodes.Remove(node);
            UpdateDictionary();
        }

        public DialogueNode CreateChildTextNode(DialogueNode parent)
        {
            var childNode = CreateNewTextNode();
            parent.AddChildNode(childNode.name);
            var nodeWidth = (parent.Transform.xMax - parent.Transform.center.x) * 2;
            var childPosition = parent.Transform.position + new Vector2(nodeWidth * 1.5f, 0);
            childNode.Transform.position = childPosition;
            UpdateDictionary();
            return childNode;
        }

        public void LinkNode(DialogueNode parent, string child)
        {
            parent.AddChildNode(child);
        }

        public void UnlinkNode(DialogueNode parent, string child)
        {
            parent.RemoveChildNode(child);
        }

        public IEnumerable<DialogueNode> GetDialogueNodes() { return nodes; }

        public DialogueNode GetRootNode() { return nodes[0]; }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode node)
        {
            var children = new List<DialogueNode>();

            if (node.ChildNodes == null)
            {
                return children;
            }

            foreach (var child in node.ChildNodes)
            {
                if (nodeDictionary.ContainsKey(child))
                {
                    children.Add(nodeDictionary[child]);
                }
            }

            return children;
        }

        public DialogueNode GetFirstChildNodeOf(DialogueNode node)
        {
            DialogueNode firstChild = null;

            if (node.ChildNodes.Any())
            {
                firstChild = nodeDictionary[node.ChildNodes[0]];
            }

            return firstChild;
        }

        public static Dialogue CreateNewDialogue(string name)
        { 
            var newDialogue = CreateInstance<Dialogue>();
            newDialogue.UniqueID = name;
            return newDialogue;
        }

        public override void PerformNode()
        {
            base.PerformNode();
            DialogueEventManager.Instance.InvokeOnDialogueActivated(this);
        }
    }
}

