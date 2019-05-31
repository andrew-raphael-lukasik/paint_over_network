using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.Assertions;


public static class ExtensionMethods_SystemIO
{

    /// <summary> Writes to file in Application.streamingAssetsPath folder </summary>
    public static void WRITE_STREAMING_FILE ( this string thisString , string filename )
    {
        //construct file path:
        string filePath = Application.streamingAssetsPath + ( filename[ 0 ]=='/' ? filename : "/"+filename );

        //create file if it does not exists already:
        if( File.Exists( filePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( filePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write to file:
        File.WriteAllText( filePath , thisString );
    }
    public static void WRITE_STREAMING_FILE ( this byte[] thisBytes , string filename )
    {
        //construct file path:
        string filePath = Application.streamingAssetsPath + ( filename[ 0 ]=='/' ? filename : "/"+filename );

        //create file if it does not exists already:
        if( File.Exists( filePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( filePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write to file:
        File.WriteAllBytes( filePath , thisBytes );
    }

    /// <summary> Writes to file in Application.persistentDataPath folder </summary>
    public static void WRITE_PERSISTENT_FILE ( this string thisString , string filename )
    {
        //construct file path:
        string filePath = Application.persistentDataPath + ( filename[ 0 ]=='/' ? filename : "/"+filename );
        
        //create file:
        if( File.Exists( filePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( filePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write text:
        File.WriteAllText( filePath , thisString );
    }
    public static void WRITE_PERSISTENT_FILE ( this byte[] thisBytes , string filename )
    {
        //construct file path:
        string filePath = Application.persistentDataPath + ( filename[ 0 ]=='/' ? filename : "/"+filename );
        
        //create file:
        if( File.Exists( filePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( filePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write text:
        File.WriteAllBytes( filePath , thisBytes );
    }

    /// <summary> Writes to file in Application.dataPath folder </summary>
    public static void WRITE_DATA_FILE ( this string thisString , string filename )
    {
        //construct file path:
        string filePath = Application.dataPath + ( filename[ 0 ]=='/' ? filename : "/"+filename );
        
        //create file:
        if( File.Exists( filePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( filePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write text:
        File.WriteAllText( filePath , thisString );
    }
    public static void WRITE_DATA_FILE ( this byte[] thisBytes , string filename )
    {
        //construct file path:
        string filePath = Application.dataPath + ( filename[ 0 ]=='/' ? filename : "/"+filename );
        
        //create file:
        if( File.Exists( filePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( filePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write text:
        File.WriteAllBytes( filePath , thisBytes );
    }

    public static void WRITE_FILE ( this string thisString , string fullFilePath )
    {
        //create file:
        if( File.Exists( fullFilePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( fullFilePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write text:
        File.WriteAllText( fullFilePath , thisString );
    }
    public static void WRITE_FILE ( this byte[] thisBytes , string fullFilePath )
    {
        //create file:
        if( File.Exists( fullFilePath )==false )
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText( fullFilePath );
            }
            catch( System.Exception ex ) { throw ex; }
            finally { if( writer!=null ) { writer.Close(); } }
        }

        //write text:
        File.WriteAllBytes( fullFilePath , thisBytes );
    }

    /// <summary> Reads from file in Application.persistentDataPath folder </summary>
    public static string READ_PERSISTENT_FILE ( this string thisFileName )
    {
        string filePath = Application.persistentDataPath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE();
        } else { return null; }
    }
    public static byte[] READ_PERSISTENT_FILE_BYTES ( this string thisFileName )
    {
        string filePath = Application.persistentDataPath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE_BYTES();
        } else { return null; }
    }

    /// <summary> Reads from file in Application.streamingAssetsPath folder </summary>
    public static string READ_STREAMING_FILE ( this string thisFileName )
    {
        string filePath = Application.streamingAssetsPath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE();
        } else { return null; }
    }
    public static byte[] READ_STREAMING_FILE_BYTES ( this string thisFileName )
    {
        string filePath = Application.streamingAssetsPath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE_BYTES();
        } else { return null; }
    }

    /// <summary> Reads from file in Application.dataPath folder </summary>
    public static string READ_DATA_FILE ( this string thisFileName )
    {
        string filePath = Application.dataPath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE();
        } else { return null; }
    }
    public static byte[] READ_DATA_FILE_BYTES ( this string thisFileName )
    {
        string filePath = Application.dataPath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE_BYTES();
        } else { return null; }
    }

    /// <summary> Reads from file in Application.temporaryCachePath folder </summary>
    public static string READ_TEMP_FILE ( this string thisFileName )
    {
        string filePath = Application.temporaryCachePath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE();
        } else { return null; }
    }
    public static byte[] READ_TEMP_FILE_BYTES ( this string thisFileName )
    {
        string filePath = Application.temporaryCachePath + ( thisFileName[ 0 ]=='/' ? thisFileName : '/'+thisFileName );
        if( File.Exists( filePath ) )
        {
            return filePath.READ_FILE_BYTES();
        } else { return null; }
    }

    /// <summary> Reads text file from absolute file path </summary>
    public static string READ_FILE ( this string thisPath )
    {
        #if DEBUG
        if( Path.HasExtension( thisPath )==false ) { throw new System.Exception( $"Invalid file path, NO EXTENSION: { thisPath }" ); }
        #endif
        string result = null;
        FileStream stream = null;
        StreamReader reader = null;
        try
        {
            stream = new FileStream( thisPath , FileMode.Open , FileAccess.Read , FileShare.Read );
            reader = new StreamReader( stream );
            result = reader.ReadToEnd();
        }
        finally
        {
            if( reader!=null ) reader.Close();
            if( stream!=null ) stream.Close();
        }
        return result;
    }
    public static byte[] READ_FILE_BYTES ( this string thisPath )
    {
        #if DEBUG
        if( Path.HasExtension( thisPath )==false ) { throw new System.Exception( $"Invalid file path, NO EXTENSION: { thisPath }" ); }
        #endif
        byte[] result = null;
        FileStream stream = null;
        BinaryReader reader = null;
        try
        {
            stream = new FileStream( thisPath , FileMode.Open , FileAccess.Read , FileShare.Read );
            reader = new BinaryReader( stream );
            result = reader.ReadBytes( int.MaxValue );
        }
        finally
        {
            if( reader!=null ) reader.Close();
            if( stream!=null ) stream.Close();
        }
        return result;
    }

    /// <summary>
    /// Replaces invalid file name characters.
    /// NOTE: file PATH is different
    /// NOTE2: Returns unchanged string reference when all characters are valid
    /// </summary>
    public static string ReplaceInvalidFileNameCharacters ( this string thisFileName  , char replacement = '_' )
    {
        //lazy get:
        if( _invalidFileNameChars==null ){ _invalidFileNameChars = Path.GetInvalidFileNameChars(); }
        
        #if UNITY_EDITOR
        //if( _invalidFileNameChars.Contains( replacement ) ) { throw new System.Exception( $"given replacement char '{ replacement }' is invalid, choose different one" ); }
        #endif
        
        System.Text.StringBuilder sb = null;
        bool nothingChanged = true;
        foreach( char invalidChr in _invalidFileNameChars )
        {
            foreach( var chr in thisFileName )
            {
                if( chr==invalidChr )
                {
                    nothingChanged = false;
                    if( sb==null ) { sb = new System.Text.StringBuilder( thisFileName ); }
                    sb.Replace( chr , replacement );
                }
            }
        }
        return nothingChanged ? thisFileName : sb.ToString();
    }
    static char[] _invalidFileNameChars;

}