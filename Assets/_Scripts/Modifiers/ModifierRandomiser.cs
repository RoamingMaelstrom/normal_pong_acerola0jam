using System.Collections.Generic;
using UnityEngine;

public class ModifierRandomiser : MonoBehaviour
{
    [SerializeField] Modifier modifierClass;
    public List<ModifierInfoInstance> GetModifiersForCards(int playerScore, int cardCount) {
        if (IsSpecialModifierGeneration(playerScore)) return GetPresetModifierCards(playerScore);

        bool[] cardsArePositive = new bool[cardCount]; 
        for (int i = 0; i < cardCount - 1; i++) cardsArePositive[i] = Random.Range(0f, 2f) > 1f;
        cardsArePositive[cardCount - 1] = true;

        return GetRandomModifierInstances(cardsArePositive, playerScore);
    }

    private List<ModifierInfoInstance> GetRandomModifierInstances(bool[] cardsArePositive, int playerScore) {
        List<ModifierInfoInstance> output = new();

        float shortestCommon = modifierClass.GetShortestCommonListLength();
        float shortestRare = modifierClass.GetShortestRareListLength();
        bool isRare;

        for (int i = 0; i < cardsArePositive.Length - 1; i++)
        {
            isRare = RandomIsRare(shortestCommon, shortestRare, playerScore);

            output.Add(modifierClass.GetRandomModifierInfo(cardsArePositive[i] ? ModifierRating.POSITIVE : ModifierRating.NEUTRAL, isRare));
            if (cardsArePositive[i]) output.Add(modifierClass.GetRandomModifierInfo(ModifierRating.NEGATIVE, isRare));
        }

        isRare = RandomIsRare(shortestCommon, shortestRare, playerScore);

        output.Add(modifierClass.GetRandomHealthModifierInfo(isRare));
        output.Add(modifierClass.GetRandomModifierInfo(ModifierRating.NEGATIVE, isRare));

        return output;
    }

    private bool RandomIsRare(float shortestCommon, float shortestRare, int playerScore) => Random.Range(0f, 1f) > shortestRare / (shortestCommon + shortestRare) && playerScore >= 5;

    private List<ModifierInfoInstance> GetPresetModifierCards(int playerScore) {
        return playerScore == 1 ? GetCreatePaddleCards() : GetActivateEnemyAICards();
    }

    private List<ModifierInfoInstance> GetCreatePaddleCards()
    {
        return new()
        {
            modifierClass.CreateOpponentPaddleModifier
        };
    }

    private List<ModifierInfoInstance> GetActivateEnemyAICards()
    {
        List<ModifierInfoInstance> output = new()
        {
            modifierClass.GetRandomModifierInfo(ModifierRating.POSITIVE, false),
            modifierClass.ActivateOpponentAIModifier
        };

        ModifierInfoInstance positiveMod2;
        int c = 0;
        do 
        {
            c++;
            positiveMod2 = modifierClass.GetRandomModifierInfo(ModifierRating.POSITIVE, false);
        } while(positiveMod2.type == output[0].type && c < 25);

        output.Add(positiveMod2);
        output.Add(modifierClass.ActivateOpponentAIModifier);

        return output;
    }

    private bool IsSpecialModifierGeneration(int playerScore) {
        if (playerScore <= 2) return true;
        return false;
    }
}
