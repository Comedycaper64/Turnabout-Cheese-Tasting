using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Journal : MonoBehaviour
{
    public static Journal Instance {get; private set;}

    [SerializeField] private GameObject journalUI;
    [SerializeField] private Image heldItemUI;
    [SerializeField] private Transform itemButtonUIPrefab;
	[SerializeField] private Transform itemButtonContainerTransform;
    [SerializeField] private Image itemDetailsImage;
    [SerializeField] private TextMeshProUGUI itemDetailsText;
    public bool journalOpen;
    private List<ItemButtonUI> itemButtonUIList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one Journal! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        InputReader.Instance.OpenJournalEvent += OpenJournal; 
        //Inventory.Instance.HeldItemChangedEvent += ChangeHeldItem;
        itemButtonUIList = new List<ItemButtonUI>();
        journalUI.SetActive(false);
        journalOpen = false;
    }

    public void OpenJournal()
    {
        if (journalOpen)
        {
            journalUI.SetActive(false);
            journalOpen = false;
            return;
        }

        journalUI.SetActive(true);
        journalOpen = true;
        ShowItemButtons();
        if (Inventory.Instance.heldObject != null)
        {
            ShowItemDetails(Inventory.Instance.heldObject);
        }
    }

    private void ShowItemButtons()
    {
        ClearItemButtons();
        foreach(InventoryObject inventoryObject in Inventory.Instance.GetInventoryObjects())
        {
            Transform itemButtonTransform = Instantiate(itemButtonUIPrefab, itemButtonContainerTransform);
			ItemButtonUI itemButtonUI = itemButtonTransform.GetComponent<ItemButtonUI>();
			itemButtonUI.CreateItemButton(inventoryObject);
			itemButtonUIList.Add(itemButtonUI);
        }
    }

    public void ShowItemDetails(InventoryObject inventoryObject)
    {
        itemDetailsImage.sprite = inventoryObject.objectImage;
        itemDetailsText.text = inventoryObject.objectDescription;
        heldItemUI.sprite = inventoryObject.objectImage;
        Inventory.Instance.SetHeldObject(inventoryObject);
    }

    private void ClearItemButtons()
    {
        foreach(Transform itemButton in itemButtonContainerTransform)
        {
            Destroy(itemButton.gameObject);
        }
        itemButtonUIList.Clear();
    }
}
