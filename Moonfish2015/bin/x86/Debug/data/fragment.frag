#version 130

smooth in vec4 normal;
<<<<<<< HEAD
smooth in vec3 tangent;
smooth in vec3 bitangent;
=======
>>>>>>> parent of 92e0283... P8 Palette
smooth in vec4 diffuseColour;
smooth in vec3 vertexPosition;
smooth in vec4 lightPosition;
smooth in vec4 varyingTexcoord;

uniform sampler2D diffuseTexture;

out vec4 frag_color;

void main()
{
    vec3 normal = normal.xyz;
	vec3 light = lightPosition.xyz;
	vec3 lightNormal = normalize(light - vertexPosition);
	vec3 eyeNormal = normalize(-vertexPosition);
	vec3 reflection = normalize(-reflect(lightNormal, normal));
	
	vec4 ambient = diffuseColour * 0.45f;
	
	vec4 diffuse = diffuseColour * max(dot(normal, lightNormal), 0.0);
	diffuse = clamp(diffuse, 0.0, 1.0);
		
	float specularLevel = 128f;
	vec4 specular = vec4(0.2, 0.2, 0.2, 1.0)  * pow(max(dot(reflection, eyeNormal), 0.0), 0.3 * specularLevel);
	specular = clamp(specular, 0.0, 1.0);
	
<<<<<<< HEAD
	float index = texture(diffuseTexture, varyingTexcoord.st).r;

	frag_color =  texture(ColourPaletteUniform, index).bgra * max( ambient, diffuse ) + specular;
=======
	frag_color =  texture(diffuseTexture, varyingTexcoord.st) * max( ambient, diffuse ) + specular;
>>>>>>> parent of 92e0283... P8 Palette
}
