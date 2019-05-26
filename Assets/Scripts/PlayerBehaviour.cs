using System.Collections.Generic;
using IO = System.IO;
using System.IO.Compression;

using LZMA = SevenZip.Compression.LZMA.SevenZipHelper;

using UnityEngine;
using UnityEngine.Networking;

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


    [Command(channel=2)]
    public void CmdBufferCreate ( int buffer , int length , string logMessage )
    {
        Debug.LogWarning($"{nameof(CmdBufferCreate)} called");
        UnityEngine.Assertions.Assert.IsFalse( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains '{buffer}' buffer already!" );
        rawDataBuffers.Add( buffer , new byte[length] );
        if( logMessage!=null ) Debug.Log( logMessage );
    }

    [Command(channel=2)]
    public void CmdBufferClose ( int buffer , string logMessage )
    {
        Debug.LogWarning($"{nameof(CmdBufferClose)} called");
        UnityEngine.Assertions.Assert.IsTrue( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains no '{buffer}' buffer" );
        rawDataBuffers.Remove( buffer );
        if( logMessage!=null ) Debug.Log( logMessage );
    }
    
    [Command(channel=2)]
    public void CmdBufferSendBytes ( int buffer , byte[] bytes , int index )
    {
        Debug.LogWarning($"{nameof(CmdBufferSendBytes)} called");
        UnityEngine.Assertions.Assert.IsTrue( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains no '{buffer}' buffer" );
        bytes.CopyTo( rawDataBuffers[buffer] , index );
    }
    
    [Command(channel=2)]
    public void CmdBufferCreateTexture ( int buffer , int width , int height , TextureFormat textureFormat , int mipmapCount )
    {
        Debug.LogWarning($"{nameof(CmdBufferCreateTexture)} called");
        UnityEngine.Assertions.Assert.IsTrue( rawDataBuffers.ContainsKey(buffer) , $"{nameof(rawDataBuffers)} contains no '{buffer}' buffer. Existing buffers: {JsonUtility.ToJson(rawDataBuffers.Keys)}" );
        
        //create texture from bytes:
        Texture2D tex = new Texture2D( width , height , textureFormat , mipmapCount , false );
        tex.LoadRawTextureData( LZMA.Decompress(rawDataBuffers[buffer]) );
        tex.Apply();

        //instantiate scene object:
        var instance = Instantiate( _imagePrefab , FindObjectOfType<ServerBehaviour>().gallery.transform );
        instance.rawImage.texture = tex;
    }

    // public static byte[] Compress ( byte[] uncompressed )
    // {
    //     IO.MemoryStream strm = new IO.MemoryStream();
    //     GZipStream GZipStrem = new GZipStream( strm , CompressionMode.Compress , true );
    //     GZipStrem.Write( uncompressed , 0 , uncompressed.Length );
    //     GZipStrem.Flush();
    //     strm.Flush();
    //     byte[] compressed = strm.GetBuffer();
    //     GZipStrem.Close();
    //     strm.Close();
    //     return compressed;
    // }
    // public static byte[] Decompress ( byte[] compressed )
    // {
    //     IO.MemoryStream memoryStream = new IO.MemoryStream( compressed );
    //     GZipStream GZipStrem = new GZipStream( memoryStream , CompressionMode.Decompress , true );
    //     List<byte> uncompressed = new List<byte>( compressed.Length*2 );
    //     int bytesRead = GZipStrem.ReadByte();
    //     while( bytesRead!=-1 )
    //     {
    //         uncompressed.Add( (byte)bytesRead );
    //         bytesRead = GZipStrem.ReadByte();
    //     }
    //     GZipStrem.Flush(); GZipStrem.Close();
    //     memoryStream.Flush(); memoryStream.Close();
    //     return uncompressed.ToArray();
    // }

    #endregion
}
