using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouteButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    private Conversation routeConversation;

    public void CreateRoute(DialogueRoute dialogueRoute)
    {
        this.routeConversation = dialogueRoute.routeConversation;
        textMeshPro.text = dialogueRoute.dialogueOption;
        button.onClick.AddListener(() => {
            DialogueManager.Instance.SelectRoute(routeConversation);
        });
    }

    public void CreatePresentButton(DialogueBranch dialogueBranch)
    {
        textMeshPro.text = "Present Held Evidence";
        button.onClick.AddListener(() => {
            DialogueManager.Instance.CheckHeldItem(dialogueBranch);
        });
    }
}
