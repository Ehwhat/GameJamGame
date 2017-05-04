using UnityEngine;
using System.Collections;

public class ClearAreaOfAllEnemiesObjective : BoundObjective {

    [System.Serializable]
    public class ObjectiveEnemy
    {
        public GameObject _enemyGameObject;
        public static string PROPERTY_ENEMY_GAMEOBJECT = "_enemyGameObject";

        public bool _useCustomPosition;
        public static string PROPERTY_USE_CUSTOM_POSITION = "_useCustomPosition";

        public Vector3 _customPosition;
        public static string PROPERTY_CUSTOM_POSITION = "_customPosition";

        public bool IsDead()
        {
            return ((_enemyGameObject == null || !_enemyGameObject.activeInHierarchy));
        }

    }

    [SerializeField]
    private bool _isActive = true;

    [SerializeField]
    private bool _spawnEnemies = false;
    public static string PROPERTY_SPAWN_ENEMIES = "_spawnEnemies";

    [SerializeField]
    private ObjectiveEnemy[] _enemies;
    public static string PROPERTY_ENEMIES = "_enemies";


    // Use this for initialization
    void Start () {
        if (_isActive)
        {
            if (_spawnEnemies)
            {
                PopulateAreaWithEnemies(_enemies);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (CheckIfEnemiesDead())
        {
            ObjectiveSuccess();
        }

    }

    bool CheckIfEnemiesDead()
    {
        foreach(ObjectiveEnemy e in _enemies)
        {
            if (!e.IsDead())
            {
                return false;
            }
        }
        return true;
    }

    void PopulateAreaWithEnemies(ObjectiveEnemy[] enemies)
    {
        foreach(ObjectiveEnemy e in enemies)
        {
            Vector3 enemyPosition = (e._useCustomPosition) ? e._customPosition : GetRandomPositionInArea();
            e._enemyGameObject = (GameObject)Instantiate(e._enemyGameObject, enemyPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPositionInArea()
    {
        Vector3 result = Vector3.zero;
        if(_boundsType == BoundsType.Circle)
        {
            result = transform.position + Random.insideUnitSphere * _boundsRadius;
        }else if(_boundsType == BoundsType.Box)
        {
            result = transform.position + new Vector3(Random.Range(-_boundsArea.x/2, _boundsArea.x/2),0, Random.Range(-_boundsArea.y/2, _boundsArea.y/2));
        }
        return result;
    }

}
