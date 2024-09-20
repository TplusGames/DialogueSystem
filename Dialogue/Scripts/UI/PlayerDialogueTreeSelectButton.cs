using TMPro;
using TPlus.Dialogue;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialogueTreeSelectButton : MonoBehaviour
{
    private Dialogue _dialogue;
    private Button _button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SelectDialogue);
    }

    public void SetDialogue(Dialogue dialogue)
    {
        _dialogue = dialogue;
        buttonText.text = _dialogue.Text;
    }

    private void SelectDialogue()
    {
        DialogueEventManager.Instance.InvokeOnDialogueSelected(_dialogue);
    }
}
