using UnityEngine;
using System.Collections;

public class DisarmMinesObjective : BoundObjective {

    [SerializeField]
    private Mine _minePrefab;
    public static string PROPERTY_MINE_PREFAB = "_minePrefab";

    [SerializeField]
    private int _mineAmount = 3;
    public static string PROPERTY_MINE_AMOUNT = "_mineAmount";

    [SerializeField]
    private Mine[] _mines;

    // Use this for initialization
    void Start () {
        DistributeMines();
    }
	
	// Update is called once per frame
	void Update () {
        if (_objectiveActive && CheckMines())
        {
            Debug.Log("Disarmmed");
            ObjectiveSuccess();
        }
	}

    bool CheckMines()
    {
        foreach(Mine mine in _mines)
        {
            if (!mine._disarmed)
            {
                return false;
            }
        }
        return true;
    }

    void DistributeMines()
    {
        _mines = new Mine[_mineAmount];
        for (int i = 0; i < _mineAmount; i++)
        {
            _mines[i] = Instantiate<Mine>(_minePrefab);

            Vector3 pos = GetRandomPositionInBounds();
            RaycastHit hit;

            if (Physics.Raycast(pos + Vector3.up*10, Vector3.down, out hit))
            {
                pos = hit.point;
            }

            _mines[i].transform.parent = transform;
            _mines[i].transform.position = GetRandomPositionInBounds();
        }
    }

}
