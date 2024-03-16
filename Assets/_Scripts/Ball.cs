using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    public float currentSpeed;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask opponentLayer;
    [SerializeField] List<string> collisionsSfx = new();

    public float defaultSpeedIncrease = 0.25f;
    public float playerSpeedIncrease = 0.5f;
    public float opponentSpeedIncrease = 0.5f;


    private void OnCollisionEnter2D(Collision2D other) {
        int otherLayer = 1 << other.gameObject.layer;
        if (playerLayer.value == otherLayer) AddPlayerBallSpeed();
        else if (opponentLayer.value == otherLayer) AddOpponentBallSpeed();
        else AddDefaultBallSpeed();

        GliderAudio.SFX.PlayRandomStandard(1.0f, collisionsSfx.ToArray());
    }

    private void AddDefaultBallSpeed(){
        currentSpeed += defaultSpeedIncrease;
    }

    private void AddOpponentBallSpeed(){
        currentSpeed += opponentSpeedIncrease;
    }

    private void AddPlayerBallSpeed(){
        currentSpeed += playerSpeedIncrease;
    }

    public void Freeze() => body.simulated = false;
    public void UnFreeze() => body.simulated = true;

    public int GetXDirection(){
        return body.velocity.x switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0,
        };
    }
}
