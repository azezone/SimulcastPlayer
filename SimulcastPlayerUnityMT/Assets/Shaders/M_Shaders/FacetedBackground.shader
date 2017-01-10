Shader "Custom/FacetedBackground" {
Properties {

	_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
			
	_BackColor ("Back Color", Color) = (0,0,0,1)
	_EmissionColor ("Emission Color", Color) = (0,0,0,1)

	_Amplitude("Amplitude", float) = 1
	_Freq("Freq", float) = 1
	
	_RippleData ("Ripple Data", Vector) = (0,0,0,0)
	_ProjectionCentre ("Projection Centre", Vector) = (0,0,0,0)	

	_GapToSecondRipple ("Gap To Second Ripple", float) = 2
	
	_OuterRadius1 ("Outer Radius 1", float) = 1
	_OuterRadius2 ("Outer Radius 2", float) = 5
			
	_ProjectionAlpha ("ProjectionAlpha", float) = 1
	
	_Scale ("Scale", float) = 0
	_AspectRatio ("AspectRatio", float) = 1
	
	_HFeather ("H Feather", float) = 1
	_VFeather ("V Feather", float) = 1
	
	_FlattenAmount ("_FlattenAmount", Range (0.0, 1.0)) = 0
	
	_ScreenPos ("Screen Depth", float) = 9.0
	_SnapThreshold ("Snap Threshold", float) = 0.2

	_SnapAmount("Snap Amount", Range (0.0, 1.0)) = 0
	_ScreenWidth("Screen Width", float) = 10
	_ScreenHeight("Screen Height", float) = 6
					
	_ColorFade("Color Fade", Range (0.0, 1.0)) = 0
	
	_DarkenRadius("Darken Radius", float) = 1
	
	_DarkenAmount("Darken Amount", float) = 0.5
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	
	Cull Front
	CGPROGRAM
	#pragma surface surf Lambert vertex:vert
	#pragma target 3.0
		
	sampler2D _MainTex;
		
	fixed4 _EmissionColor;
	fixed4 _BackColor;
	
	float _Amplitude;
	float _Freq;
	
	float4 _RippleData;
	
	float4 _ProjectionCentre;

	float _GapToSecondRipple;

	float _OuterRadius1;
	float _OuterRadius2;
	
	float _ProjectionAlpha;
	float _Scale;
	float _AspectRatio;
	
	float _HFeather;
	float _VFeather;
	float _FlattenAmount;
	float _ScreenPos;
	
	float _ColorFade;
	float _SnapThreshold;

	float _ScreenWidth;
	float _ScreenHeight;
	
	float _SnapAmount;
	float _DarkenRadius;
	float _DarkenAmount;
	
	struct Input {
		float3 pos;
		float2 texcoord;
		float texAlpha;
		float inside;
	};

	const float radius = 10;
	const fixed3 projectionColor = fixed3(1.5, 1.5, 1.5);
	const float2 centrePoint = float2(0.5, 0.5);
	

	const float screenDepth = 11.0;

	
	float ripple(float3 centre, float3 v){
	
		// straight line distance
		float distFromCentre = length(v - centre);
		
		// arc length
		float innerAngle = acos((distFromCentre*0.5) / radius)* 2.0;
    	float arcLength = radius * innerAngle ;

    	
    	float outerFalloff = smoothstep(_OuterRadius2, _OuterRadius1, distFromCentre);
		
		// single ripple
	 	float n1 = (2 + distFromCentre - _RippleData.w) * _Freq;
	 	float y1 = n1 * exp(-pow(n1, 2));
    	
    	float n2 = (2 + distFromCentre - (_RippleData.w-_GapToSecondRipple)) * _Freq;
	 	float y2 = n2 * exp(-pow(n2, 2));
	 	
    	float y = y1+y2;
    	
    	return y * _Amplitude * outerFalloff;
	}


	float pointWithinBounds(float3 pnt, float3 corner1, float3 corner2){

		float3 comp1 = step(corner1, pnt);
		float3 comp2 = step(pnt, corner2);
	
		return comp1.x * comp1.y * comp1.z * comp2.x * comp2.y * comp2.z;
	}

	float linearstep(float edge0, float edge1, float val)
	{
		float r = edge1 - edge0; //5 - 10 = -5
		float pos = val - edge0; //8 - 10 = -2

		return min(1.0, max(0.0, pos/r));
	}
		
	float distort (inout float3 v){
	
		
		float rippleAmount = ripple(_RippleData.xyz, v);
		v += normalize(v) * rippleAmount;
		
		float leftInnerEdge = _ScreenWidth*-0.5;
		float leftOuterEdge = leftInnerEdge-_HFeather;
		
		float rightInnerEdge = _ScreenWidth*0.5;
		float rightOuterEdge = rightInnerEdge+_HFeather;
		
		float hFlatten = min( smoothstep(leftOuterEdge, leftInnerEdge, v.x), smoothstep(rightOuterEdge, rightInnerEdge, v.x));
		
		float topInnerEdge = _ScreenHeight*0.5;
		float topOuterEdge = topInnerEdge+_VFeather;
		
		float bottomInnerEdge = _ScreenHeight*-0.5;
		float bottomOuterEdge = bottomInnerEdge-_VFeather;
		
		float vFlatten = min( smoothstep(topOuterEdge, topInnerEdge, v.y), smoothstep(bottomOuterEdge, bottomInnerEdge, v.y));
		
		float f = hFlatten * vFlatten * _FlattenAmount;
		
		float control = saturate(sign(v.z));
		v.z = lerp(v.z, _ScreenPos, f*control);


		float pointInside = pointWithinBounds(v, float3(_ScreenWidth*-0.5, _ScreenHeight*-0.5, 0), float3(_ScreenWidth*0.5, _ScreenHeight*0.5, screenDepth));
						
		if (abs(topInnerEdge-v.y)<_SnapThreshold && pointInside>=1.0){
			v.y = lerp(v.y, topInnerEdge, _SnapAmount);
		} 

		if (abs(bottomInnerEdge-v.y)<_SnapThreshold && pointInside>=1.0){
			v.y = lerp(v.y, bottomInnerEdge, _SnapAmount);
		}
		
		if (abs(leftInnerEdge-v.x)<_SnapThreshold && pointInside>=1.0){
			v.x = lerp(v.x, leftInnerEdge, _SnapAmount);
		}
		
		if (abs(rightInnerEdge-v.x)<_SnapThreshold && pointInside>=1.0){
			v.x = lerp(v.x, rightInnerEdge, _SnapAmount);
		}
	
		return pointInside;
	}
	

	

	void vert (inout appdata_full v, out Input data) {
		UNITY_INITIALIZE_OUTPUT(Input, data);
		
		// pass original vertex position to fragment shader for gradient color 
		data.pos = v.vertex.xyz;

		// calculate UV of projected texture
		float3 relPos = (v.vertex.xyz - _ProjectionCentre.xyz) / float3(_Scale*_AspectRatio, _Scale, 1.0);
		float2 texcoord = centrePoint + (relPos.xy);
		float texAlpha = 0;
		if (sign(relPos.z) == sign(_ProjectionCentre.w)){
			texAlpha = _ProjectionAlpha;
		}		
		data.texcoord = texcoord;
		data.texAlpha = texAlpha;
		
		// get information about other vertices of this face, from encoded vertex data
		float3 p1 = v.vertex.xyz;
		float3 p2 = v.normal.xyz;
		float3 p3 = v.tangent.xyz;
		
		// distort each point
		float inside1 = distort(p1);
		float inside2 = distort(p2);
		float inside3 = distort(p3);
		
		// recalculte normal
		float3 edge0 = p2 - p1;
		float3 edge1 = p3 - p1;
		v.normal.xyz = cross(edge0, edge1);
		
		// apply distortion to this vertex
		v.vertex.xyz = p1;
		
		float p1_len = length(p1.xy);
		float p2_len = length(p2.xy);
		float p3_len = length(p3.xy);
		
		data.inside = p1_len < _DarkenRadius && p2_len < _DarkenRadius && p3_len < _DarkenRadius && inside1 > 0 && inside2 > 0 && inside3 > 0;
					
	}
			
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 gradientTexture = tex2D(_MainTex, IN.texcoord);
		
		o.Albedo = lerp(_BackColor, gradientTexture, IN.texAlpha)*_ColorFade;
	
		o.Emission = _EmissionColor.rgb*_ColorFade;
		
		if (IN.inside > 0.5){
			o.Albedo *= _DarkenAmount;
			o.Emission *= _DarkenAmount;
		}
		
		o.Alpha = 1.0;
	}
	
	ENDCG
} 

}
