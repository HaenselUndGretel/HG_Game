
struct LS
{
	float4 Light	: COLOR0;
	float4 Shadow	: COLOR1;
	float4 UV		: COLOR2;
};


float4 VS_Main(float3 Position : POSITION0) : POSITION0
{
	return float4(Position, 1);
}

LS PS_Main()
{
	LS output;

	output.Light.rgba  = float4(0.0f, 0.0f, 0.0f, 1.0f);
	output.Shadow.rgba = float4(0.0f, 0.0f, 0.0f, 1.0f);
	output.UV.rgba     = float4(0.0f, 0.0f, 0.0f, 1.0f);

	return output;
}

technique Technique1
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 VS_Main();
		PixelShader = compile ps_2_0 PS_Main();
    }
}
