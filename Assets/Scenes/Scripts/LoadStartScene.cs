using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadStartScene : MonoBehaviour
{
     public void OnClick()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void JumpScene(){
        SceneManager.LoadScene("StartScene");
    }
}
