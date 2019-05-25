using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerBehaviour : NetworkBehaviour
{

    [SerializeField] GameObject _canvasPrefab = null;
    GameObject _canvasInstance;


    public override void OnStartLocalPlayer ()
    {
        _canvasInstance = Instantiate( _canvasPrefab , transform );
    }
    
    [Command]
    public void SendTexture ()
    {
        
    }

}
