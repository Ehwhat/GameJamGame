using UnityEngine;
using System.Collections;

<<<<<<< HEAD
public abstract class BoundObjective : ObjectiveAbstract {
=======
public abstract class BoundObjective : ObjectiveAbstract
{
>>>>>>> ObjectiveGeneration

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




}
