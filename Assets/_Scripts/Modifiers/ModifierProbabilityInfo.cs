using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "ModifierObject", order = -1)]
public class ModifierProbabilityInfo : ScriptableObject 
{
    public ModifierType type;
    public bool isRare;
    public ModifierRating rating;
    public int count;
    public float magnitude;
    public string displayText;
}

[System.Serializable]
public class ModifierInfoInstance 
{
    public int index;
    public ModifierType type;
    public bool isRare;
    public ModifierRating rating;
    public float magnitude;
    public string displayText;
}

public enum ModifierType
{
    PADDLE_BOUNCE_SPEED,
    BALL_START_SPEED,
    BALL_BOUNCE_SPEED,
    PADDLE_SIZE_PLAYER,
    PADDLE_SIZE_OPPONENT,
    PADDLE_SPEED_PLAYER,
    PADDLE_SPEED_OPPONENT,
    RANDOM_BARRIER_PLAYER,
    RANDOM_BARRIER_OPPONENT,
    BALL_SPEED_DIRECTIONAL_PLAYER,
    BALL_SPEED_DIRECTIONAL_OPPONENT,
    LENS_DISTORTION,
    BALL_DELAY_TIME,
    FLIP_SCREEN,
    BLUR_SCREEN,
    OBSTACLE_MIDDLE,
    NARROW_PLAYER,
    NARROW_OPPONENT,
    RANDOMIZE_PADDLES,

    HEAL_1 = 1000,
    BARRIER_1 = 1001,
    PASSIVE_HEALING = 1100,
    BALL_DAMAGE = 1101,
    NEGATE_DAMAGE = 1102,

    CREATE_OPPONENT_PADDLE = 1200,
    ACTIVATE_OPPONENT_AI = 1201
}

public enum ModifierRating
{
    POSITIVE = 0,
    NEUTRAL = 1,
    NEGATIVE = 2,
    HEALTH = 3
}
