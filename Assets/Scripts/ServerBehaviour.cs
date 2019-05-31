using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Mirror;

public class ServerBehaviour : NetworkBehaviour
{

    [SerializeField] RectTransform _gallery = null;
    public RectTransform gallery => _gallery;
    
    //public override void OnStartServer () {}
    
}
