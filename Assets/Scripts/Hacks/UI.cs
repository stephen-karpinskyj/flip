using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField]
    private Text currentScoreText;

    [SerializeField]
    private Text bestScoreText;

    private int score;

    private void Awake()
    {
        this.bestScoreText.text = string.Format("Lo:{0}", this.GetBestScore());
        this.currentScoreText.text = 0.ToString();
    }

    private void Start()
    {
        MusicManager.Instance.Player.OnScoreBeat += this.HandleScoreBeat;
        MusicManager.Instance.Player.OnSongEnd += this.HandleSongEnd;
    }

    private void OnDestroy()
    {
        if (MusicManager.Exists)
        {
            MusicManager.Instance.Player.OnMeasureChange -= this.HandleScoreBeat;
            MusicManager.Instance.Player.OnSongEnd -= this.HandleSongEnd;
        }
    }
    
    private int GetBestScore()
    {
        return PlayerPrefs.GetInt("score.best", 999);
    }

    private void Reset()
    {
        SceneManager.LoadScene(0);
    }

    public void UGUI_HandleResetButtonClick()
    {
        this.Reset();
    }

    private void HandleScoreBeat(int numScoreBeats)
    {
        score = numScoreBeats;
        currentScoreText.text = this.score.ToString();
    }
    
    private void HandleSongEnd()
    {
        if (this.score < this.GetBestScore())
        { 
            PlayerPrefs.SetInt("score.best", this.score);
            PlayerPrefs.Save();
        }
        this.Reset();
    }
}
