using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance {get; private set;}

	// SERIALIZABLES
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private TextMeshProUGUI interactText;
	[SerializeField] private Image characterImageLeft;
	[SerializeField] private Image characterImageRight;
	[SerializeField] private Animator dialogueAnimator;
	[SerializeField] private AudioSource dialogueAudioSource;
	[SerializeField] private Transform routeButtonUIPrefab;
	[SerializeField] private TextMeshProUGUI loadingUI;
	[SerializeField] private Transform routeButtonContainerTransform;
	[SerializeField] private Transform exitConversationButton;
	[SerializeField] private float timeBetweenLetterTyping;

	// TRACKERS
	private Conversation currentConversation;
	private Dialogue currentDialogue;
	private string currentSentence;
	public bool inConversation = false;
	private bool currentTextTyping = false;

	private Coroutine typingCoroutine;
	private Image currentCharacterImage;
	private AudioClip currentCharacterTalkSound;
	private Queue<ConversationNode> conversationNodes;
	private Queue<string> sentences;
	private Queue<Sprite> characterImages;
	private List<RouteButtonUI> routeButtonUIList;

	//MISC
	private float inactiveTalkerAlpha = 0.2f;

	private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one DialogueManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

		sentences = new Queue<string>();
		characterImages = new Queue<Sprite>();
		conversationNodes = new Queue<ConversationNode>();
		routeButtonUIList = new List<RouteButtonUI>();
    }

	void Start()
	{
		InputReader.Instance.InteractEvent += OnInteract;
		currentCharacterImage = null;
	}

	private void OnInteract()
	{
		if (inConversation && currentDialogue != null)
		{
			if (!currentTextTyping)
			{
				DisplayNextSentence();
			}
			else
			{
				FinishTypingSentence();
			}
		}
	}

    public void SetNameText()
	{
		nameText.text = "";
	}

	public void SetNameText(Dialogue dialogue)
	{
		nameText.text = dialogue.characterName;		
	}

	private void ClearCharacterImages()
	{
		ClearCharacterImage(characterImageLeft);
		ClearCharacterImage(characterImageRight);
	}

	private void ClearCharacterImage(Image characterImage)
	{
		characterImage.enabled = false;
		//characterImage.sprite = null;
		//characterImage.color = new Color(255, 255, 255, 0f);
	}

	public void ShowInteractText()
	{
		interactText.enabled = true;
	}

	public void CloseInteractText()
	{
		interactText.enabled = false;
	}


	public void StartConversation(Conversation conversation)
	{
		inConversation = true;
		dialogueAnimator.SetTrigger("startConversation");
		currentConversation = conversation;
		conversationNodes.Clear();
		dialogueAudioSource.volume = SoundManager.Instance.GetSoundEffectVolume() / 2;
		ClearCharacterImages();
		foreach (ConversationNode conversationNode in currentConversation.conversationNodes)
		{
			conversationNodes.Enqueue(conversationNode);
		}
		StartDialogue(conversationNodes.Dequeue());
	}

	public void StartDialogue(ConversationNode conversationNode)
	{
		
		string nodeType = conversationNode.GetType().ToString();

		switch(nodeType)
		{
			case "DialogueBranch":
				currentDialogue = null;
				PresentDialogueBranch((DialogueBranch)conversationNode);
				break;

			case "DialogueAddItem":
				currentDialogue = ScriptableObject.CreateInstance<Dialogue>();
				AddItem((DialogueAddItem)conversationNode);
				break;

			case "DialogueChangeScene":
				currentDialogue = null;
				ChangeScene((DialogueChangeScene)conversationNode);
				break;

			default:
			case "Dialogue":
				currentDialogue = (Dialogue)conversationNode;
				sentences.Clear();
				foreach (string sentence in currentDialogue.sentences)
				{
					sentences.Enqueue(sentence);
				}
				foreach (Sprite image in currentDialogue.characterImages)
				{
					characterImages.Enqueue(image);
					//Debug.Log(image);
				}
				currentCharacterTalkSound = currentDialogue.characterTalkSound;
				if (currentCharacterTalkSound)
					dialogueAudioSource.clip = currentCharacterTalkSound;

				DisplayNextSentence();
				break;
		}
	}
	
	private void ChangeScene(DialogueChangeScene changeScene)
	{
		if (changeScene.changeToScene != -1)
		{
			loadingUI.enabled = true;
			MenuManager.Instance.LoadLevel(changeScene.changeToScene);
		}

		if (changeScene.fadeToBlack)
			StartCoroutine(SceneHelperScripts.Instance.FadeToBlack());
		else
			StartCoroutine(SceneHelperScripts.Instance.FadeFromBlack());
		if (changeScene.musicTrack)
			SoundManager.Instance.SetMusicTrack(changeScene.musicTrack);
		if (changeScene.sceneBackground)
			SceneHelperScripts.Instance.ChangeSceneBackground(changeScene.sceneBackground);
		dialogueText.color = changeScene.sceneFontColour;
		EndDialogue();
	}

	private void AddItem(DialogueAddItem dialogueAddItem)
	{
		if (!Inventory.Instance.GetInventoryObjects().Contains(dialogueAddItem.item))
		{
			Inventory.Instance.AddToInventory(dialogueAddItem.item);
			currentSentence = "Item added to journal: " + dialogueAddItem.itemName;
			typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
		}
		else
		{
			currentSentence = dialogueAddItem.itemName + " already in inventory";
			typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
		}
	}

	private void PresentDialogueBranch(DialogueBranch dialogueBranch)
	{
		ClearRouteOptions();
		foreach (DialogueRoute route in dialogueBranch.GetAvailableRoutes())
		{
			Transform routeButtonTransform = Instantiate(routeButtonUIPrefab, routeButtonContainerTransform);
			RouteButtonUI routeButtonUI = routeButtonTransform.GetComponent<RouteButtonUI>();
			routeButtonUI.CreateRoute(route);
			routeButtonUIList.Add(routeButtonUI);
		}
		if (dialogueBranch.inInvestigation)
		{
			Transform presentButtonTransform = Instantiate(routeButtonUIPrefab, routeButtonContainerTransform);
			RouteButtonUI presentButtonUI = presentButtonTransform.GetComponent<RouteButtonUI>();
			presentButtonUI.CreatePresentButton(dialogueBranch);
			routeButtonUIList.Add(presentButtonUI);
		}
		if (dialogueBranch.exitConvoButton)
			exitConversationButton.gameObject.SetActive(true);
	}

	private void ClearRouteOptions()
	{
		foreach (Transform buttonTransform in routeButtonContainerTransform)
		{
			Destroy(buttonTransform.gameObject);
		}
		routeButtonUIList.Clear();
	}

	public void SelectRoute(Conversation conversation)
	{
		ClearRouteOptions();
		exitConversationButton.gameObject.SetActive(false);
		foreach(ConversationNode node in conversation.conversationNodes)
		{
			conversationNodes.Enqueue(node);
		}
		EndDialogue();
	}

	public void CheckHeldItem(DialogueBranch currentBranch)
	{
		InventoryObject heldItem = Inventory.Instance.heldObject;
		if (heldItem == null)
		{
			SelectRoute(currentBranch.wrongHeldItemRoute.routeConversation);
			return;
		}
		foreach(DialogueRoute route in currentBranch.dialogueRoutes)
		{
			if (route.itemKey == heldItem)
			{
				SelectRoute(route.routeConversation);
				return;
			}
		}
		SelectRoute(currentBranch.wrongHeldItemRoute.routeConversation);
	}

	/*
	public void InterruptDialogue(Dialogue dialogue, int distance, bool isQuestioning)
	{
		sentences.Clear();
		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		for (int i = 0; i < dialogueCounter; i++)
		{
			sentences.Dequeue();
		}
		DisplayNextSentence(dialogue, distance, isQuestioning, false);
	}
	*/

	public void DisplayNextSentence()
	{
		SetNameText(currentDialogue);
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}
		string sentence = sentences.Dequeue();

		//characterImageLeft.color

		if (currentDialogue.rightSideCharacterImage)
		{
			characterImageLeft.color = new Color(255, 255, 255, inactiveTalkerAlpha);
			currentCharacterImage = characterImageRight;
		}
		else if (currentDialogue.leftSideCharacterImage)
		{
			characterImageRight.color = new Color(255, 255, 255, inactiveTalkerAlpha);
			currentCharacterImage = characterImageLeft;
		}
		else
		{
			characterImageLeft.color = new Color(255, 255, 255, inactiveTalkerAlpha);
			characterImageRight.color = new Color(255, 255, 255, inactiveTalkerAlpha);
			currentCharacterImage = null;
		}

		if (currentCharacterImage)
			currentCharacterImage.color = new Color(255, 255, 255, 1);

		if (characterImages.Count > 0)
		{
			Sprite image = characterImages.Dequeue();
			if (image != null)
			{
				currentCharacterImage.enabled = true;
				currentCharacterImage.sprite = image;
			}
			else
				ClearCharacterImage(currentCharacterImage);
		}

		if (typingCoroutine != null)
			StopCoroutine(typingCoroutine);
		typingCoroutine = StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence)
	{
		currentSentence = sentence;
		currentTextTyping = true;
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			dialogueAudioSource.Play();
			yield return new WaitForSeconds(timeBetweenLetterTyping);
		}
		currentTextTyping = false;
	}

	private void FinishTypingSentence()
    {
        if (typingCoroutine != null)
			StopCoroutine(typingCoroutine);
		dialogueText.text = currentSentence;
		currentTextTyping = false;
    }

	public void ExitConversationButton()
	{
		ClearRouteOptions();
		exitConversationButton.gameObject.SetActive(false);
		EndDialogue();
	}

	public void EndDialogue()
	{
		if (currentCharacterImage != null)
			currentCharacterImage.color = new Color(255, 255, 255, inactiveTalkerAlpha);
		
		SetNameText();
		StartCoroutine(TypeSentence(""));
		
		if (conversationNodes.TryDequeue(out ConversationNode nextNode))
		{
			StartDialogue(nextNode);
		}
		else
		{
			EndConversation();
		}
	}

	private void EndConversation()
	{
		dialogueAnimator.SetTrigger("endConversation");
			StartCoroutine(DialogueCooldown());
	}

	private IEnumerator DialogueCooldown()
	{
		yield return new WaitForEndOfFrame();
		inConversation = false;
	}

}
