using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradePropertyCard : MonoBehaviour
{
    MonopolyNode nodeReference;

    [SerializeField] Image colorField;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image typeImage;
    [SerializeField] Sprite houseSprite, railRoadSprite, utilitySprite;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] Toggle toggleButton;

    public void SetTradeCard(MonopolyNode node, ToggleGroup toggleGroup)
    {
        if (node == null)
        {
            gameObject.SetActive(false);
            return;
        }

        nodeReference = node;
        colorField.color = node.propertyColorField != null ? node.propertyColorField.color : Color.black;
        propertyNameText.text = node.name;
        switch (node.monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                typeImage.sprite = houseSprite;
                typeImage.color = Color.blue;
                break;
            case MonopolyNodeType.Railroad:
                typeImage.sprite = railRoadSprite;
                typeImage.color = Color.white;
                break;
            case MonopolyNodeType.Utility:
                typeImage.sprite = utilitySprite;
                typeImage.color = Color.black;
                break;
        }

        mortgageImage.SetActive(node.IsMortgaged);
        propertyPriceText.text = node.price + "RON";
        toggleButton.isOn = false;
        toggleButton.group = toggleGroup;
    }

    public MonopolyNode Node()
    {
        return nodeReference;
    }
}