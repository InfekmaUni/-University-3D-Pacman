#version 330

out vec4 Color;
uniform sampler2D texture;
uniform int textureScale;
in vec2 v_TexCoord;

bool IsTransparentPixel(vec3 text){
	return( (text.x == 0 || text.x == 255) && (text.y == 0 || text.y == 255) && (text.z == 0 || text.z == 255));
}

void main()
{
	int scale = textureScale;
	if( scale == 0 )
		scale = 1;

	vec3 text = texture2D( texture, v_TexCoord *scale ).xyz;
	if(IsTransparentPixel(text))
		discard;

	Color = vec4(text, 1);
}