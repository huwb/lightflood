Shader "Unlit/WallShader"
{
	Properties
	{
		_MainTex( "Texture", 2D ) = "white" {}
		_WallColour( "Colour", Color ) = (0.,0.,0.,0.)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float3 _WallColour;
			float _texRes = 64.;
			float _exposure = 0.5;
			float _contrast = 1.0;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float3 ACESFilm( float3 x )
			{
				float a = 2.51f;
				float b = 0.03f;
				float c = 2.43f;
				float d = 0.59f;
				float e = 0.14f;
				return saturate( (x*(a*x + b)) / (x*(c*x + d) + e) );
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				//float3 dd = float3(1. / _texRes, 0., -1. / _texRes);
				//col += tex2D( _MainTex, i.uv + dd.xy );
				//col += tex2D( _MainTex, i.uv - dd.xy );
				//col += tex2D( _MainTex, i.uv + dd.yx );
				//col += tex2D( _MainTex, i.uv - dd.yx );
				//col /= 5.;

				col.rgb *= _WallColour;

				if( _exposure == 0. ) _exposure = 0.5;

				col.rgb = log( 1 + col.rgb ) * _exposure;
				col.rgb = lerp( col.rgb, smoothstep( 0., 1., col.rgb ), _contrast );

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
