#ifdef vertex
#version 330 core
layout (location = 0) in vec2 aPos;
out vec2 textureCoords;

uniform mat4 model_matrix;

void main() {
	gl_Position = model_matrix * vec4(aPos, 0, 1.0);
	textureCoords = vec2((aPos.x + 1.0) * 0.5,(aPos.y + 1.0) * 0.5);

}
#endif

#ifdef fragment
#version 330 core
in vec2 textureCoords;

out vec4 fragmentColor;

uniform sampler2D ourTexture;
uniform float length_used;

void main() {
	if(textureCoords.x>length_used){
		fragmentColor=vec4(0,0,0,0);
		}
	else if(length_used<0.15){
		fragmentColor = texture(ourTexture,textureCoords)*vec4(1,0,0,1);
	}
	else if(length_used<=0.5){
		fragmentColor = texture(ourTexture,textureCoords)-vec4(0,0.3,0,1)+vec4(0.7,0.2,0,1);
	}
	
	else{
		fragmentColor = texture(ourTexture,textureCoords);
	}
}

#endif



