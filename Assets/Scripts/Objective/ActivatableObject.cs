using UnityEngine;
using System.Collections;

public interface IActivatableObject {

    void OnActivate(PlayerManager player);

    void OnDeactivate(PlayerManager player);

    bool ActivateCheck(PlayerManager player);

}
