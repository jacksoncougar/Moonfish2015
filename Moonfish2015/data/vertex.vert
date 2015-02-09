#version 130

in vec4 position;
in vec2 texcoord;
in int compressedNormal;
in vec4 colour; 
in mat4 worldMatrix;
in mat4 objectExtents;

uniform mat4 viewProjectionMatrix;
uniform mat4 viewMatrix; 
uniform vec3 LightPositionUniform;

smooth out vec3 normal;
smooth out vec4 diffuseColour;
smooth out vec3 vertexPosition;
smooth out vec4 lightPosition;
smooth out vec2 varyingTexcoord;

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
	normal = normalize(normalMatrix * decompress(compressedNormal));
	varyingTexcoord = texcoord;
	lightPosition = viewMatrix * vec4(LightPositionUniform, 1);
	
    gl_Position = viewProjectionMatrix  * worldMatrix * objectExtents * position;
}