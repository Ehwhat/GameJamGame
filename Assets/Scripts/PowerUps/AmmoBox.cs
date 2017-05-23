using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IActivatableObject
{
    public bool ActivateCheck(PlayerManager player)
    {
        return true;
    }

    public void OnActivate(PlayerManager player)
    {
        player.GiveAmmoPack();
        InputContextManager.RemoveIndicator(transform);
        Destroy(gameObject);
    }

    public void OnDeactivate(PlayerManager player)
    {
        
    }
}
