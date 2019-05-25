using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerBehaviour : NetworkBehaviour
{
    
    [SerializeField] GameObject _canvasPrefab = null;
    GameObject _canvasInstance;

    public override void OnStartServer ()
    {
        _canvasInstance = Instantiate( _canvasPrefab );
    }
    
}
