using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using Unity.Mathematics;

//TODO: brakuje linijki na samej górze tekstury dla parzystych wymiarów
//TODO: wynik z wersji async wygląda jakby była po efekcie sharpen

public static class ExtensionMethods_Texture
{
	
	/// <summary> Inserts pixels from another texture. </summary>
	public static void SetPixels ( this Texture2D thisTexture , RectInt targetArea , Texture2D source )
	{
		try
		{
			Texture2D sourceResized = thisTexture.ScaleTextureCopy( targetArea.width , targetArea.height , true );
			Color[] pixels = sourceResized.GetPixels();
			UnityEngine.Object.Destroy( sourceResized );
			thisTexture.SetPixels( targetArea , pixels );
		}
		catch( System.Exception ex ) { Debug.LogException( ex ); }
	}
	// public static async Task SetPixelsAsync ( this Texture2D thisTexture , RectInt targetArea , Texture2D source , bool linear = false )
	// {
	// 	try
	// 	{
	// 		Texture2D sourceResized = new Texture2D( source.width , source.height , source.format , false , linear );
	// 		sourceResized.wrapMode = source.wrapMode;
	// 		sourceResized.filterMode = source.filterMode;
	// 		sourceResized.SetPixels( source.GetPixels() );
	// 		await ScaleTextureAsync( sourceResized , targetArea.width , targetArea.height );
	// 		Color[] pixels = sourceResized.GetPixels();
	// 		UnityEngine.Object.Destroy( sourceResized );
	// 		thisTexture.SetPixels( targetArea , pixels );
	// 	}
	// 	catch( System.Exception ex ) { Debug.LogException( ex ); }
	// }
	public static void SetPixels ( this Texture2D thisTexture , RectInt targetArea , Color fillColor )
	{
		Color[] pixels = new Color[ targetArea.width * targetArea.height ];
		for( int i=pixels.Length-1 ; i!=-1 ; i-- ){ pixels[i] = fillColor; }
		thisTexture.SetPixels( targetArea , pixels );
	}
	public static void SetPixels ( this Texture2D thisTexture , RectInt rect , Color[] pixels )
	{
		thisTexture.SetPixels( rect.x , rect.y , rect.width , rect.height , pixels );
	}

	/// <summary> Unlike Texture2D.Resize this will actually create properly resized texture. </summary>
	public static void ScaleTexture ( this Texture2D thisTexture , int width , int height )
	{
		Color[] pixels = new Color[ width * height ];
		float fwidth = (float)width;
		float stepX = 1f / fwidth;
		float stepY = 1f / (float)height;
		for( int i=pixels.Length-1 ; i!=-1 ; i-- )
		{
			float fi = (float)i;
			pixels[ i ] = thisTexture.GetPixelBilinear(
				stepX * ( fi % fwidth ) ,
				stepY * ( math.floor( fi / fwidth ) )
			);
		}
		thisTexture.Resize( width , height );
		thisTexture.SetPixels( pixels ); 
		thisTexture.Apply();
	}
	public static Texture2D ScaleTextureCopy ( this Texture2D source , int width , int height , bool linear )
	{
		Texture2D result = new Texture2D( width , height , source.format , source.mipmapCount!=0 , linear );
		result.wrapMode = source.wrapMode;
		result.filterMode = source.filterMode;
		Color[] pixels = result.GetPixels( 0 );
		float fwidth = (float)width;
		float stepX = 1f / fwidth;
		float stepY = 1f / (float)height;
		int numPixels = pixels.Length;
		for( int i=0 ; i<numPixels ; i++ )
		{
			float fi = (float)i;
			pixels[ i ] = source.GetPixelBilinear(
				stepX * ( fi % fwidth ) ,
				stepY * math.floor( fi / fwidth )
			);
		}
		result.SetPixels( pixels , 0 );
		result.Apply();
		return result;
	}
	
	public static Texture2D ScaleTextureCopy8bit ( this Texture2D source , int width , int height , bool linear )
	{
		RenderTexture rt = RenderTexture.GetTemporary( width , height );
		RenderTexture.active = rt;
		Texture2D copy;
		{
			Graphics.Blit( source , rt );
			copy = new Texture2D( width , height , source.format , source.mipmapCount!=0 , linear );
			copy.wrapMode = source.wrapMode;
			copy.filterMode = source.filterMode;
			copy.ReadPixels( new Rect(0,0,width,height) , 0 , 0 );
			copy.Apply();
		}
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary( rt );
		return copy;
	}
	public static void ScaleTexture8bit ( this Texture2D thisTexture , int width , int height , bool linear )
	{
		RenderTexture rt = RenderTexture.GetTemporary( width , height );
		RenderTexture.active = rt;
		{
			Graphics.Blit( thisTexture , rt );
			thisTexture.Resize( width , height , thisTexture.format , false );
			thisTexture.ReadPixels( new Rect(0,0,width,height) , 0 , 0 );
			thisTexture.Apply();
		}
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary( rt );
	}

	[System.Obsolete("Output texture doesn't look good (like sharpened and missing top line)")]
	public static async Task ScaleTextureAsync ( this Texture2D thisTexture , int width , int height )
	{
		await TextureScale.Bilinear( thisTexture , width , height );
	}


