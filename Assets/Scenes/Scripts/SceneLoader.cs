using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public string sceneNameToLoad; // The name of the scene you want to load.
    public TextMeshProUGUI HightScore;
    public TextMeshProUGUI GameTime;

    private void OnMouseDown()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
    void Start()
    {
        if(HightScore!=null){
            if (PlayerPrefs.HasKey("HighScore")) 
            {
                HightScore.text = "Highest Score : " + PlayerPrefs.GetInt("HighScore").ToString();
                GameTime.text = "Best Time : " + PlayerPrefs.GetString("GameTime");
            }else{
                HightScore.text = "Highest Score : 0" ;
                GameTime.text = "Best Time : 00:00:00" ;
            }
        }
    }
}
