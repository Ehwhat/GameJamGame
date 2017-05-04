using UnityEngine;
using System.Collections;

public abstract class BoundObjective : ObjectiveAbstract {

    public enum BoundsType
    {
        Box,
        Circle
    }

    [SerializeField]
    protected BoundsType _boundsType = BoundsType.Circle;
    public static string PROPERTY_BOUNDS_TYPE = "_boundsType";

    [SerializeField]
    protected float _boundsRadius = 6f;
    public static string PROPERTY_BOUNDS_RADIUS = "_boundsRadius";

    [SerializeField]
    protected Vector2 _boundsArea = new Vector2(12, 12);
    public static string PROPERTY_BOUNDS_BOX = "_boundsArea";

    public Vector3 GetRandomPositionInBounds()
    {
        Vector3 position = transform.position;
        if(_boundsType == BoundsType.Circle)
        {
            Vector3 random = Random.insideUnitCircle*_boundsRadius;
            position = transform.position + new Vector3(random.x, 0, random.y);
        }else if(_boundsType == BoundsType.Box)
        {
            Vector3 random = new Vector3(Random.Range(-_boundsArea.x / 2, _boundsArea.x / 2), 0, Random.Range(-_boundsArea.y / 2, _boundsArea.y / 2));
            position = transform.position + random;
        }
        return position;
    }


}
