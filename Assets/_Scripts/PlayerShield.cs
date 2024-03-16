using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShield : MonoBehaviour
{
    public int maxShield = 5;
    public int shield = 1;

    [SerializeField] GameObject shieldIconPrefab;
    [SerializeField] VerticalLayoutGroup shieldIconParent;
    [SerializeField] List<GameObject> shieldIcons = new();
    [SerializeField] ShieldObjectLogic shieldObjectLogic;
    
    [SerializeField] string shieldUpSfxName;
    [SerializeField] string shieldDownSfxName;

    private void Start() {
        for (int i = 0; i < maxShield; i++)
        {
            shieldIcons.Add(Instantiate(shieldIconPrefab, shieldIconParent.transform));
            shieldIcons[i].SetActive(i < shield);
        }

        shield = Mathf.Clamp(shield, 0, maxShield);
        if (shield == 0) shieldObjectLogic.gameObject.SetActive(false);
    }

    public void IncrementShield() {
        shieldIcons[shield].SetActive(true);
        shield++;
        shieldObjectLogic.gameObject.SetActive(true);
        GliderAudio.SFX.PlayStandard(shieldUpSfxName);
    }

    public void DecreaseShield(int value) {
        shield -= value;
        shield = Mathf.Max(shield, 0);
        for (int i = 0; i < maxShield; i++)
        {
            if (shieldIcons[i].activeInHierarchy && i >= shield) shieldIcons[i].GetComponent<FlashAnimationUI>().PlayFlashingAnimation();
        }

        if (shield <= 0) shieldObjectLogic.gameObject.SetActive(false);
        GliderAudio.SFX.PlayStandard(shieldDownSfxName);
    }
}
