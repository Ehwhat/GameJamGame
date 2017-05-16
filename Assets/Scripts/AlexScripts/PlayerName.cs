using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerName : UIInputField
{
    Text nameText;

	// Use this for initialization
	void Start ()
    {
        if (gameObject.name == "Player 1 Name")
        {
            nameText = GameObject.Find("Player 1 Name").GetComponent<Text>();
            nameText.text = PlayerPrefs.GetString("player_ones_name");
        }
        /*else if (gameObject.name == "Player 2 Name")
        {
            nameText = GameObject.Find("Player 2 Name").GetComponent<Text>();
            nameText.text = PlayerPrefs.GetString("player_twos_name");
        }
        else if (gameObject.name == "Player 3 Name")
        {
            nameText = GameObject.Find("Player 3 Name").GetComponent<Text>();
            nameText.text = PlayerPrefs.GetString("player_threes_name");
        }
        else if (gameObject.name == "Player 4 Name")
        {
            nameText = GameObject.Find("Player 4 Name").GetComponent<Text>();
            nameText.text = PlayerPrefs.GetString("player_fours_name");
        }*/
        else
        {

        }

    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    
}
