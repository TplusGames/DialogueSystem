using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using UnityEngine;

namespace TPlus.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private static Dialogue _dialogue;
        private static GUIStyle _nodeStyle;
        private static Vector2 _mouseOffset = Vector2.zero;
        private static Vector2 _draggingOffset;
        private static float _minZoom = 0.25f;
        private static float _maxZoom = 1f;
        private static bool _draggingCanvas;
        private static bool _creatingDialogue;
        private static bool _hasDialogueCreationNameError;
        private static string _newDialogueName;
        private static Texture2D _npcNodeTexture;
        private static Texture2D _playerNodeTexture;

        private static DialogueNode _draggingNode;
        private static DialogueNode _connectingNode;
        private static DialogueNode _disconnectingNode;

        private const float CANVAS_SIZE = 4000;
        private const float BACKGROUND_IMAGE_PIXEL_SIZE = 50;


        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        #region Initialization
        private void OnEnable()
        {
            Selection.selectionChanged += SelectionChanged;
            _playerNodeTexture = MakeColoredTexture(20, 20, Color.blue);
            _npcNodeTexture = MakeColoredTexture(20, 20, Color.gray);
            _nodeStyle = new GUIStyle();
            _nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            _nodeStyle.border = new RectOffset(12,12, 12, 12);
        }

        private static Texture2D MakeColoredTexture(int width, int height, Color color)
        {
            // Create a new texture of the given width and height
            Texture2D texture = new Texture2D(width, height);

            // Fill the texture with the specified color
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply(); // Apply the color change to the texture

            return texture;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= SelectionChanged;
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            _dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if(_dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            
            return false;
        }

        private void SelectionChanged()
        {
            if (Selection.activeObject is Dialogue dialogue)
            {
                _dialogue = dialogue;
                Repaint();
            }
        }
        #endregion

        #region Input
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                var node = GetNodeAtPoint(Event.current.mousePosition + _dialogue.EditorScrollPosition);
                if (node != null)
                {
                    BeginNodeDrag(node);
                    return;
                }
                BeginScrollDrag();
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                DragNode();
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                ScrollDrag();
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                EditorUtility.SetDirty(_draggingNode);
                AssetDatabase.SaveAssets();
                _draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                EndScrollDrag();
            }

        }

        private void HandleZoomInput(Dialogue dialogue)
        {
            if (Event.current.type != EventType.ScrollWheel) return;

            dialogue.EditorZoomAmount -= Event.current.delta.y * 0.1f * Time.deltaTime;
            dialogue.EditorZoomAmount = Mathf.Clamp(dialogue.EditorZoomAmount, _minZoom, _maxZoom);

            EditorUtility.SetDirty(dialogue);
            AssetDatabase.SaveAssetIfDirty(dialogue);

            Event.current.Use();
        }

        private void BeginScrollDrag()
        {
            _draggingCanvas = true;
            _draggingOffset = Event.current.mousePosition + _dialogue.EditorScrollPosition;
        }

        private void ScrollDrag()
        {
            _dialogue.EditorScrollPosition = _draggingOffset - Event.current.mousePosition;
            GUI.changed = true;
        }

        private void EndScrollDrag()
        {
            _draggingCanvas = false;
        }

        private DialogueNode GetNodeAtPoint(Vector2 position)
        {
            DialogueNode foundNode = null;

            foreach (var node in _dialogue.GetDialogueNodes())
            {
                if (node.Transform.Contains(position / _dialogue.EditorZoomAmount))
                {
                    foundNode = node;
                    Selection.activeObject = foundNode;
                }
            }
            return foundNode;
        }

        private void BeginNodeDrag(DialogueNode node)
        {
            if (node.Transform == null)
            {
                node.Transform = new Rect(10, 10, 200, 150);
            }

            _mouseOffset.x = Event.current.mousePosition.x - node.Transform.x * _dialogue.EditorZoomAmount;
            _mouseOffset.y = Event.current.mousePosition.y - node.Transform.y * _dialogue.EditorZoomAmount;
            _draggingNode = node;
        }

        private void DragNode()
        {
            Undo.RecordObject(_dialogue, "Move dialogue node");
            var offset = _draggingNode.Transform.position - Event.current.mousePosition;
            _draggingNode.Transform.position = (Event.current.mousePosition - _mouseOffset) / _dialogue.EditorZoomAmount;
            GUI.changed = true;
        }

        #endregion

        #region DrawDisplay
        private void OnGUI()
        {
            DrawCreateDialogueButtons();
            if (_dialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }
            else if (!_creatingDialogue)
            {
                HandleZoomInput(_dialogue);
                ProcessEvents();
                HandleScrollView();
                DrawBackground();
                CreateNodesDisplay();

                
                var oldMatrix = GUI.matrix;

                CalculateZoom();

                EditorGUILayout.EndScrollView();

                GUI.matrix = oldMatrix;
            }
        }

        private static void CreateNodesDisplay()
        {
            List<DialogueNode> nodes = new List<DialogueNode>(_dialogue.GetDialogueNodes());

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                DrawNodeConnections(node);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                DrawNode(node);
            }
        }

        private static void DrawCreateDialogueButtons()
        {
            if (_creatingDialogue)
            {
                OpenCreateDialogueMenu();
            }
            else if (GUILayout.Button("Create new dialogue"))
            {
                _creatingDialogue = true;
            }
        }

        private static void OpenCreateDialogueMenu()
        {
            _newDialogueName = EditorGUILayout.TextField(_newDialogueName);
            if (GUILayout.Button("Create"))
            {
                if (!TryCreateNewDialogue())
                {
                    _hasDialogueCreationNameError = true;
                    return;
                }
                CloseCreateDialogueMenu();
                return;
            }
            if (GUILayout.Button("Cancel"))
            {
                CloseCreateDialogueMenu();
            }
            if (_hasDialogueCreationNameError)
            {
                EditorGUILayout.LabelField(_newDialogueName + " could not be created! Make sure another file does not exist with that name");
            }
        }

        private static void CloseCreateDialogueMenu()
        {
            _creatingDialogue = false;
            _hasDialogueCreationNameError = false;
        }

        private static void HandleScrollView()
        {
            _dialogue.EditorScrollPosition = EditorGUILayout.BeginScrollView(_dialogue.EditorScrollPosition);
        }

        private static void DrawBackground()
        {
            var canvasRect = GUILayoutUtility.GetRect(CANVAS_SIZE / _dialogue.EditorZoomAmount, CANVAS_SIZE / _dialogue.EditorZoomAmount);
            var bgTexture = Resources.Load("Background") as Texture2D;

            var texCoords = new Rect(0, 0, (canvasRect.width / (BACKGROUND_IMAGE_PIXEL_SIZE * _dialogue.EditorZoomAmount)), (canvasRect.height / (BACKGROUND_IMAGE_PIXEL_SIZE * _dialogue.EditorZoomAmount)));

            GUI.DrawTextureWithTexCoords(canvasRect, bgTexture, texCoords);
        }

        private static void CalculateZoom()
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                ApplyZoom();
            }
        }

        private static void ApplyZoom()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(_dialogue.EditorZoomAmount, _dialogue.EditorZoomAmount, 1));
        }

        private static void DrawNode(DialogueNode node)
        {
            //Create visual square in editor for each node in dialogue
            var oldRect = node.Transform;
            var scaledRect = new Rect(oldRect.x * _dialogue.EditorZoomAmount, oldRect.y * _dialogue.EditorZoomAmount, oldRect.width * _dialogue.EditorZoomAmount, oldRect.height * _dialogue.EditorZoomAmount);

            var newNodeStyle = _nodeStyle;
            newNodeStyle.normal.background = node.IsPlayerNode ? _playerNodeTexture : _npcNodeTexture;

            GUILayout.BeginArea(scaledRect, newNodeStyle);

            EditorGUI.BeginChangeCheck();
            GenerateTextFields(node, out var newText);

            DrawLinkButton(node);
            DrawUnlinkButton(node);

            var isNotLinking = (_connectingNode == null && _disconnectingNode == null);
            if (isNotLinking)
            {
                DrawCreateNodeButton(node);
                DrawDeleteButton(node);
                DrawIsPlayerNodeButton(node);
                DrawNodeConditions(node);
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveNodeChanges(node, newText);
            }

            GUILayout.EndArea();
        }

        private static void DrawNodeConditions(DialogueNode node)
        {
            var serializedNode = new SerializedObject(node);
            EditorGUILayout.PropertyField(serializedNode.FindProperty("Conditions"), true);
            serializedNode.ApplyModifiedProperties();
        }

        private static void DrawDeleteButton(DialogueNode node)
        {
            if (GUILayout.Button("-"))
            {
                Undo.RecordObject(_dialogue, "delete node");
                _dialogue.DeleteNode(node);
            }
        }

        private static void DrawCreateNodeButton(DialogueNode node)
        {
            if (GUILayout.Button("+"))
            {
                Undo.RecordObject(_dialogue, "add node");
                _dialogue.CreateChildNode(node);
            }
        }

        private static void DrawUnlinkButton(DialogueNode node)
        {
            if (_disconnectingNode == null && _connectingNode == null)
            {
                if (node.ChildNodes.Count == 0) return;

                if (GUILayout.Button("Unlink child node"))
                {
                    _disconnectingNode = node;
                }
            }
            else if (_connectingNode == null)
            {
                if (!_disconnectingNode.ChildNodes.Contains(node.UniqueID)) return;

                if (GUILayout.Button("Unlink"))
                {
                    Undo.RecordObject(_dialogue, "unlink nodes");
                    _dialogue.UnlinkNode(_disconnectingNode, node.UniqueID);
                    _disconnectingNode = null;
                }
            }
        }

        private static void DrawLinkButton(DialogueNode node)
        {
            if (_connectingNode == null && _disconnectingNode == null)
            {
                if (GUILayout.Button("Connect child node"))
                {
                    _connectingNode = node;
                }
            }
            else if (_disconnectingNode == null)
            {
                if (_connectingNode == node) return;

                if (GUILayout.Button("Link"))
                {
                    Undo.RecordObject(_dialogue, "link nodes");
                    _dialogue.LinkNode(_connectingNode, node.UniqueID);
                    _connectingNode = null;
                }
            }
        }

        private static void DrawNodeConnections(DialogueNode node)
        {
            //Create bezier from right edge of parent node to left edge of all children
            var start = node.Transform.center;
            start.x = node.Transform.xMax;
            start *= _dialogue.EditorZoomAmount;

            foreach (var childNode in _dialogue.GetAllChildren(node))
            {
                var vectorToChild = node.Transform.position - childNode.Transform.position;
                var offsetPoint = vectorToChild * 0.5f;
                offsetPoint.y = 0;
                offsetPoint *= _dialogue.EditorZoomAmount;

                var end = childNode.Transform.center;
                end.x = childNode.Transform.xMin;
                end *= _dialogue.EditorZoomAmount;
                Handles.DrawBezier(start, end, start - offsetPoint, end + offsetPoint, Color.white, null, 4f * _dialogue.EditorZoomAmount);
            }
        }

        private static void DrawIsPlayerNodeButton(DialogueNode node)
        {
            node.IsPlayerNode = GUILayout.Toggle(node.IsPlayerNode, "Is player node");
        }

        private static void GenerateTextFields(DialogueNode node, out string newText)
        {
            //Generates labels and text inputs for node information
            EditorGUILayout.LabelField("Node Text:");
            newText = EditorGUILayout.TextField(node.DialogueText);
        }
        #endregion

        private static void SaveNodeChanges(DialogueNode node, string newText)
        {
            Undo.RecordObject(_dialogue, "Update dialogue node text");
            node.DialogueText = newText;
            EditorUtility.SetDirty(node);
            AssetDatabase.SaveAssets();
        }

        private static bool TryCreateNewDialogue()
        {
            _dialogue = Dialogue.CreateNewDialogue(_newDialogueName);
            if (_dialogue != null)
            {
                Undo.RegisterCreatedObjectUndo(_dialogue, "create new dialogue");
                return true;
            }
            return false;
        }
    }
}

