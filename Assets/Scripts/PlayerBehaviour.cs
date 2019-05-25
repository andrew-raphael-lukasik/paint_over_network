using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehaviour : NetworkBehaviour
{

    [SerializeField] PlayerUI _uiPrefab = null;
    PlayerUI _uiInstance;

    public override void OnStartLocalPlayer ()
    {
        _uiInstance = Instantiate( _uiPrefab , transform );
    }

}
