﻿ #version 130

in vec4 position;
smooth in out vec4 diffuseColour;

	uniform mat4 objectExtents;
	uniform mat4 objectWorldMatrix;
	uniform mat4 viewProjectionMatrix;

void main()
{
    vec4 viewPosition = viewProjectionMatrix  * objectWorldMatrix * position;
	
	gl_Position = viewPosition;
}