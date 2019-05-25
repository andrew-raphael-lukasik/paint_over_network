using UnityEngine;
using UnityEngine.Networking;

public class PlayerUI : NetworkBehaviour
{
    
    [SerializeField] FreeDraw.Drawable _drawable = null;
    [SerializeField] SpriteObject _imagePrefab = null;

    [Client]
    public void SendTextureToServer ()
    {
        var texture = _drawable.texture;
        CmdSendTextureToServer( texture.GetRawTextureData() , texture.width , texture.height );
    }
    
    [Command(channel=2)]
    public void CmdSendTextureToServer ( byte[] rawTextureData , int width , int height )
    {
        //create texture from bytes:
        Texture2D tex = new Texture2D( width , height , TextureFormat.RGBA32 , 3 , false );
        tex.LoadRawTextureData( rawTextureData );
        tex.Apply();

        //instantiate scene object:
        var instance = Instantiate( _imagePrefab , transform );
        instance.rawImage.texture = tex;
    }

}
