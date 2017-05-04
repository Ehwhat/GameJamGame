using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour {

    public GameObject[] spells;

    public void CastSpell(int curSpell)
    {
        spells[curSpell].GetComponent<Animator>().SetTrigger("isActivated");
    }
}
