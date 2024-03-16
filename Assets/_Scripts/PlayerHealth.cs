using System.Collections.Generic;
using SOEvents;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] SOEvent gameOverEvent;
    public int maxHealth = 10;
    [SerializeField] int health = 3;

    [SerializeField] GameObject healthIconPrefab;
    [SerializeField] VerticalLayoutGroup healthIconParent;
    [SerializeField] List<GameObject> healthIcons = new();

    [SerializeField] string healthUpSfxName;
    [SerializeField] string healthDownSfxName;

    private List<HealingInstance> healingInstanceList = new();
    private List<NegateDamageInstance> negateDamageInstanceList = new();


    private bool triggeredGameOver = false;


    private void Start() {
        for (int i = 0; i < maxHealth; i++)
        {
            healthIcons.Add(Instantiate(healthIconPrefab, healthIconParent.transform));
            healthIcons[i].SetActive(i < health);
        }
    }

    public void IncrementHealth() {
        healthIcons[health].SetActive(true);
        health++;
        GliderAudio.SFX.PlayStandard(healthUpSfxName);
    }

    public void DecreaseHealth(int value) {
        health -= value;
        for (int i = 0; i < maxHealth; i++)
        {
            if (healthIcons[i].activeInHierarchy && i >= health) healthIcons[i].GetComponent<FlashAnimationUI>().PlayFlashingAnimation();
        }
        GliderAudio.SFX.PlayStandard(healthDownSfxName);

        if (health <= 0 && !triggeredGameOver){ 
            gameOverEvent.Invoke();
            triggeredGameOver = true;
        }
    }

    public void TickPassiveHealing(WinStatus winStatus) {
        if (winStatus == WinStatus.NONE) return;
        foreach (var instance in healingInstanceList)
        {
            instance.currentTick --;
            if (instance.currentTick <= 0){
                instance.currentTick = instance.healIntervalTicks;
                IncrementHealth();
            }
        }
    }

    public void AddPassiveHealing(int healTickInterval) => healingInstanceList.Add(new(healTickInterval));
    public void AddDamageNegation(float probability) => negateDamageInstanceList.Add(new(probability));

    public bool NegateDamage(){
        foreach (var instance in negateDamageInstanceList)
        {
            if (Random.Range(0f, 1f) < instance.probability) return true;
        }
        return false;
    }
}

public class HealingInstance 
{
    public int healIntervalTicks;
    public int currentTick;

    public HealingInstance(int healIntervalTicks) {
        this.healIntervalTicks = healIntervalTicks;
        currentTick = healIntervalTicks;
    }

}

public struct NegateDamageInstance
{
    public float probability;
    public NegateDamageInstance(float probability){
        this.probability = probability;
    }

}
