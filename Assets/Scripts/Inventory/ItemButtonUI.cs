using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    //private InventoryObject inventoryObject;

    public void CreateItemButton(InventoryObject inventoryObject)
    {
        //this.inventoryObject = inventoryObject;
        textMeshPro.text = inventoryObject.objectName;
        button.onClick.AddListener(() => {
            Journal.Instance.ShowItemDetails(inventoryObject);
        });
    }
}
