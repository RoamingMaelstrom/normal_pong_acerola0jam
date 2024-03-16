using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Modifier : MonoBehaviour
{
    [SerializeField] string modifierInfoObjectsFolderPath;
    [SerializeField] ModifierProbabilityInfo[] modifierProbabilityInfo; 

    [SerializeField] List<ModifierInfoInstance> commonPositiveModifiers = new();
    [SerializeField] List<ModifierInfoInstance> commonNeutralModifiers = new();
    [SerializeField] List<ModifierInfoInstance> commonNegativeModifiers = new();
    [SerializeField] List<ModifierInfoInstance> commonHealthModifiers = new();

    [SerializeField] List<ModifierInfoInstance> rarePositiveModifiers = new();
    [SerializeField] List<ModifierInfoInstance> rareNeutralModifiers = new();
    [SerializeField] List<ModifierInfoInstance> rareNegativeModifiers = new();
    [SerializeField] List<ModifierInfoInstance> rareHealthModifiers = new();

    public ModifierInfoInstance CreateOpponentPaddleModifier {get; private set;}
    public ModifierInfoInstance ActivateOpponentAIModifier {get; private set;}

    [SerializeField] BasePlayerController basePlayerController;
    [SerializeField] EnemyAI enemyAI;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] PlayerShield playerShield;
    [SerializeField] Ball ball;
    [SerializeField] BallManager ballManager;
    [SerializeField] GameObject playerPaddle;
    [SerializeField] GameObject opponentPaddle;

    [SerializeField] GameObject narrowPlayerEndObject;
    [SerializeField] GameObject narrowOpponentEndObject;

    [SerializeField] GameObject staticPlayerPaddlePrefab;
    [SerializeField] GameObject staticOpponentPaddlePrefab;
    [SerializeField] GameObject middleObstaclePrefab;
    [SerializeField] BoundsInt playerBarrierBounds;
    [SerializeField] BoundsInt opponentBarrierBounds;
    [SerializeField] PostProcessVolume postProcessing;
    [SerializeField] GameObject UIContentObject;
    [SerializeField] string[] applyModifierSFX;

    [SerializeField] RectTransform uiBlurParent;
    [SerializeField] RectTransform worldBlurParent;
    [SerializeField] GameObject uiBlurPrefab;
    [SerializeField] GameObject worldBlurPrefab;
    [SerializeField] Material uiBlurMaterial;
    [SerializeField] Material worldBlurMaterial;

    [SerializeField] List<Rigidbody2D> playerPaddles = new();
    [SerializeField] List<Rigidbody2D> opponentPaddles = new();
    private int playerPaddleIndex = 0;
    private int opponentPaddleIndex = 0;

    private int validStaticPaddlePlaces = 0;
    public int validObstaclePlaces = 0;

    private List<GameObject> playerStaticBarriers = new();
    private List<GameObject> opponentStaticBarriers = new();

    [SerializeField] string activateStaticBarrierSfx;
    [SerializeField] string deactivateStaticBarrierSfx;

    private void Start() {

        CreateOpponentPaddleModifier = new ModifierInfoInstance
        {
            type = ModifierType.CREATE_OPPONENT_PADDLE,
            rating = ModifierRating.NEUTRAL,
            displayText = "Create Paddle for Opponent"
        };

        ActivateOpponentAIModifier = new ModifierInfoInstance
        {
            type = ModifierType.ACTIVATE_OPPONENT_AI,
            rating = ModifierRating.NEGATIVE,
            displayText = "Activate AI Opponent"
        };

        modifierProbabilityInfo = Resources.LoadAll<ModifierProbabilityInfo>(modifierInfoObjectsFolderPath);

        foreach (var probInfo in modifierProbabilityInfo) 
        {
            for (int i = 0; i < probInfo.count; i++) 
            {
                ModifierInfoInstance info = new() {
                    index = -1,
                    type = probInfo.type,
                    isRare = probInfo.isRare,
                    rating = probInfo.rating,
                    magnitude = probInfo.magnitude,
                    displayText = probInfo.displayText
                };

                AddModifierInfoToList(info);
            }
        }

        ResetBlur();
        ResetLensDistortion();
    }

    private void ResetLensDistortion()
    {
        if (postProcessing.profile.TryGetSettings(out LensDistortion lensDistortion)){
            lensDistortion.enabled.value = false;
        }
    }

    private void FixedUpdate() {
        int ballDirection = ball.GetXDirection();
        if (ballDirection != 0) RunStaticBarrierAudioLogic(ballDirection);

        for (int i = 0; i < playerStaticBarriers.Count; i++) playerStaticBarriers[i].SetActive(ballDirection <= 0);
        for (int i = 0; i < opponentStaticBarriers.Count; i++) opponentStaticBarriers[i].SetActive(ballDirection >= 0);
    }

    private void RunStaticBarrierAudioLogic(int ballDirection) {
        bool sfxPlayed = false;
        if (playerStaticBarriers.Count > 0) sfxPlayed = RunPlayerStaticBarrierSfxLogic(ballDirection, sfxPlayed, playerStaticBarriers[0].activeInHierarchy);
        if (opponentStaticBarriers.Count > 0) RunOpponentStaticBarrierSfxLogic(ballDirection, sfxPlayed, opponentStaticBarriers[0].activeInHierarchy);
    }

    private bool RunPlayerStaticBarrierSfxLogic(int ballDirection, bool sfxPlayed, bool barriersActive) {
        if (sfxPlayed || ballDirection == 0) return sfxPlayed;

        if (ballDirection == 1 && barriersActive){
            GliderAudio.SFX.PlayStandard(deactivateStaticBarrierSfx);
            return true;
        }
        if (ballDirection == -1 && !barriersActive){
            GliderAudio.SFX.PlayStandard(activateStaticBarrierSfx);
            return true;
        }
        return false;
    }

    private bool RunOpponentStaticBarrierSfxLogic(int ballDirection, bool sfxPlayed, bool barriersActive) {
        if (sfxPlayed || ballDirection == 0) return sfxPlayed;

        if (ballDirection == 1 && !barriersActive){
            GliderAudio.SFX.PlayStandard(activateStaticBarrierSfx);
            return true;
        }
        if (ballDirection == -1 && barriersActive){
            GliderAudio.SFX.PlayStandard(deactivateStaticBarrierSfx);
            return true;
        }
        return false;
    }

    private void ResetBlur(){
        uiBlurMaterial.SetFloat("_BlurStrength", 0f);
        worldBlurMaterial.SetFloat("_BlurStrength", 0f);
    }

    public int GetShortestCommonListLength(){
        return Mathf.Min(commonHealthModifiers.Count, commonNegativeModifiers.Count, commonNeutralModifiers.Count, commonPositiveModifiers.Count);
    }

    public int GetShortestRareListLength(){
        return Mathf.Min(rareHealthModifiers.Count, rareNegativeModifiers.Count, rareNeutralModifiers.Count, rarePositiveModifiers.Count);
    }

    private void AddModifierInfoToList(ModifierInfoInstance info)
    {
        List<ModifierInfoInstance> instanceList = GetModifierInfoInstanceList(info.rating, info.isRare);
        instanceList.Add(info);
        info.index = instanceList.Count - 1;
    }

    private List<ModifierInfoInstance> GetModifierInfoInstanceList(ModifierRating modifierRating, bool isRare){
        if (isRare){
            return modifierRating switch
            {
                ModifierRating.NEUTRAL => rareNeutralModifiers,
                ModifierRating.POSITIVE => rarePositiveModifiers,
                ModifierRating.NEGATIVE => rareNegativeModifiers,
                ModifierRating.HEALTH => rareHealthModifiers,
                _ => new(),
            };
        }

        return modifierRating switch
        {
            ModifierRating.NEUTRAL => commonNeutralModifiers,
            ModifierRating.POSITIVE => commonPositiveModifiers,
            ModifierRating.NEGATIVE => commonNegativeModifiers,
            ModifierRating.HEALTH => commonHealthModifiers,
            _ => new(),
        };
    }

    private ModifierInfoInstance GetRandomItemFromModifierInfoList(List<ModifierInfoInstance> infoList) => infoList[Random.Range(0, infoList.Count)];

    public ModifierInfoInstance GetRandomModifierInfo(ModifierRating modifierRating, bool isRare) {

        List<ModifierInfoInstance> instanceList = GetModifierInfoInstanceList(modifierRating, isRare);
        if (instanceList.Count == 0) 
        {
            Debug.Log(modifierRating);
            Debug.Log("Should be unreachable. modifierRating == Health?");
            return new();
        }

        return GetRandomItemFromModifierInfoList(instanceList);
    }

    
    public ModifierInfoInstance GetRandomHealthModifierInfo(bool isRare)
    {
        if (isRare) return GetRandomItemFromModifierInfoList(rareHealthModifiers);
        return GetRandomItemFromModifierInfoList(commonHealthModifiers);
    }



    public void RemoveModifierInfoInstance(int index, ModifierRating modifierRating, bool isRare){

        List<ModifierInfoInstance> instanceList = GetModifierInfoInstanceList(modifierRating, isRare);
        RemoveAndUpdateIndexes(instanceList, index);
    }

    private void RemoveAndUpdateIndexes(List<ModifierInfoInstance> instanceList, int index){
        if (instanceList.Count == 1) return;
        if (instanceList.Count <= index)
        {
            Debug.Log(string.Format("Count less than index (ABNORMAL CASE)   index={0}     count={1}", index, instanceList.Count));
            instanceList.RemoveAt(instanceList.Count - 1);
            return;
        }
        
        instanceList.RemoveAt(index);
        for (int i = 0; i < instanceList.Count; i++) instanceList[i].index = i;
    }

    public void ApplyModifier(ModifierInfoInstance modifier){
        switch (modifier.type)
        {
            case ModifierType.PADDLE_BOUNCE_SPEED: ModifyPaddleBounceSpeedGain(modifier.magnitude); break;
            case ModifierType.BALL_BOUNCE_SPEED: ModifyDefaultBounceSpeedGain(modifier.magnitude); break;
            case ModifierType.BALL_START_SPEED: ModifyBallStartSpeed(modifier.magnitude); break; 
            case ModifierType.PADDLE_SIZE_PLAYER: ModifyPlayerPaddleSize(modifier.magnitude); break;
            case ModifierType.PADDLE_SIZE_OPPONENT: ModifyOpponentPaddleSize(modifier.magnitude); break;
            case ModifierType.PADDLE_SPEED_PLAYER: ModifyPlayerPaddleSpeed(modifier.magnitude); break;
            case ModifierType.PADDLE_SPEED_OPPONENT: ModifyOpponentPaddleSpeed(modifier.magnitude); break;
            case ModifierType.RANDOM_BARRIER_PLAYER: PlaceRandomBarrierPlayer(modifier.magnitude); break;
            case ModifierType.RANDOM_BARRIER_OPPONENT: PlaceRandomBarrierOpponent(modifier.magnitude); break;
            case ModifierType.BALL_SPEED_DIRECTIONAL_PLAYER: ModifyPlayerDirectionBallSpeed(modifier.magnitude); break;
            case ModifierType.BALL_SPEED_DIRECTIONAL_OPPONENT: ModifyOpponentDirectionBallSpeed(modifier.magnitude); break;
            case ModifierType.LENS_DISTORTION: EnableLensDistortion(modifier.magnitude); break;
            case ModifierType.FLIP_SCREEN: FlipScreen(modifier.magnitude); break;
            case ModifierType.BLUR_SCREEN: BlurScreen(modifier.magnitude); break;
            case ModifierType.OBSTACLE_MIDDLE: AddObstacleMiddle(modifier.magnitude); break;
            case ModifierType.NARROW_PLAYER: NarrowPlayerEnd(modifier.magnitude); break;
            case ModifierType.NARROW_OPPONENT: NarrowOpponentEnd(modifier.magnitude); break;
            case ModifierType.RANDOMIZE_PADDLES: RandomizePaddles(modifier.magnitude); break;
            case ModifierType.HEAL_1: HealPlayerByOne(modifier.magnitude); break;
            case ModifierType.BARRIER_1: AddOneBarrier(modifier.magnitude); break;
            case ModifierType.PASSIVE_HEALING: AddPassiveHealingInstance(modifier.magnitude); break;
            case ModifierType.NEGATE_DAMAGE: AddNegateDamageInstance(modifier.magnitude); break;
            case ModifierType.BALL_DAMAGE: IncreaseBallDamage(modifier.magnitude); break;
            case ModifierType.BALL_DELAY_TIME: ModifyBallStartDelay(modifier.magnitude); break;
            case ModifierType.CREATE_OPPONENT_PADDLE: EnableOpponentPaddle(modifier.magnitude); break;
            case ModifierType.ACTIVATE_OPPONENT_AI: EnableOpponentAI(modifier.magnitude); break;

            default: break;
        }

        RemoveModifierInfoInstance(modifier.index, modifier.rating, modifier.isRare);
        GliderAudio.SFX.PlayRandomStandard(1f, applyModifierSFX);
    }

    private void EnableOpponentPaddle(float arg0){
        enemyAI.paddleBody.gameObject.SetActive(true);
    }

    private void EnableOpponentAI(float arg0){
        enemyAI.aiActive = true;
    }

    public void RandomizePaddles(float arg0) {
        int newPlayerPaddleIndex = Random.Range(0, playerPaddles.Count);
        int newOpponentPaddleIndex = Random.Range(0, opponentPaddles.Count);

        if (newPlayerPaddleIndex == playerPaddleIndex) newPlayerPaddleIndex = (newPlayerPaddleIndex + 1) % playerPaddles.Count;
        if (newOpponentPaddleIndex == opponentPaddleIndex) newOpponentPaddleIndex = (newOpponentPaddleIndex + 1) % playerPaddles.Count;

        basePlayerController.SetPaddle(playerPaddles[newPlayerPaddleIndex]);
        enemyAI.SetPaddle(opponentPaddles[newOpponentPaddleIndex]);
    }

    public void NarrowPlayerEnd(float arg0){
        narrowPlayerEndObject.SetActive(true);
    }

    public void NarrowOpponentEnd(float arg0){
        narrowOpponentEndObject.SetActive(true);
    }

    public void AddObstacleMiddle(float arg0){
        int spawnLocation;
        int c = 0;
        do
        {
            c++;
            spawnLocation = Random.Range(0, 3);
        } while (c < 25 && (1 << spawnLocation & validObstaclePlaces) > 0);

        Vector3 spawnPos = new(0f, (9.5f * spawnLocation) - 9.5f);
        if ((1 << spawnLocation & validObstaclePlaces) == 0) validObstaclePlaces += 1 << spawnLocation;
        Instantiate(middleObstaclePrefab, spawnPos, Quaternion.Euler(0, 0, 45f));
    }

    public void BlurScreen(float magnitude)
    {   
        //Instantiate(uiBlurPrefab, uiBlurParent);
        //Instantiate(worldBlurPrefab, worldBlurParent);
        //Debug.Log(uiBlurMaterial);
        //Debug.Log(uiBlurMaterial.GetFloat("_BlurStrength"));
        uiBlurMaterial.SetFloat("_BlurStrength", uiBlurMaterial.GetFloat("_BlurStrength") + 0.75f);
        //Debug.Log(uiBlurMaterial.GetFloat("_BlurStrength"));
        worldBlurMaterial.SetFloat("_BlurStrength", worldBlurMaterial.GetFloat("_BlurStrength") + 2.25f);
    }

    private void AddNegateDamageInstance(float probability) => playerHealth.AddDamageNegation(probability);

    public void FlipScreen(float arg0) {
        Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles + new Vector3(0, 0, 180));
        UIContentObject.transform.rotation = Camera.main.transform.rotation;
        ballManager.flippedScreen = ! ballManager.flippedScreen;
    }

    private void EnableLensDistortion(float arg0){
        if (postProcessing.profile.TryGetSettings(out LensDistortion lensDistortion)){
            lensDistortion.enabled.value = true;
            Camera.main.orthographicSize -= 1f;
        }
    }

    private void ModifyPaddleBounceSpeedGain(float multiplier){
        ball.playerSpeedIncrease += multiplier;
        ball.opponentSpeedIncrease += multiplier;
    }

    private void ModifyDefaultBounceSpeedGain(float multiplier){
        ball.defaultSpeedIncrease += multiplier;
    }

    private void ModifyBallStartSpeed(float addition){
        ballManager.ballStartSpeed += addition;
        basePlayerController.paddleSpeed += addition * 0.75f;
        enemyAI.paddleSpeed += addition * 0.75f;

    }

    private void ModifyPlayerPaddleSize(float addition){
        basePlayerController.paddleBody.transform.localScale += new Vector3(0f, addition, 0f);
    }

    private void ModifyOpponentPaddleSize(float addition){
        enemyAI.paddleBody.transform.localScale += new Vector3(0f, addition, 0f);
    }

    private void ModifyPlayerPaddleSpeed(float addition){
        basePlayerController.paddleSpeed += addition;
    }

    private void ModifyOpponentPaddleSpeed(float addition){
        enemyAI.paddleSpeed += addition;
        enemyAI.speedLerpStrength += 0.1f;
    }

    public void PlaceRandomBarrierPlayer(float arg0){
        int posNumber;
        int c = 0;
        do 
        {
            c++;
            posNumber = Random.Range(0, 16);
        } while (c < 50 && (1 << posNumber & validStaticPaddlePlaces) > 0);

        if (c < 50) validStaticPaddlePlaces += 1 << posNumber;

        Vector3 position = new Vector3(playerBarrierBounds.xMin + (playerBarrierBounds.size.x * (posNumber % 4) / 3f),
             playerBarrierBounds.yMin + (playerBarrierBounds.size.y * (posNumber / 4) / 3f), 0f);
        playerStaticBarriers.Add(Instantiate(staticPlayerPaddlePrefab, position, Quaternion.identity));
    }

    public void PlaceRandomBarrierOpponent(float arg0){
        int posNumber;
        int c = 0;
        do 
        {
            c++;
            posNumber = Random.Range(16, 32);
        } while (c < 50 && (1 << posNumber & validStaticPaddlePlaces) > 0);
        
        if (c < 50) validStaticPaddlePlaces += 1 << posNumber;


        Vector3 position = new Vector3(opponentBarrierBounds.xMin + (opponentBarrierBounds.size.x * (posNumber % 4) / 3f),
         opponentBarrierBounds.yMin + (opponentBarrierBounds.size.y * ((posNumber - 16) / 4) / 3f), 0f);
        opponentStaticBarriers.Add(Instantiate(staticOpponentPaddlePrefab, position, Quaternion.identity));
    }

    private void ModifyPlayerDirectionBallSpeed(float multiplier){
        ballManager.playerDirectionSpeedMultiplier *= 1f + multiplier;
    }

    private void ModifyOpponentDirectionBallSpeed(float multiplier){
        ballManager.opponentDirectionSpeedMultiplier *= 1f + multiplier;
    }

    private void HealPlayerByOne(float arg0) => playerHealth.IncrementHealth();

    private void AddOneBarrier(float arg0) => playerShield.IncrementShield();

    private void AddPassiveHealingInstance(float healIntervalTicks) => playerHealth.AddPassiveHealing((int)healIntervalTicks);

    private void IncreaseBallDamage(float arg0) => ballManager.ballDamage ++;
    private void ModifyBallStartDelay(float multiplier) => ballManager.ballDelayMs = (int)(ballManager.ballDelayMs * (1f + multiplier));
}
