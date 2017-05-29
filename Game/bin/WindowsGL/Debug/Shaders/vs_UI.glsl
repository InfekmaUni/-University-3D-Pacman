#version 330
 
in vec3 a_position;
in vec2 a_texCoord;
uniform mat4 projection;
uniform mat4 model;
out vec2 v_TexCoord;

void main()
{
	// screen allign
    gl_Position =  projection * model * vec4(a_position.xy, 0.0, 1.0);
    v_TexCoord = (vec2( a_position.x, a_position.y ) + vec2( 1.0 ) ) / vec2( 2.0 );
}