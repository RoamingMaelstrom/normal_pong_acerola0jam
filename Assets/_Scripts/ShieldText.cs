using UnityEngine;
using TMPro;

public class ShieldText : MonoBehaviour
{
    [SerializeField] PlayerShield playerShield;
    [SerializeField] TextMeshProUGUI shieldText;

    private void FixedUpdate(){
        if (playerShield.shield > 0) shieldText.gameObject.SetActive(true);
        else shieldText.gameObject.SetActive(false);
    }
}
