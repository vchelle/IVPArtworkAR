Shader "Orb/OrbShader" {
	                                                                                                                          
	Properties {
		[HideInInspector]_Core("Core", Range(0.0, 1.0)) = 0.5
		[HideInInspector]_GlowThickness("Glow Thickness", Range(0.0, 1.0)) = 0.5
		[HideInInspector]_Color("Core Color", Color) = (1, 1, 1, 1)
		[HideInInspector]_EdgeColor("Edge Color", Color) = (0, 0, 0, 1)
		[HideInInspector]_GlowColor("Glow Color", Color) = (1, 1, 1, 1)
		[HideInInspector]_NoiseColor("Noise Color", Color) = (0, 0, 0, 1)
		[HideInInspector]_Animation("Noise Animation", Vector) = (0.0, -3.0, 0.5, 0.0)
		[HideInInspector]_Amount("Noise Amount", Float) = 10
		[HideInInspector]_Amplitude("Noise Amplitude", Range(0.0, 1.0)) = 0.3
		[HideInInspector]_AlphaTestCutoff("Alpha cutoff", Float) = 0.5
	}

	/** Orb shader **/

	SubShader {
		
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		/* Pass 1: Render the core */

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
			
			#pragma surface surf Standard fullforwardshadows alphatest:_AlphaTestCutoff vertex:vert
			#pragma target 3.0

			#include "OrbNoise.cginc"

			/** INPUT STRUCTURE **/

			struct Input {
				float3 viewDir;
				float3 localPos;
				float3 worldPos;
			};

			/*** VERTEX SHADER ***/

			void vert(inout appdata_full input, out Input surfaceInput) {
				UNITY_INITIALIZE_OUTPUT(Input, surfaceInput);
				surfaceInput.localPos = input.vertex.xyz;
			}

			/*** SURFACE SHADER **/
			
			fixed4 _Center;
			fixed _Radius;

			fixed _GlowThickness;
			fixed _Core;

			fixed4 _Color;
			fixed4 _GlowColor;
			fixed4 _EdgeColor;
			fixed4 _NoiseColor;

			fixed3 _Animation;
			fixed _Amount;
			fixed _Amplitude;

			void surf(Input input, inout SurfaceOutputStandard output) {
				float a = dot(normalize(input.worldPos - _WorldSpaceCameraPos), _WorldSpaceCameraPos - _Center);
				float aa = pow(a, 2);
				float b = length(_WorldSpaceCameraPos - _Center);
				float bb = pow(b, 2);

				// Calculate noise
				float noise = saturate(abs(turbulence(input.localPos * _Amount + _Animation * _Time[1]) * _Amplitude * 0.1));

				float adjustedRadius = _Radius * lerp(_Core, 1.0, noise);
				float inner = sqrt(aa - bb + pow(adjustedRadius, 2));

				if (inner > 0) {
					output.Alpha = 1;
				} else {
					output.Alpha = 0;
				}

				output.Emission = lerp(lerp(_EdgeColor.rgb, _Color.rgb, pow(inner / adjustedRadius, 2)), _NoiseColor.rgb, noise);
			}

		ENDCG

		/* Pass 2: Render the glow */

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

			#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert
			#pragma target 3.0

			#include "OrbNoise.cginc"

			/** INPUT STRUCTURE **/

			struct Input {
				float3 viewDir;
				float3 localPos;
				float3 worldPos;
			};

			/*** VERTEX SHADER ***/

			void vert(inout appdata_full input, out Input surfaceInput) {
				UNITY_INITIALIZE_OUTPUT(Input, surfaceInput);
				surfaceInput.localPos = input.vertex.xyz;
			}

			/*** SURFACE SHADER **/

			fixed4 _Center;
			fixed _Radius;

			fixed _Core;
			fixed _GlowThickness;

			fixed4 _Color;
			fixed4 _GlowColor;

			fixed3 _Animation;
			fixed _Amount;
			fixed _Amplitude;

			void surf(Input input, inout SurfaceOutputStandard output) {
				float a = dot(normalize(input.worldPos - _WorldSpaceCameraPos), _WorldSpaceCameraPos - _Center);
				float aa = pow(a, 2);
				float b = length(_WorldSpaceCameraPos - _Center);
				float bb = pow(b, 2);

				// Calculate noise
				float noise = saturate(abs(turbulence(input.localPos * _Amount + _Animation * _Time[1]) * _Amplitude * 0.1));

				float adjustedRadius = _Radius * lerp(_Core, 1.0, noise);

				float inner = sqrt(aa - bb + pow(adjustedRadius, 2));

				if (inner > 0) {
					output.Alpha = 0;
				} else {
					if (adjustedRadius < _Radius) {
						float outer = sqrt(aa - bb + pow(_Radius, 2));
						float maxDistance = sqrt(pow(_Radius, 2) - pow(adjustedRadius, 2));
						output.Alpha = pow(outer / maxDistance, 3) * _GlowThickness;
					} else {
						output.Alpha = 1;
					}
				}

				output.Emission = _GlowColor.rgb;
			}

		ENDCG

	}

	/** Fallback shader **/

	Fallback "Diffuse"

}