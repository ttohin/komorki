Shader "Custom/KomorkaShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 color: Color;
		};

		fixed4 _Color;

    void vert(inout appdata_full v) 
		{
			half waveValue = step(v.color.r, 0.1);
			
      float3 pos = v.vertex.xyz;
			v.vertex.x += sin (pos.y * 4 + _Time * 50) * 0.1 * waveValue;
			v.vertex.y += sin (pos.x * 4 + _Time * 50) * 0.1 * waveValue;
    }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
