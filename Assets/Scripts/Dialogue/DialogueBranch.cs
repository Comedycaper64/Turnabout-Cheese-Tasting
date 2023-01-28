using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueBranch", fileName = "NewDialogueBranch")]
public class DialogueBranch : ConversationNode
{
    public List<DialogueRoute> dialogueRoutes;

    public bool inInvestigation;

    public bool exitConvoButton;

    public DialogueRoute wrongHeldItemRoute;
    
    public List<DialogueRoute> GetAvailableRoutes()
    {
        List<DialogueRoute> unlockedRoutes = new List<DialogueRoute>();
        foreach(DialogueRoute route in dialogueRoutes)
        {
            if(route.itemKey == null)
            {
                unlockedRoutes.Add(route);
            }
        }
        return unlockedRoutes;
    }
    
}
