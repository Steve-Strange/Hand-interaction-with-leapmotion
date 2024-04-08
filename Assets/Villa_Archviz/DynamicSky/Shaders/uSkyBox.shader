// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// For Unity 5 Only!

Shader "uSky/uSkyBox" 
{
Properties {

[HideInInspector]	_SkyMultiplier("_SkyMultiplier", Vector) = (1, 4, 0.15, 0)
[HideInInspector]	_betaR("BetaR", Vector) = (0.0058, 0.0136, 0.0331, 0)
[HideInInspector]	_betaM("BetaM", Vector) = (0.004, 0.005, 0.006, 0)
[HideInInspector]	_GroundColor ("Ground Color", Vector) = (0.2, 0.6, 1.4, 0)
[HideInInspector]	_SunDir ("Sun Direction", Vector) = (0.321,0.766,-0.557,0)

[HideInInspector]	_NightHorizonColor("_Night Horizon Color", Vector) = (0.43, 0.47, 0.5, 1)
[HideInInspector]	_NightZenithColor("Night Zenith Color", Vector) = (0.00532, 0.00707, .001, 0)
[HideInInspector]	_MoonOuterCorona("Moon Outer Corona Color", Vector) = (0.73, 0.89, 1, 1)
[HideInInspector]	_colorCorrection ("Color Correction", Vector) = (1,1,0,0)

//					_MoonSize("Moon Size", Range(0.01,1.0)) = 0.15
[NoScaleOffset]		_MoonSampler ("Moon",2D) = "black" {}
	
[HideInInspector]	_OuterSpaceIntensity("Outer Space Intensity",Range(0, 2)) = 0.25
[NoScaleOffset]		_OuterSpaceCube("Outer Space Cubemap", Cube) = "black" {}
}
SubShader 
	{
		Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"}
		Cull Off ZWrite Off  
		//Fog { Mode Off }

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		
		struct appdata_t {
			float4 vertex 			: POSITION;
		};
		
		struct v2f 
		{
			float4	pos 			: SV_POSITION;
			float3	worldPos 		: TEXCOORD0;
			half4	zenithAngle		: TEXCOORD1;
			#ifdef NIGHTSKY_ON
			float2	moonTC			: TEXCOORD2;
			half	nightConst		: TEXCOORD3;
			float3	spaceTC			: TEXCOORD4;
			#endif
		};
		
		uniform half 		_SunSize, _OuterSpaceIntensity;
		uniform half2		_colorCorrection;
		uniform half3		_betaR, _betaM, _miePhase_g, _mieConst; 
		uniform half4		_NightZenithColor,_GroundColor;
		uniform float3		_SunDir;
		uniform float4x4 	rotationMatrix;
		
		// x = Sunset, y = Day, z = Night 
		uniform half4		_SkyMultiplier;
		#ifdef NIGHTSKY_ON
		sampler2D			_MoonSampler;
		samplerCUBE			_OuterSpaceCube;
		uniform half4		_NightHorizonColor, _MoonInnerCorona, _MoonOuterCorona;
		uniform float4x4	_Moon_wtl;
		uniform float		_MoonSize;
		#endif
		
		v2f vert(appdata_t v)
		{
			v2f OUT;
			OUT.pos = UnityObjectToClipPos(v.vertex);
			OUT.worldPos = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));

			float t = OUT.worldPos.y;
			OUT.zenithAngle.xyz = max(6e-2, t + 6e-2) + (max(0.0, -t ) * _GroundColor.xyz);
			OUT.zenithAngle.w = max(0, t);
			
			#ifdef NIGHTSKY_ON
			float3 right =	_Moon_wtl[0].xyz;
			float3 up =		_Moon_wtl[1].xyz;			
			OUT.moonTC = float2( dot( right, v.vertex.xyz), dot( up, v.vertex.xyz) )/_MoonSize + 0.5;
			OUT.nightConst = OUT.zenithAngle.w * _SkyMultiplier.z ; 
			OUT.spaceTC = mul((float3x3)rotationMatrix,v.vertex.xyz);
			#endif
			return OUT;
		}
			
		half4 frag(v2f IN) : SV_Target
		{
		    float3 pos = normalize( IN.worldPos );
			float cosTheta = dot( pos, _SunDir); // need precision float in iOS
			half cosine = cosTheta;
			
			// optical depth
			half3 sR = 8.0 / IN.zenithAngle.xyz ;
			half3 sM = 1.2 / IN.zenithAngle.xyz ;
			
			// gradient
			half3 gr = _NightZenithColor.xyz * sR;
			gr *= (2 - gr);
			
			// sky color
			half3 extinction = exp(-( _betaR * sR + _betaM * sM ));
			
			half3 rayleigh = lerp( extinction * gr, 1 - extinction, _SkyMultiplier.x );
			half3 mie = rayleigh * sM / rayleigh.r * _mieConst * sign(_LightColor0.w);

			// scattering phase
			half miePhase = _miePhase_g.x * pow( _miePhase_g.y - _miePhase_g.z * cosine, -1.5 );

			half3 inScatter = ( rayleigh * 0.75 + mie * miePhase ) * (( 1.0 + cosine * cosine ) * _SkyMultiplier.y);
			
			// add sun
			if ( _SunDir.y > -0.1 ){
			half sun = min(1e3, pow((1-cosine)* _SunSize, -1.5 ));
			inScatter += sun * min(mie,IN.zenithAngle.w)* extinction ;
			}
// --------------------------------------------------------------------------------
			// night sky

			half moonMask = 0.0;
			#ifdef NIGHTSKY_ON
			if ( _SunDir.y < 0.25 ){
				// night horizontal gradient
				inScatter += _NightHorizonColor.xyz * gr;
				
				// add moon and outer space
				half4 moonAlbedo = tex2D( _MoonSampler, IN.moonTC.xy );
				moonMask = moonAlbedo.a;
				
				half4 spaceAlbedo = texCUBE (_OuterSpaceCube, IN.spaceTC) * (1 - moonMask) * _OuterSpaceIntensity;
				inScatter += (moonAlbedo.rgb * sign(_LightColor0.w) + spaceAlbedo.rgb )* IN.nightConst ;
				
				// add inner and outer moon corona
				float moonDir = 1 + dot( pos, _Moon_wtl[2].xyz);
				half m = moonDir;
				inScatter += _MoonInnerCorona.xyz * (1.0 / (1.05 + m * _MoonInnerCorona.w));
				inScatter += _MoonOuterCorona.xyz * (1.0 / (1.05 + m * _MoonOuterCorona.w));
			}
			#endif
// --------------------------------------------------------------------------------		

			// tonemapping
			#ifndef USKY_HDR_ON
			inScatter = 1 - exp(-1 * inScatter);															// clear sky	(4 ALU)
//			inScatter = inScatter < 1.413 ? pow(inScatter * 0.38317, 1.0 / 2.2) : 1.0 - exp(-inScatter);	// standard sky	(10 AlU)
			#endif
			
			// color correction
			inScatter = pow(inScatter * _colorCorrection.x, _colorCorrection.y);
			
			// "max 1e-3" to avoid the real time reflection probe render with black issue
			half alpha = lerp( max(1e-3,moonMask), 1.0, _SkyMultiplier.x);
			
			return half4(inScatter, alpha); 

		}

		ENDCG
// --------------------------------------------------------------------------------			
	Pass{	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile NIGHTSKY_ON NIGHTSKY_OFF
			#pragma multi_compile USKY_HDR_ON USKY_HDR_OFF
			#pragma target 3.0
			ENDCG
    	}
	}
	
	Fallback Off
}