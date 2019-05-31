using System.Collections.Generic;
using IO = System.IO;
using System.IO.Compression;

using LZMA = SevenZip.Compression.LZMA.SevenZipHelper;

using UnityEngine;

using Mirror;

public class PlayerBehaviour : NetworkBehaviour
{
    #region FIELDS & PROPERTIES


    static PlayerBehaviour _localPlayer;
    public static PlayerBehaviour localPlayer => _localPlayer;

    [SerializeField] PlayerUI _uiPrefab = null;
    PlayerUI _uiInstance;

    [SerializeField] SpriteObject _imagePrefab = null;

    Dictionary<int,byte[]> rawDataBuffers = new Dictionary<int,byte[]>();


    #endregion
    #region NETWORK BEHAVIOUR METHODS

    public override void OnStartLocalPlayer ()
    {
        _localPlayer = this;
        _uiInstance = Instantiate( _uiPrefab , transform );
    }


    #endregion
    #region PUBLIC METHODS


    [Command]
    public void CmdBufferCreate ( int buffer , int length , string logMessage )
    {
        #if DEBUG
        //Debug.Log($"{nameof(CmdBufferCreate)} called");
        UnityEngine.Assertions.Assert.IsFalse( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains '{buffer}' buffer already!" );
        #endif

        rawDataBuffers.Add( buffer , new byte[length] );
        if( logMessage!=null ) Debug.Log( logMessage );
    }

    [Command]
    public void CmdBufferClose ( int buffer , string logMessage )
    {
        #if DEBUG
        //Debug.Log($"{nameof(CmdBufferClose)} called");
        UnityEngine.Assertions.Assert.IsTrue( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains no '{buffer}' buffer" );
        #endif

        rawDataBuffers.Remove( buffer );
        if( logMessage!=null ) Debug.Log( logMessage );
    }
    
    [Command]
    public void CmdBufferSendBytes ( int buffer , byte[] bytes , int index )
    {
        #if DEBUG
        //Debug.Log($"{nameof(CmdBufferSendBytes)} called");
        UnityEngine.Assertions.Assert.IsTrue( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains no '{buffer}' buffer" );
        #endif
        
        bytes.CopyTo( rawDataBuffers[buffer] , index );
    }
    
    [Command]
    public void CmdBufferCreateTexture ( int buffer , int width , int height , TextureFormat textureFormat , int mipmapCount )
    {
        #if DEBUG
        //Debug.Log($"{nameof(CmdBufferCreateTexture)} called");
        UnityEngine.Assertions.Assert.IsTrue( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains no '{buffer}' buffer. Existing buffers: {JsonUtility.ToJson(rawDataBuffers.Keys)}" );
        #endif

        //create texture from bytes:
        Texture2D tex = new Texture2D( width , height , textureFormat , mipmapCount , false );
        tex.LoadRawTextureData( LZMA.Decompress(rawDataBuffers[buffer]) );
        tex.Apply();

        //save texture to file:
        tex.EncodeToPNG().WRITE_FILE(
            IO.Path.Combine( IO.Path.GetDirectoryName( Application.dataPath ) , $"{buffer}.png" )
        );

        //downsize texture to save memory:
        tex.ScaleTexture8bit( 256 , 256 , false );

        //instantiate scene object:
        var instance = Instantiate( _imagePrefab , FindObjectOfType<ServerBehaviour>().gallery.transform );
        instance.rawImage.texture = tex;
    }

    #endregion
}
