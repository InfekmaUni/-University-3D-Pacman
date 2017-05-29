#version 330 core
 
layout (location=0) in vec3 a_position;
layout (location=1) in vec2 a_texCoord;
layout (location=2) in vec3 a_normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform mat4 world;

uniform vec3 lightPosition;
uniform vec3 eyePosition;

uniform int textureScale;

out vec2 textCoord;
out vec3 fragPos;
out vec3 viewDirection;
out vec3 lightDirection;
out vec3 normal;

void main()
{
	
    gl_Position =  projection * view * world*  model * vec4(a_position, 1.0);
    textCoord = a_texCoord * textureScale;
	fragPos = vec3(model * vec4(a_position, 1));

	//lightDirection = lightPosition - fragPos;
	lightDirection = vec3(0,-1,0);
	viewDirection = eyePosition - a_position;
	normal = normalize(a_normal);
}