using UnityEngine;
using System.Collections;

public class ArrowManager : MonoBehaviour {

    public ObjectiveManager objectiveManager;
   
    // Update is called once per frame
    void Update () {
        PointTowardsObjective();
	}

    void PointTowardsObjective()
    {
        Vector3 playerCentre = GameManager.GetPlayersCentre();
        ObjectiveAbstract obj = objectiveManager.findClosestObjectiveToPlayers();
        if(obj != null)
        {
            Vector3 direction = (obj.transform.position- playerCentre).normalized;
            float angle = Angle360(Vector3.forward, direction, Vector3.left);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+45));
        }
    }

    float Angle360(Vector3 from, Vector3 to, Vector3 right)
    {
        float angle = Vector3.Angle(from, to);
        return (Vector3.Angle(right, to) > 90f) ? 360f - angle : angle;
    }
}
