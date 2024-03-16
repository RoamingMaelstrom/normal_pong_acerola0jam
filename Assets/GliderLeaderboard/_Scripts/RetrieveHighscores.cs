using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace GliderServices
{
    public class RetrieveHighscores : MonoBehaviour
    {
        public RetrievedScoreObject[] scoreObjects;
        [SerializeField] int numScoreObjectsRetrieved = 10;
        public LeaderboardScoresPage scoresResponse {get; private set;}

        public async Task LoadScores(string leaderboardId)
        {
            GetScoresOptions scoreOptions = CreateScoreOptions(numScoreObjectsRetrieved);
            Debug.Log("Leaderboard Retrieve Initiated.");
            scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, scoreOptions);
            Debug.Log("Leaderboard Retrieve Completed.");
            scoreObjects = new RetrievedScoreObject[numScoreObjectsRetrieved];

            for (int i = 0; i < scoresResponse.Results.Count; i++)
            {
                LeaderboardEntry score = scoresResponse.Results[i];
                scoreObjects[i] = new RetrievedScoreObject(score.PlayerId, score.Rank, score.PlayerName, (int)score.Score);
            }

            AddBlankScoreObjects(numScoreObjectsRetrieved - scoresResponse.Results.Count, scoreObjects);
        }

        private GetScoresOptions CreateScoreOptions(int numScores)
        {
            GetScoresOptions scoreOptions = new GetScoresOptions
            {
                Limit = numScores,
                Offset = 0
            };
            return scoreOptions;
        }

        private void AddBlankScoreObjects(int numBlankObjects, RetrievedScoreObject[] scoreObjectArray)
        {
            for (int i = scoreObjectArray.Length - numBlankObjects; i < scoreObjectArray.Length; i++)
            {
                scoreObjectArray[i] = new RetrievedScoreObject("#", i, "", 0);
            } 
        }
    }

    
    [System.Serializable]
    public class RetrievedScoreObject
    {
        public string playerID;
        public int rank;
        public string playerName;
        public int score;

        public RetrievedScoreObject(string _playerID, int _rank, string _playerName, int _score)
        {
            playerID = _playerID;
            rank = _rank;
            playerName = _playerName;
            score = _score;
        }
    }
}
