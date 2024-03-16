using UnityEngine;


namespace GliderServices
{
    public class AccountSystem : MonoBehaviour
    {
        [SerializeField] SubmitHighscore submitHighscoreObject;
        [SerializeField] RetrieveHighscores retrieveHighscoresObject;

        [SerializeField] string leaderboardId;


        static AccountSystem mainSystem;
        static bool hasMainSystem = false;

        private void Awake() 
        {
            if (!hasMainSystem) 
            {
                mainSystem = this;
                hasMainSystem = true;
            }
            else Destroy(gameObject);
        }

        private async void Start()
        {
            if (!PlayerLocalInfo.IsSetup()) PlayerLocalInfo.SetupPlayerPrefs();

            await ServiceConnection.InitUnityServices();

            if (!ServiceConnection.IsConnectedToNetwork())
            {
                Debug.Log("Could not find local network. Aborting Sign-In.");
                return;
            }

            await ServiceConnection.SignIn();
            await retrieveHighscoresObject.LoadScores(leaderboardId);
        }

        private async void OnApplicationQuit() 
        {
            RenameUser(PlayerLocalInfo.PlayerName);
            await submitHighscoreObject.TrySubmitScore(leaderboardId, PlayerLocalInfo.BestScore);
            ServiceConnection.SignOut();
        }

        public void RenameUser(string newName)
        {
            PlayerLocalInfo.PlayerName = newName;
            ServiceConnection.SyncPlayerNameServerToLocal(PlayerLocalInfo.PlayerName);
        }

        public async void ResetAccount()
        {
            await ServiceConnection.DeleteAccount();
            await ServiceConnection.SignIn();  // Creates a new account automatically.
            PlayerLocalInfo.SetupPlayerPrefs();
        }

        public async void SubmitHighscore(int newScore)
        {
            ScoreCheckLog log = new();
            if (!submitHighscoreObject.RunSubmitScoreChecks(ref log, newScore, PlayerLocalInfo.GetScoreMode())) {
                //Debug.Log(log.GetStateAsString());
                return;
            }
            //Debug.Log(log.GetStateAsString());

            ServiceConnection.SyncPlayerNameServerToLocal(PlayerLocalInfo.PlayerName);
            await submitHighscoreObject.TrySubmitScore(leaderboardId, newScore, runChecks: false);

            await retrieveHighscoresObject.LoadScores(leaderboardId);
        }
    }
}
