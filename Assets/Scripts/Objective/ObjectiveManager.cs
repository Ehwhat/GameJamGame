using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour {

    [System.Serializable]
    public struct ObjectivePrefab
    {
        public LevelPopulation.PrefabSize size;
        public ObjectiveAbstract objective;
    }

    public ObjectivePrefab[] objectivePrefabs;
    public int amountOfObjectives;

    public List<ObjectiveAbstract> objectives;

    public List<ObjectivePrefab> DecideObjectives(int amount, out int bigObjective, out int midObjectives)
    {
        midObjectives = 0;
        bigObjective = 0;
        List<ObjectivePrefab> decidedList = new List<ObjectivePrefab>();
        for (int i = 0; i < amount; i++)
        {
            ObjectivePrefab randomObjective = objectivePrefabs[Random.Range(0, objectivePrefabs.Length)];
            if(randomObjective.size == LevelPopulation.PrefabSize.Medium)
            {
                midObjectives++;
            }else if (randomObjective.size == LevelPopulation.PrefabSize.Large)
            {
                bigObjective++;
            }
            decidedList.Add(randomObjective);
        }
        return decidedList;
        
    }

    public void PopulateObjectives(List<ObjectivePrefab> decidedObjs,List<Vector3>mediumSpots, List<Vector3>bigSpots)
    {
        for (int i = 0; i < decidedObjs.Count; i++)
        {
            ObjectivePrefab objective = decidedObjs[i];
            Vector3 position = Vector3.zero;
            if (objective.size == LevelPopulation.PrefabSize.Medium)
            {
                int rand = Random.Range(0, mediumSpots.Count);
                position = mediumSpots[rand];
                mediumSpots.RemoveAt(rand);
            }else if(objective.size == LevelPopulation.PrefabSize.Large)
            {
                int rand = Random.Range(0, bigSpots.Count);
                position = bigSpots[rand];
                mediumSpots.RemoveAt(rand);
            }
            ObjectiveAbstract obj = Instantiate<ObjectiveAbstract>(objective.objective);
            obj.transform.position = position;
            RegisterObjective(obj);
        }
    }

    public void RegisterObjective(ObjectiveAbstract obj)
    {
        objectives.Add(obj);
        obj.AddObjectiveCallbacks(AddSuccessScore, AddFailureScore);
    }

    public void AddSuccessScore(ObjectiveAbstract obj)
    {
        GameManager.AddScore(obj._objectiveSuccessScore);
    }

    public void AddFailureScore(ObjectiveAbstract obj)
    {
        GameManager.AddScore(obj._objectiveFailureScore);
    }

    public bool CheckAllObjectivesDone()
    {
        if(objectives.Count > 0)
        {
            foreach(ObjectiveAbstract obj in objectives)
            {
                if(obj != null || obj._objectiveActive)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public ObjectiveAbstract findClosestObjectiveToPlayers()
    {
        ObjectiveAbstract bestObjective = null;
        if (objectives.Count > 0) {
            Vector3 playerCentre = GameManager.GetPlayersCentre();
            float bestDistance = Mathf.Infinity;
            if (objectives[0]._objectiveActive)
            {
                bestObjective = objectives[0];
                bestDistance = Vector3.Distance(objectives[0].transform.position, playerCentre);
            }
            foreach (ObjectiveAbstract obj in objectives)
            {
                if (obj._objectiveActive)
                {
                    float distance = Vector3.Distance(obj.transform.position, playerCentre);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestObjective = obj;
                    }
                }
            }
        }
        return bestObjective;
    }

}
