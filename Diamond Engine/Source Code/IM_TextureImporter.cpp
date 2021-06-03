#include"IM_TextureImporter.h"

#include"IM_FileSystem.h"

#include"Globals.h"

#include "OpenGL.h"
#include "DevIL\include\ilu.h"
#include "DevIL\include\ilut.h"

#include"DEResource.h"
#include"RE_Texture.h"
#include"DE_Cubemap.h"

#pragma comment( lib, "DevIL/libx86/DevIL.lib" )
#pragma comment( lib, "DevIL/libx86/ILU.lib" )
#pragma comment( lib, "DevIL/libx86/ILUT.lib" )



void TextureImporter::Init()
{
	ilInit();
	iluInit();
	ilutInit();
	ilutRenderer(ILUT_OPENGL);
}

GLuint TextureImporter::LoadToMemory(char* buffer, int size, int* w, int* h)
{
	ILuint imageID;
	ilGenImages(1, &imageID);
	ilBindImage(imageID);

	if (!ilLoadL(IL_TYPE_UNKNOWN, buffer, size))
	{
		LOG(LogType::L_ERROR, "Image not loaded");
	}

	if (w)
		*w = ilGetInteger(IL_IMAGE_WIDTH);
	if (h)
		*h = ilGetInteger(IL_IMAGE_HEIGHT);

	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
	GLuint glID = ilutGLBindTexImage();

	//TODO: Generate mipmaps and use best settings
	glBindTexture(GL_TEXTURE_2D, glID);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glGenerateMipmap(GL_TEXTURE_2D);
	glBindTexture(GL_TEXTURE_2D, 0);


	ilDeleteImages(1, &imageID);
	glBindTexture(GL_TEXTURE_2D, 0);

	return glID;
}

void TextureImporter::SaveDDS(char* buffer, int size, const char* fileName)
{
	ILuint imageID;
	ilGenImages(1, &imageID);
	ilBindImage(imageID);

	if (!ilLoadL(IL_TYPE_UNKNOWN, buffer, size))
	{
		LOG(LogType::L_ERROR, "Image not loaded");
	}

	//TODO: Move this to function
	ILuint _size = 0;
	ILubyte* data = nullptr;
	ilSetInteger(IL_DXTC_FORMAT, IL_DXT5);
	_size = ilSaveL(IL_DDS, nullptr, 0);
	
	if (_size > 0) 
	{
		data = new ILubyte[_size];
		ilSaveL(IL_DDS, data, _size);

		std::string path(fileName);
		//path += ".dds";

		FileSystem::Save(path.c_str(), (char*)data, _size, false);

		delete[] data;
		data = nullptr;
	}

	ilDeleteImages(1, &imageID);
}

void TextureImporter::Import(char* buffer, int bSize, Resource* res)
{
	SaveDDS(buffer, bSize, res->GetLibraryPath());
}

/*Take a screenshot*/
void TextureImporter::TakeScreenshot(int frameBuffer)
{
	glBindFramebuffer(GL_FRAMEBUFFER, frameBuffer);

	ILuint imageID = ilGenImage();
	ilBindImage(imageID);
	ilutGLScreen();
	ilEnable(IL_FILE_OVERWRITE);
	ilSaveImage("Screenshots/Screenshot.png");
	ilDeleteImage(imageID);

	glBindFramebuffer(GL_FRAMEBUFFER, 0);
}

void TextureImporter::LoadCubeMap(char* faces[6], DE_Cubemap& cubeMap)
{
	int shouldReload = 0;
	for (size_t i = 0; i < 6; i++)
	{
		if (&cubeMap.loadedTextures[i] != nullptr && strcmp(cubeMap.loadedTextures[i].c_str(), faces[i]) == 0) 
		{
			shouldReload++;
		}
	}
	if (shouldReload == 6) {
		return;
	}

	cubeMap.ClearTextureMemory();

	unsigned int textureID;
	glGenTextures(1, &textureID);
	glBindTexture(GL_TEXTURE_CUBE_MAP, textureID);


	int width = 0, height = 0, nrChannels = 0;
	for (unsigned int i = 0; i < 6; i++)
	{
		char* buffer = nullptr;
		unsigned int size = FileSystem::LoadToBuffer(faces[i], &buffer);

		ILuint imageID;
		ilGenImages(1, &imageID);
		ilBindImage(imageID);

		if (!ilLoadL(IL_DDS, buffer, size))
			LOG(LogType::L_ERROR, "Image not loaded");

		width = ilGetInteger(IL_IMAGE_WIDTH);
		height = ilGetInteger(IL_IMAGE_HEIGHT);

		unsigned char* data = ilGetData();

		if (data)
		{
			glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
			glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i,
				0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, data
			);

			//glTexParameterf(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
			//glTexParameterf(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
		}
		else
		{
			LOG(LogType::L_ERROR, "Cubemap tex failed to load at path: %s", faces[i]);
		}

		RELEASE_ARRAY(buffer);
		ilDeleteImage(imageID);
		ilBindImage(0);
	}
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);

	glBindTexture(GL_TEXTURE_CUBE_MAP, 0);


	for (size_t i = 0; i < 6; i++)
	{
		cubeMap.loadedTextures[i] = faces[i];
		//strcpy(cubeMap.loadedTextures[i], faces[i]);
	}
	cubeMap.textureID = textureID;
	cubeMap.canRender = true;
}
