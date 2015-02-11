#version 130

in vec4 position;
in vec2 texcoord;
in int compressedNBTvectors[3];
in vec4 colour; 
in mat4 worldMatrix;
in mat4 objectExtents;

uniform mat4 viewProjectionMatrix;
uniform mat4 viewMatrix; 
uniform vec3 LightPositionUniform;
uniform vec4 texcoordRangeUniform;

smooth out vec3 normal;

out mat3 TBN;

smooth out vec4 diffuseColour;
smooth out vec3 vertexPosition;
smooth out vec4 lightPosition;
smooth out vec2 varyingTexcoord;

float decompress(in float value, in vec2 bounds)
{
	const float ushortMaxInverse = 1.0 / 65535.0;
	const float ushortHalf = 32767.0;
    return (((value + ushortHalf) * ushortMaxInverse ) * (bounds.y - bounds.x)) + bounds.x;
}

vec3 decompress(in int compressedNormal)
{
	int x10 = (compressedNormal & 0x000007FF);
	if ((x10 & 0x00000400) == 0x00000400)
	{
		x10 = -((~x10) & 0x000007FF);
		if (x10 == 0) x10 = -1;
	}
	int y11 = (compressedNormal >> 11) & 0x000007FF;
	if ((y11 & 0x00000400) == 0x00000400)
	{
		y11 = -((~y11) & 0x000007FF);
		if (y11 == 0) y11 = -1;
	}
	int z11 = (compressedNormal >> 22) & 0x000003FF;
	if ((z11 & 0x00000200) == 0x00000200)
	{
		z11 = -((~z11) & 0x000003FF);
		if (z11 == 0) z11 = -1;
	}

	float x = float(x10) / 1023.0;
	float y = float(y11) / 1023.0;
	float z = float(z11) / 511.0;
	
	return vec3(x, y, z);
}

void main()
{
	mat3 normalMatrix = mat3(viewMatrix);	
	diffuseColour  = colour;
	vertexPosition = vec3(viewMatrix  * worldMatrix * objectExtents * position);

	vec3 vertexNormal_cameraspace = normalMatrix * normalize(decompress(compressedNBTvectors[0]));
	vec3 vertexTangent_cameraspace =normalMatrix *  normalize(decompress(compressedNBTvectors[1]));
	vec3 vertexBitangent_cameraspace = normalMatrix * normalize(decompress(compressedNBTvectors[2]));
	
	TBN = transpose(mat3(
        vertexTangent_cameraspace,
        vertexBitangent_cameraspace,
        vertexNormal_cameraspace
    ));

	varyingTexcoord = vec2(decompress(texcoord.s, texcoordRangeUniform.xy), decompress(texcoord.t, texcoordRangeUniform.zw));
	lightPosition = viewMatrix * vec4(LightPositionUniform, 1);
	
    gl_Position = viewProjectionMatrix  * worldMatrix * objectExtents * position;
}