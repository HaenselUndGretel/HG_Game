sampler DrawMap: register(s1);


struct VS_Input
{
	float3 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct PS_Input
{
	float4 Position : POSITION;
	float2 UV : TEXCOORD0;
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
	float4 fColor = tex2D(DrawMap, input.UV);
	return fColor;
}

technique Technique1
{
	pass SimpelDraw
	{
		VertexShader = compile vs_2_0 Vs_Main();
		PixelShader = compile ps_2_0 Ps_Main();
	}

}
