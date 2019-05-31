// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.
using System.ComponentModel;
using UnityEngine;

namespace Mirror
{
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [HelpURL("https://vis2k.github.io/Mirror/Components/NetworkManagerHUD")]
    public class MyNetworkManagerHUD : MonoBehaviour
    {
        NetworkManager manager;
        public bool showGUI = true;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void OnGUI()
        {
            if (!showGUI)
                return;

            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.black;

            int HEIGHT = Screen.height;
            int WIDTH = Screen.width;
            Rect areaRect;
            {
                int w = WIDTH / 5;
                int h = (int)( HEIGHT * 0.9f );
                int x = ( WIDTH - w ) / 2;
                int y = ( HEIGHT - h ) / 2;
                areaRect = new Rect(x,y,w,h);
            }
            int MIN = 18;
            var newLineHeigth = GUILayout.Height( Mathf.Max( HEIGHT / 10 , MIN ) );
            var newLineHeigthHalved = GUILayout.Height(  Mathf.Max( HEIGHT / 20 , MIN ) );

            GUILayout.BeginArea( areaRect );
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                if (!NetworkClient.active)
                {
                    // // LAN Host
                    // if (Application.platform != RuntimePlatform.WebGLPlayer)
                    // {
                    //     if (GUILayout.Button("LAN Host"))
                    //     {
                    //         manager.StartHost();
                    //     }
                    // }

                    // LAN Client + IP
                    //GUILayout.BeginHorizontal();
                    {
                        const string defaulthost = "localhost";
                        const string networkAddressKey = "manager.networkAddress";
                        manager.networkAddress = GUILayout.TextField( PlayerPrefs.GetString( "manager.networkAddress" , defaulthost ) , newLineHeigthHalved );
                        if( manager.networkAddress.Length==0 ) manager.networkAddress = defaulthost;
                        PlayerPrefs.SetString( networkAddressKey , manager.networkAddress );
                        
                        if( GUILayout.Button("START",newLineHeigth) )//if (GUILayout.Button("LAN Client"))
                        {
                            manager.StartClient();
                        }
                    }
                    //GUILayout.EndHorizontal();

                    // LAN Server Only
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // cant be a server in webgl build
                        GUILayout.Box("(  WebGL cannot be server  )");
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Start Server",newLineHeigthHalved)) manager.StartServer();
                        // if (GUILayout.Button("LAN Server Only")) manager.StartServer();
                    }
                }
                else
                {
                    // Connecting
                    GUILayout.Label("Connecting to " + manager.networkAddress + "..");
                    if (GUILayout.Button("Cancel Connection Attempt",newLineHeigth))
                    {
                        manager.StopClient();
                    }
                }
            }
            else
            {
                // server / client status message
                if (NetworkServer.active)
                {
                    GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
                }
                if (NetworkClient.isConnected)
                {
                    GUILayout.Label("Client: address=" + manager.networkAddress);
                }
            }

            // client ready
            if (NetworkClient.isConnected && !ClientScene.ready)
            {
                if (GUILayout.Button("Client Ready",newLineHeigth))
                {
                    ClientScene.Ready(NetworkClient.connection);

                    if (ClientScene.localPlayer == null)
                    {
                        ClientScene.AddPlayer();
                    }
                }
            }

            GUILayout.EndArea();

            // stop
            //if (NetworkServer.active || NetworkClient.isConnected)
            if( NetworkClient.isConnected )
            {
                if( GUILayout.Button("Exit",newLineHeigthHalved,GUILayout.Width(WIDTH/10)) )
                {
                    manager.StopHost();
                }
            }
            if( NetworkServer.active )
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" GET MY IP: ",newLineHeigthHalved);
                GUILayout.TextField("https://www.whatismybrowser.com/detect/what-is-my-local-ip-address",newLineHeigthHalved);
                // GUILayout.Label(" LOCAL IPv4: ",newLineHeigthHalved);
                // GUILayout.Label(_localIPv4,newLineHeigthHalved);
                GUILayout.EndHorizontal();
            }
            
        }
    }
}
