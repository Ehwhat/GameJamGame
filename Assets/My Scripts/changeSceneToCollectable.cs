using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class changeSceneToCollectable : MonoBehaviour
{
    public UnityEngine.UI.Text charText;

    public void LoadStage()
    {
        PlayerPrefs.SetString("player_ones_name", charText.text);
        /*PlayerPrefs.SetString("player_twos_name", charText.text);
        PlayerPrefs.SetString("player_threes_name", charText.text);
        PlayerPrefs.SetString("player_fours_name", charText.text);*/
        SceneManager.LoadScene("collectable", LoadSceneMode.Single);
        //PlayerPrefs.DeleteAll();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}