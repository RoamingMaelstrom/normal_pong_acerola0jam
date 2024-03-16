using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;
using System;

public class BasePlayerController : MonoBehaviour
{
    [SerializeField] SOEvent startGameEvent;
    [SerializeField] IntSOEvent playerWinRoundEvent;
    [SerializeField] SOEvent togglePauseScreenEvent;

    public float paddleSpeed = 10f;
    [SerializeField] float speedGainOnPlayerVictory = 0.5f;

    private Vector2 movement;
    private float boosting;
    public Vector2 mousePos;

    public Rigidbody2D paddleBody;
    private bool controlsActive = false;

    private void Awake() {
        startGameEvent.AddListener(OnGameStart);
        playerWinRoundEvent.AddListener(IncreaseSpeed);
    }

    private void OnGameStart(){
        controlsActive = true;
    }

    private void IncreaseSpeed(int arg0)
    {
        paddleSpeed += speedGainOnPlayerVictory;
    }

    public void FixedUpdate() {
        if (!paddleBody || !controlsActive) return;
        paddleBody.velocity = movement * paddleSpeed; 
        MouseControl();
    }

    // Todo: Add Mouse Control option (Currently keyboard controls are overridden
    private void MouseControl() {
        float deltaY = Camera.main.ScreenToWorldPoint(mousePos).y - paddleBody.position.y;
        if (Mathf.Abs(deltaY) < 0.5f) 
        {
            paddleBody.velocity = Vector2.zero;
            return;
        }
        paddleBody.velocity = new Vector2(0, Mathf.Sign(deltaY)) * paddleSpeed;
    }

    public void SetPaddle(Rigidbody2D newPaddle){
        newPaddle.gameObject.SetActive(true);
        newPaddle.transform.position = paddleBody.transform.position;
        paddleBody.gameObject.SetActive(false);
        paddleBody = newPaddle;
    }

    void OnBoost(InputValue value) => boosting = value.Get<float>();

    void OnMousePosition(InputValue value) => mousePos = value.Get<Vector2>();

    void OnMove(InputValue value) => movement = value.Get<Vector2>();

    void OnPause(InputValue value) => togglePauseScreenEvent.Invoke();
}
