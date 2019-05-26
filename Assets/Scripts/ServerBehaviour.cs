using UnityEngine;
using UnityEngine.Networking;

public class ServerBehaviour : NetworkBehaviour
{

    [SerializeField] RectTransform _gallery;
    public RectTransform gallery => _gallery;
    
    public override void OnStartServer ()
    {
        //NetworkServer.Configure();
    }
    
}
