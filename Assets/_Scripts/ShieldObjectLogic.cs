using UnityEngine;

public class ShieldObjectLogic : MonoBehaviour
{
    [SerializeField] BallManager ballManager;
    [SerializeField] PlayerShield playerShield;
    [SerializeField] FlashAnimationObject flashAnimationObject;
    private void OnCollisionEnter2D(Collision2D other) {
        playerShield.DecreaseShield(ballManager.ballDamage);
        if (playerShield.shield > 0) flashAnimationObject.PlayFlashingAnimation();
    }
}
