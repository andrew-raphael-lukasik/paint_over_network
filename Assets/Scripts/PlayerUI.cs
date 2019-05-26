using System.Collections.Generic;
using System.Threading.Tasks;

using LZMA = SevenZip.Compression.LZMA.SevenZipHelper;

using UnityEngine;
using UnityEngine.Networking;

public class PlayerUI : NetworkBehaviour
{
    
    [SerializeField] FreeDraw.Drawable _drawable = null;

    [Client]
    public async void SendTextureToServer ()
    {
        var localPlayer = PlayerBehaviour.localPlayer;
        var texture = _drawable.texture;
        byte[] bytes = LZMA.Compress( texture.GetRawTextureData() );
        int buffer = System.DateTime.Now.GetHashCode();
        int byteLimit = 1300;//63800; //channel limit
        int bytesLength = bytes.Length;
        int fullBatches = bytesLength / byteLimit;
        int finalBatchLength = bytesLength % byteLimit;
        var connectionToServer = PlayerBehaviour.localPlayer.connectionToServer;
        localPlayer.CmdBufferCreate( buffer , bytesLength , $"Transfer #{buffer} of {bytesLength} bytes in {fullBatches+(finalBatchLength!=0?1:0)} batches started" );
        {
            for( int batch=0 ; batch<=fullBatches ; batch++ )
            {
                int batchOffset = batch*byteLimit;
                int batchLength = batch!=fullBatches ? byteLimit : finalBatchLength;
                byte[] batchBytes = new byte[ batchLength ];
                System.Array.Copy( bytes , batchOffset , batchBytes , 0 , batchLength );
                
                bool sent = false;
                while( sent==false )
                {
                    try
                    {
                        localPlayer.CmdBufferSendBytes( buffer , batchBytes , batchOffset );
                        sent = true;
                    }
                    catch( System.Exception ex )
                    {
                        sent = false;
                        Debug.LogException(ex);
                        await Task.Delay( 50 );
                    }
                    await Task.Delay( 5 );
                }
            }
        }
        localPlayer.CmdBufferCreateTexture( buffer , texture.width , texture.height , texture.format , texture.mipmapCount );
        localPlayer.CmdBufferClose( buffer , $"Transfer #{buffer} closed" );
    }

}
