using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject menuGameObject;
    [SerializeField] private Conversation prologueConversation;
    [SerializeField] private Conversation investigationStartConversation;

    private GameObject currentOpenScreen;

    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one DialogueManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            mainMenuScreen.SetActive(true);
            currentOpenScreen = mainMenuScreen;
            optionsScreen.SetActive(false);
        }
    }

    private void Start() 
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            DialogueManager.Instance.StartConversation(investigationStartConversation);

    }

    public void StartPrologue()
    {
        StartCoroutine(SceneHelperScripts.Instance.FadeToBlack());
        menuGameObject.SetActive(false);
        DialogueManager.Instance.StartConversation(prologueConversation);
    }

    


    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }

    public void OpenScreen(GameObject screen)
    {
        if (currentOpenScreen)
            currentOpenScreen.SetActive(false);
        screen.SetActive(true);
        currentOpenScreen = screen;
    }

    public void CloseCurrentScreen()
    {
       currentOpenScreen.SetActive(false);
       currentOpenScreen = null;
    }

}
