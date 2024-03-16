using UnityEngine;
using TMPro;
using System.Collections.Generic;
using SOEvents;
using UnityEngine.EventSystems;

public class ModifierCardDisplayLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent playerWinRoundEvent;
    [SerializeField] BoolSOEvent updatePauseCounterEvent;
    [SerializeField] List<GameObject> cardObjects = new();
    [SerializeField] List<TextMeshProUGUI> cardDescriptionTextList = new();
    [SerializeField] Modifier modifierClass;
    [SerializeField] ModifierRandomiser modifierRandomiser;
    [SerializeField] AnimatedPanel cardParentPanel;

    private int cardCount = 0;
    private List<int> modifierCardNumberMapping = new();
    private List<ModifierInfoInstance> instanceList = new();

    private void Awake() {
        playerWinRoundEvent.AddListener(OpenModifierCardWindow);
    }

    public void OpenModifierCardWindow(int playerScore){
        cardParentPanel.Open();
        EventSystem.current.SetSelectedGameObject(null);
        switch (playerScore)
        {
            case 1: 
            {
                cardCount = 1;
                cardObjects[0].SetActive(true);
                break;
            }
            case 2:
            {
                cardCount = 2;
                cardObjects[1].SetActive(true);
                break;
            }
            case 3:
            {
                cardCount = 4;
                cardObjects[2].SetActive(true);
                cardObjects[3].SetActive(true);
                break;
            }
            default: cardCount = 4; break;
        }

        SetCardModifiers(playerScore, cardCount);
    }

    public void SetCardModifiers(int playerScore, int cardCount){
        instanceList = modifierRandomiser.GetModifiersForCards(playerScore, cardCount);
        modifierCardNumberMapping = new();

        int cardNumber = 0;
        for (int i = 0; i < instanceList.Count; i++)
        {
            if (instanceList[i].rating == ModifierRating.POSITIVE || instanceList[i].rating == ModifierRating.HEALTH){
                cardDescriptionTextList[cardNumber].SetText(instanceList[i].displayText + "\n\n<b><size=150%>AND</b><size=100%>\n\n" + instanceList[i + 1].displayText);
                modifierCardNumberMapping.Add(cardNumber);
                i++;
            }
            else cardDescriptionTextList[cardNumber].SetText(instanceList[i].displayText);

            modifierCardNumberMapping.Add(cardNumber);
            cardNumber ++;
        }
    }
    public void SelectUpgrade(int cardNumber){
        for (int i = 0; i < modifierCardNumberMapping.Count; i++)
        {
            if (cardNumber == modifierCardNumberMapping[i]) modifierClass.ApplyModifier(instanceList[i]);
        }

        cardParentPanel.Close();
        updatePauseCounterEvent.Invoke(false);
    }
}
