using UnityEngine;
using SOEvents;

public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponSOEvent requestDamageDealerEvent;
    [SerializeField] public WeaponSOEvent requestTargetEvent;
    [SerializeField] public WeaponSOEvent requestCooldownEvent;

    public int weaponTypeID;
    public Rigidbody2D parentBody;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



/*
using UnityEngine;
using SOEvents;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponSOEvent requestDamageDealerEvent;
    [SerializeField] public WeaponSOEvent requestTargetEvent;
    [SerializeField] public WeaponSOEvent requestCooldownEvent;
    public int weaponTypeID;
    public Vector2 offset;
    public float baseMaxCd;
    public float currentCd;
    public float accuracyCoefficient = 1f;

    public float damageMod;
    public float damageAdd;

    public float dotDamageMod;
    public float dotDamageAdd;

    public float dotIntervalMod;
    public float dotIntervalAdd;

    public float piercingAdd;

    public float cooldownMod;
    public float cooldownAdd;

    public int damageDealerTypeID;

    public WeaponTargetingType targetingType;
    public Vector3 currentTarget;
    public GameObject currentDamageDealerObject;

    public Rigidbody2D parentBody;

    public void TryFire(Rigidbody2D body)
    {
        parentBody = body;
         if (CheckIfCanFire()) Fire();
    }

    private void FixedUpdate() 
    {
        currentCd -= Time.fixedDeltaTime;
    }

    public virtual bool CheckIfCanFire()
    {
        if (currentCd > 0) return false;
        requestCooldownEvent.Invoke(this);
        return true;
    }
    public abstract void Fire();

    public void SetCurrentDamageDealerRotationToVelocity()
    {
        SetBodyRotationToVelocity(currentDamageDealerObject.GetComponent<Rigidbody2D>());
    }

    public void SetBodyRotationToVelocity(Rigidbody2D body)
    {
        float newRotation = - Mathf.Atan2(body.velocity.x, body.velocity.y) * Mathf.Rad2Deg;
        currentDamageDealerObject.transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }


    public Vector2 CalculateVelocity(Rigidbody2D playerBody, Vector3 targetPosition, float baseMunitionSpeed)
    {
        // Base Velocity based on Profile munitionSpeed and difference between target and weapon's position
        Vector2 outputVelocity = (currentTarget - transform.position).normalized * baseMunitionSpeed;

        // Modifies outputVelocity depending on accuracyCoefficient (1 = no modification, 0 = Lots of randomness)
        outputVelocity += new Vector2(outputVelocity.y * Random.Range(-1 + accuracyCoefficient, 1 - accuracyCoefficient) / 4f,
            outputVelocity.x * Random.Range(-1 + accuracyCoefficient, 1 - accuracyCoefficient) / 4f);

        // Projects player's Velocity onto outputVelocity (means munition speed adjusts depending on player velocity)
        outputVelocity += Vector2.Dot(outputVelocity, playerBody.velocity) * outputVelocity / outputVelocity.sqrMagnitude;

        return outputVelocity;
    }

}

public enum WeaponTargetingType 
{
    NEAREST,
    FARTHEST,
    FORWARD,
    BACKWARD,
    RANDOM_ENEMY,
    RANDOM,
    PLAYER,
    AT_MOUSE
}
*/