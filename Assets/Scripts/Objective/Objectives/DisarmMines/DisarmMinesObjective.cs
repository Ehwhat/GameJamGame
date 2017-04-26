using UnityEngine;
using System.Collections;

public class DisarmMinesObjective : BoundObjective {

    [SerializeField]
    private float _mineAmount = 3;
    public static string PROPERTY_MINE_AMOUNT = "_mineAmount";

    [SerializeField]
    private Mine[] _mines;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
