float4x4 World;
float4x4 View;
float4x4 Projection;

sampler NormalMap  : register(s0);
sampler DepthMap   : register(s1);


float2 screen;

float LightIntensity;
float3 LightColor;
float4 LightPosition;
float LightRadius;

struct VertexShaderInput
{
    float4 Position     : POSITION0;
	float2 TexCoord		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position     : POSITION0;
	float2 TexCoord		: TEXCOORD0;
	float4 LightPoss	: TEXCOORD1;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
  	VertexShaderOutput Output = (VertexShaderOutput)0;

	Output.Position = input.Position;

	Output.LightPoss = mul(LightPosition, View);
	Output.TexCoord = input.TexCoord;

	return Output;
}



float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 output;

	float ration = screen.y / screen.x;

	//Maps Laden
	float4 normalColor = tex2D(NormalMap, input.TexCoord.xy).rgba;
	float AoColor = normalColor.a;
	float3 DepthColor = tex2D(DepthMap, input.TexCoord.xy).rgb;

		// Normalen normalisiren (RGB) = (XYZ)
	float3 normal = normalColor.rgb*2.0-1.0 ;

	normal.x *= -1;
	normal.z *= -1;

	float3 NnormalColor = normalize(normal);
		
	
	

	// Position Berechnen
	float z = ((DepthColor.r + DepthColor.g + DepthColor.b) / 3)*650 ;
	float3 WorldPos = float3(screen.x*input.TexCoord.x, (screen.y*input.TexCoord.y) + (z*0.8), z);

		//Licht Position und Richtung
	float3 LightPos = input.LightPoss.xyz;
	float3 lightDir = (WorldPos - LightPos);// * float3(0.75, 1.0, 1.0);

	// Licht einfall berechnen
	float light = max(dot(NnormalColor, normalize(lightDir)), 0.0) ;

	float r = LightPos / (-1 * lightDir.z);

	float3 ShadowPos = LightPos + r*lightDir;

	
	//falloff
	float lightDistance = length(lightDir);
	float attenuation = LightIntensity/(lightDistance / LightRadius);//LightIntensity / (lightDistance + 0.00001) - (1/LightRadius);//LightIntensity / (max(lightDistance * LightRadius, 1.0 / LightRadius));

	float3 fColor = light.xxx;//LightColor*light*attenuation;
		//fColor = dot(NnormalColor, float3(1,0,0));
	//fColor = NnormalColor;


	output = float4(fColor, 1.0f);
	
	return output;


	/*float3 LightDir = (WorldPos - LightPos);
	float3 NLightDir = normalize(LightDir);


	float light = saturate(dot(NnormalColor, -NLightDir));

	float distance = length(LightDir);
	
	float att = saturate(LightRadius / distance);*/

}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