	#region improved http://wiki.unity3d.com/index.php/TextureScale
	public static class TextureScale
	{
		public class ThreadData
		{
			public int start;
			public int end;
			public ThreadData (int s, int e) {
				start = s;
				end = e;
			}
		}
		public class ShaderJobData : System.IDisposable
		{
			public Color[] texColors;
			public Color[] newColors;
			public int w;
			public float ratioX;
			public float ratioY;
			public int w2;
			void System.IDisposable.Dispose()
			{
				this.texColors = null;
				this.newColors = null;
			}
		}

		public static async Task Point ( Texture2D texture , int newWidth , int newHeight )
		{
			await ThreadedScale( texture , newWidth , newHeight , false );
		}
	
		public static async Task Bilinear ( Texture2D tex , int newWidth , int newHeight )
		{
			await ThreadedScale( tex , newWidth , newHeight , true );
		}
	
		static async Task ThreadedScale ( Texture2D texture , int newWidth , int newHeight , bool useBilinear )
		{
			//early check:
			if( texture.width==newWidth && texture.height==newHeight )
			{
				//unnecessary runs proved to produce invalid textures, so let's not do that
				return;
			}

			//
			using( ShaderJobData jobData = new ShaderJobData() )
			{
				try
				{
					jobData.texColors = texture.GetPixels();
					jobData.newColors = new Color[newWidth * newHeight];
					if( useBilinear )
					{
						jobData.ratioX = 1f / ((float)newWidth / (texture.width-1));
						jobData.ratioY = 1f / ((float)newHeight / (texture.height-1));
					}
					else
					{
						jobData.ratioX = ((float)texture.width) / newWidth;
						jobData.ratioY = ((float)texture.height) / newHeight;
					}
					jobData.w = texture.width;
					jobData.w2 = newWidth;
					var cores = math.min( SystemInfo.processorCount , newHeight );
					var slice = newHeight/cores;
			
					if( cores>1 )
					{
						int i=0;

						//run tasks on other threads:
						Task[] tasks = new Task[ cores ];
						for( i=0 ; i<cores ; i++ )
						{
							ThreadData threadData = new ThreadData(slice * i, slice * (i + 1));
							tasks[i] = (
								Task.Run( ()=>{
									try
									{
										if( useBilinear ) { BilinearScale( threadData , jobData ); }
										else { PointScale( threadData , jobData ); }
									}
									catch( System.Exception ex ) { Debug.LogException( ex ); }
								} )
							);
						}
						
						//wait until all other tasks completed:
						await Task.WhenAll( tasks );
					}
					else
					{
						ThreadData threadData = new ThreadData( 0 , newHeight );
						if( useBilinear ) { BilinearScale( threadData , jobData ); }
						else { PointScale( threadData , jobData ); }
					}
					
					texture.Resize( newWidth , newHeight );
					texture.SetPixels( jobData.newColors );
					texture.Apply();
				}
				catch ( System.Exception ex )
				{
					Debug.LogException( ex );
				}
			}
		}
	
		public static void BilinearScale ( ThreadData threadData , ShaderJobData jobData )
		{
			for( var y = threadData.start ; y<threadData.end ; y++ )
			{
				int yFloor = (int)math.floor( y * jobData.ratioY );
				var y1 = yFloor * jobData.w;
				var y2 = ( yFloor + 1 ) * jobData.w;
				var yw = y * jobData.w2;
	
				for( var x=0 ; x<jobData.w2 ; x++ )
				{
					int xFloor = (int)math.floor( x * jobData.ratioX );
					var xLerp = x * jobData.ratioX-xFloor;
					jobData.newColors[yw + x] = ColorLerpUnclamped(
						ColorLerpUnclamped(
							jobData.texColors[ y1 + xFloor ] ,
							jobData.texColors[ y1 + xFloor + 1 ] ,
							xLerp
						) ,
						ColorLerpUnclamped(
							jobData.texColors[ y2 + xFloor ] ,
							jobData.texColors[ y2 + xFloor + 1 ] ,
							xLerp
						) ,
						y * jobData.ratioY - yFloor
					);
				}
			}
		}
	
		public static void PointScale ( ThreadData threadData , ShaderJobData jobData )
		{
			for( var y=threadData.start ; y<threadData.end ; y++ )
			{
				var thisY = (int)(jobData.ratioY * y) * jobData.w;
				var yw = y * jobData.w2;
				for( var x=0 ; x<jobData.w2 ; x++ )
				{
					jobData.newColors[ yw + x ] = jobData.texColors[ (int)( thisY + jobData.ratioX * x ) ];
				}
			}
		}
	
		static Color ColorLerpUnclamped ( Color a , Color b , float value )
		{
			float4 fa = new float4{ x = a.r , y = a.g , z = a.b , w = a.a };
			float4 fb = new float4{ x = b.r , y = b.g , z = b.b , w = b.a };
			float4 f = fa + (fb * value);
			return new Color{ r = f.x , g = f.y , b = f.z , a = f.w };
		}

	}
	#endregion
	
}
