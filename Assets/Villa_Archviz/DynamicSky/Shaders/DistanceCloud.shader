// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "uSky/DistanceCloud" {
Properties {

    Attenuation ("Attenuation", Range(0,5)) = 0.6
    StepSize ("Step size", Range(0.001,0.01)) = 0.004
    AlphaSaturation("Alpha saturation", Range(1,10)) = 2.0
    SunColorMultiplier ("Sun Color multiplier", Range(0,8)) = 4
    SkyColorMultiplier("Sky Color multiplier", Range(0,8)) = 1.5
	[KeywordEnum(Rectangular,Polar)] USKY_MAPPING ("Mapping mode", Float) = 0	
	CloudSampler ("Texture (R)", 2D) = "white" {}
	
	// "Opacity mask" is for blocking the color behind the cloud, useful at night time.
	// if using "Opacity mask", then requires cloud texture's channel "g" for masking.
	// if not using "Opacity mask", then set the "Opacity mask" to 0.0 will gain some performance.
	Mask ("Opacity mask (G)", Range (0,1)) = 0.0
	// Range 0 ~ 360 for non-animated Rotation
    RotateSpeed("Rotate speed", Range (-1,1)) = 0.0 
//	[Toggle] USKY_MASK ("Use Opacity mask?", Float) = 0

// make sure following value store in the material if it is not focusing
[HideInInspector] ShadeColorFromSun ("ShadeColorFromSun",Vector) = (1,0.87,0.73,1)
[HideInInspector] ShadeColorFromSky ("ShadeColorFromSky",Vector) = (0.58,0.7,0.86,1)
}
SubShader {
Tags { "Queue"="Geometry+501" "RenderType"="Background" }

Blend  SrcAlpha OneMinusSrcAlpha

Zwrite Off  

Pass {
		Name "BASE"
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
//		#include "Lighting.cginc"
		#pragma multi_compile USKY_MAPPING_RECTANGULAR USKY_MAPPING_POLAR
		
		sampler2D CloudSampler;
		uniform float3 _SunDir; 
		float RotateSpeed;
		half Attenuation, StepSize, AlphaSaturation, SunColorMultiplier, SkyColorMultiplier, Mask;
		uniform half2 _colorCorrection;
		uniform half3 ShadeColorFromSun, ShadeColorFromSky;
		float4 CloudSampler_ST;

		struct appdata_t {
			float4	vertex		: POSITION;
    		float2  rectangular	: TEXCOORD0; 
    		float2  polar		: TEXCOORD1; 
		};
		
		struct v2f {
		    float4	pos 	: SV_POSITION;
		    half2	baseTC	: TEXCOORD0;
		};

		float3 RotateAroundYInDegrees (float3 vertex, float degrees)
		{
			float alpha = degrees * 3.1416 / 180.0;
			float sina, cosa;
			sincos(alpha, sina, cosa);
			float2x2 m = float2x2(cosa, -sina, sina, cosa);
			return float3(mul(m, vertex.xz), vertex.y).xzy;
		}
		
		v2f vert (appdata_t v)
		{
		    v2f o;

		    float3 t = RotateAroundYInDegrees(v.vertex.xyz, RotateSpeed *_Time.y+ unity_DeltaTime.z).xyz; //  animate rotation 
//			t.y = pow(v.vertex.y, 0.85 );
		    t = t * _ProjectionParams.z + _WorldSpaceCameraPos.xyz ; //  camera’s far plane
//			t = t + _WorldSpaceCameraPos.xyz ; 
			
			o.pos = UnityObjectToClipPos (float4(t,1));
			o.pos.z = o.pos.w; // avoid front face clipping?

			#ifdef USKY_MAPPING_RECTANGULAR
				o.baseTC = TRANSFORM_TEX (v.rectangular, CloudSampler);
			#else
				o.baseTC = v.polar ;
			#endif
		    return o;
		}

		half4 frag (v2f i) : SV_Target
		{
			const int c_numSamples = 6; // 6 to keep in SM2.0, default is 8

			half2 sampleDir = half2(0, StepSize); // Upward // TODO: Polar mapping need different sample direction
			half2 uv = i.baseTC.xy;
			
			// r = cloud density , g = Opacity mask
			half2 opacity = tex2D( CloudSampler, uv ).rg; 
			
			// Alternative for better performance: just tweak the Mask in texture channel "G" directly,
			// and read the opacity.g directly without using "lerp function" between 2 channels.
			// so alpha will be:  a = pow( opacity.g, AlphaSaturation );
			if (Mask > 0.02)
				opacity.r = lerp(opacity.r, opacity.g, Mask); 

			
			half density = 0;
			half2 sampleUV = (half2)0;  
			
			for( int i = 0; i < c_numSamples; i++ )
			{
				sampleUV = uv + i * sampleDir;
				half t = tex2D( CloudSampler, sampleUV ).r ;
				density += t ;
			}
//			density *= 2 / c_numSamples;
			
			half c = exp2( -Attenuation * density);
			half a = pow( opacity.r, AlphaSaturation );
			half3 col = lerp( SkyColorMultiplier * ShadeColorFromSky.xyz, SunColorMultiplier * ShadeColorFromSun.xyz, c );
			
			// need color correction for linear
//			half3 col = lerp( SkyColorMultiplier * unity_AmbientSky.xyz, SunColorMultiplier * _LightColor0.rgb, c );

			
			half4 Color = half4( col, a ) ;
			
			return Color;

		}
		ENDCG

    }
}
Fallback "uSky/DistanceCloud Cheap"
} 