using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.IO;
using System.Linq;

namespace TPlus.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodeDictionary = new Dictionary<string, DialogueNode>();

        public float EditorZoomAmount = 1f;
        public Vector2 EditorScrollPosition;

        private void OnEnable()
        {
            UpdateDictionary();
        }

        private void UpdateDictionary()
        {
            nodeDictionary.Clear();
            foreach (DialogueNode node in nodes)
            {
                if (String.IsNullOrEmpty(node.UniqueID))
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


        public void CreateRootNode()
        {
            var rootNode = CreateNewTextNode();
        }

        public DialogueNode CreateNewTextNode()
        {
            var randomID = Guid.NewGuid().ToString();
            DialogueNode_Text node = CreateInstance(nameof(DialogueNode_Text)) as DialogueNode_Text;
            InitializeNodeAsset(node);
            return node;
        }

        public DialogueNode_Condition CreateNewConditionNode()
        {
            DialogueNode_Condition node = CreateInstance(nameof(DialogueNode_Condition)) as DialogueNode_Condition;
            InitializeNodeAsset(node);
            return node;
        }

        private void InitializeNodeAsset(DialogueNode node)
        {
            var randomID = Guid.NewGuid().ToString();
            node.UniqueID = randomID;
            node.name = randomID;
            nodes.Add(node);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            UpdateDictionary();
            Undo.RegisterCreatedObjectUndo(node, "create node");
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
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.DeleteAsset($"Assets/Dialogue/{node.name}.asset");
            AssetDatabase.SaveAssets();
            UpdateDictionary();
        }

        public void CreateChildTextNode(DialogueNode parent)
        {
            var childNode = CreateNewTextNode();
            parent.AddChildNode(childNode.name);
            var nodeWidth = (parent.Transform.xMax - parent.Transform.center.x) * 2;
            var childPosition = parent.Transform.position + new Vector2(nodeWidth * 1.5f, 0);
            childNode.Transform.position = childPosition;
            UpdateDictionary();
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
            if (File.Exists($"Assets/Dialogue/Dialogues/{name}.asset"))
            {
                Debug.LogWarning($"Asset with name '{name}' already exists!");
                return null;
            }

            var newDialogue = CreateInstance<Dialogue>();
            AssetDatabase.CreateAsset(newDialogue, $"Assets/Dialogue/Dialogues/{name}.asset");
            newDialogue.CreateRootNode();
            return newDialogue;
        }
    }
}

