sampler SceneDepthMap: register(s1);
sampler FogTexture: register(s2);
sampler SceneDiffuse: register(s3);

float FogStrength;
float FogFactorMin;
float FogFactorMax;

float Timer;
float Speed;

struct VS_Input
{
	float3 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct PS_Input
{
	float4 Position : POSITION;
	float2 UV		: TEXCOORD0;
};

PS_Input Vs_Main(VS_Input input)
{
	PS_Input output;

	output.Position = float4(input.Position, 1.0f);
	output.UV = input.UV;

	return output;
}

float4 Ps_Main(PS_Input input) : COLOR0
{
	float4 fColor;

	float3 diffuse		= tex2D(SceneDiffuse, input.UV).rgb;
	float3 diffuseDepth = tex2D(SceneDepthMap, input.UV).rgb;
	float3 diffuseFog	= tex2D(FogTexture, float2(input.UV.x + (-Timer * Speed), input.UV.y)).rgb;

	float lerpFactor = 1 / (FogFactorMax - FogFactorMin);
	float depth = diffuseDepth.r - FogFactorMin;
	depth *= lerpFactor;
	depth = saturate(depth);

	float3 Fog = lerp(diffuseFog, diffuse, depth);

	diffuse = lerp(diffuse, Fog, FogStrength);

	fColor = float4(diffuse, 1);

	return fColor;
}



technique Technique1
{
	pass Fog
	{
		VertexShader = compile vs_2_0 Vs_Main();
		PixelShader = compile ps_2_0 Ps_Main();
	}

}
