#ifdef vertex
#version 330 core
layout (location = 0) in vec2 aPos;
out vec2 textureCoords;
out vec2 textureCoordsRaw;

uniform mat4 model_matrix;


void main() {
	gl_Position = model_matrix * vec4(aPos, 0.0, 1.0);
	textureCoords = vec2((aPos.x + 1.0) * 0.5,(aPos.y + 1.0) * 0.5);
	textureCoordsRaw=aPos;

}
#endif

#ifdef fragment
#version 330 core
in vec2 textureCoords;
in vec2 textureCoordsRaw;
out vec4 fragmentColor;

//uniform sampler2D ourTexture;

uniform float timeSinceStart;
uniform vec3 damageColor;

uniform float pulsationAmmount;
uniform float alphaAmmount;
void main() 
{
	
	float intensity=100;//15 def
	//float extend=0.25;//0.25 def

	float extend = 0.5*(sin(timeSinceStart*pulsationAmmount)+1.0);
	extend *=0.9;
	float vignette = 1.0;
	//vec4 myColor = texture(ourTexture,textureCoords);
	
	vec2 uv = textureCoords;
	uv*= (1.0-uv.yx);
	vignette = uv.x*uv.y * intensity;
	vignette = pow(vignette, extend);
	

	vec4 myColor= vec4(damageColor,1.0);
	myColor.a *= (1.0-vignette);
	myColor.a *=clamp(alphaAmmount,0.0,1.0);
	
	fragmentColor=myColor;
}

#endif




























