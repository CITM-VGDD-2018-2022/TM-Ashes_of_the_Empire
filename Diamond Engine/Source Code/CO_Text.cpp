#include "CO_Text.h"
#include "CO_Transform2D.h"

#include "Application.h"

#include "RE_Material.h"
#include "RE_Shader.h"

#include "MO_FileSystem.h"

#include "IM_FontImporter.h"
#include <map>

#include "MathGeoLib/include/MathGeoLib.h"
#include "ImGui/imgui.h"
#include "OpenGL.h"

C_Text::C_Text(GameObject* gameObject) : Component(gameObject),
	text(""),
	maxTextLenght(0.f),
	font(nullptr)
{
	memset(textColor, 0, sizeof(textColor));
	name = "Text";
}


C_Text::~C_Text()
{
	text.clear();
	font = nullptr;
}


void C_Text::RenderText(C_Transform2D* transform, ResourceMaterial* material, unsigned int VAO, unsigned int VBO)
{
	if (font == nullptr)
		return;

	material->shader->Bind();
	material->PushUniforms();
	glActiveTexture(GL_TEXTURE0);
	glBindVertexArray(VAO);

	float x = 0;
	float y = 0;

	int textIndex = 0;
	std::map<char, Character>::iterator it;

	while (textIndex<text.size())
	{
		if (text[textIndex] == ' ')//In case we enconter a Space
		{
			it = font->characters.find(text[textIndex]);
			if (it != font->characters.end()) //If it exists in the font
			{
				const Character& character = it->second;

				if (x+(character.advance[0] >> 6) >= maxTextLenght && maxTextLenght != 0) //if the space is in the end of the line go to next line
				{
					y -= (character.advance[1] >> 6);
					x = 0.f;
				}
				else
				{
					x += (character.advance[0] >> 6);	//Bitshift by 6 to get size in pixels
				}
			}

			++textIndex;
		}
		else //Then it is a word
		{
			int wordLengthInChars = 0;
			int wordLength = DetectWordLength(textIndex, wordLengthInChars);

			if(x+wordLength >= maxTextLenght && maxTextLenght != 0) //if the word exceeds line width go to next line
			{
				it = font->characters.find(text[textIndex]);
				if (it != font->characters.end()) //If it exists in the font
				{
					const Character& character = it->second;
					y -= (character.advance[1] >> 6);
					x = 0.f;
				}
			}


			for (int i = textIndex; i < textIndex+ wordLengthInChars; i++)
			{
				it = font->characters.find(text[i]);

				if (it != font->characters.end())
				{
					const Character& character = it->second;

					glBindTexture(GL_TEXTURE_2D, font->atlasTexture);
					GLint modelLoc = glGetUniformLocation(material->shader->shaderProgramID, "model_matrix");
					glUniformMatrix4fv(modelLoc, 1, GL_FALSE, transform->GetGlobal2DTransform().ptr());

					modelLoc = glGetUniformLocation(material->shader->shaderProgramID, "altColor");
					glUniform3f(modelLoc, textColor[0], textColor[1], textColor[2]);

					float posX = (x + character.bearing[0]) / 100.f;
					float posY = (y - (character.size[1] - character.bearing[1])) / 100.f;

					float width = character.size[0] / 100.f;
					float height = character.size[1] / 100.f;

					float vertices[6][4] = {
						{ posX,			posY + height,  character.textureOffset, 0.0f },
						{ posX,			posY,			character.textureOffset, character.size[1] / (float)font->atlasHeight },
						{ posX + width, posY,			character.textureOffset + character.size[0] / (float)font->atlasWidth, character.size[1] / (float)font->atlasHeight},

						{ posX,			posY + height,  character.textureOffset, 0.0f },
						{ posX + width, posY,			character.textureOffset + character.size[0] / (float)font->atlasWidth, character.size[1] / (float)font->atlasHeight },
						{ posX + width, posY + height,  character.textureOffset + character.size[0] / (float)font->atlasWidth, 0.0f }
					};

					glBindBuffer(GL_ARRAY_BUFFER, VBO);
					glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(vertices), vertices);

					// render quad
					glDrawArrays(GL_TRIANGLES, 0, 6);

					x += (character.advance[0] >> 6);	//Bitshift by 6 to get size in pixels
				}
			}

			textIndex += wordLengthInChars;
		}
		


	}

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glBindVertexArray(0);
	glBindTexture(GL_TEXTURE_2D, 0);

	if (material->shader)
		material->shader->Unbind();
}

int C_Text::DetectWordLength(int textStartingIndex, int& nCharacters)
{
	int wordLenght = 0;
	nCharacters = 0;

	bool encounteredSpace = false;
	std::map<char, Character>::iterator it;

	while (textStartingIndex+ nCharacters <text.size()&& encounteredSpace==false)
	{
		if (text[textStartingIndex + nCharacters] == ' ')
		{
			encounteredSpace = true; //TODO maybe a break wold be more efficient?
		}
		else
		{
			it = font->characters.find(text[textStartingIndex + nCharacters]);
			if (it != font->characters.end()) //If it exists in the font
			{
				const Character& character = it->second;
				wordLenght += (character.advance[0] >> 6);
			}

			++nCharacters;
		}
	}

	return wordLenght;
}

const char* C_Text::GetText()
{
	return text.c_str();
}

float C_Text::GetColorText(int index)
{
	if (index < 0 || index>2)
		return 0.0f;
	return textColor[index];
}

void C_Text::SetColorText(int index, float value)
{
	if (index < 0 || index>2 || value < 0 || value>1)
		return;
	textColor[index] = value;
}

void C_Text::SetText(const char* new_text)
{
	text = new_text;
}



#ifndef STANDALONE
bool C_Text::OnEditor()
{
	//EngineExternal->moduleFileSystem->free_type_library
	if (Component::OnEditor() == true)
	{
		char inputText[MAX_CHARACTER_TEXT];
		strcpy(inputText, text.c_str());
		ImGui::InputText("Text to print", &inputText[0], sizeof(inputText));

		text = inputText;


		ImGui::Text("Text color");
		ImGui::SameLine();
		ImGui::DragFloat3("##ltextCol", &textColor[0], 0.01f, 0.0f, 1.0f);

		ImGui::Text("Text width");
		ImGui::SameLine();
		ImGui::DragFloat("##lMaxLenght", &maxTextLenght);

		ImGui::Text("Drop here to change font");
		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("_FONT"))
			{
				std::string* assetsPath = (std::string*)payload->Data;
				font = EngineExternal->moduleFileSystem->free_type_library->GetFont(assetsPath->c_str());
			}
			ImGui::EndDragDropTarget();
		}

		if (font)
			ImGui::Text("Using font: %s", font->name.c_str());
	}
	return true;
}
#endif // !STANDALONE


void C_Text::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	DEJson::WriteString(nObj, "text", text.c_str());
	DEJson::WriteFloat(nObj, "maxTextLenght", maxTextLenght);

	if (font != nullptr)
		DEJson::WriteString(nObj, "fontName", font->name.c_str());

	DEJson::WriteVector3(nObj, "textColor", textColor);
}


void C_Text::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	text = nObj.ReadString("text");
	maxTextLenght = nObj.ReadFloat("maxTextLenght");

	font = EngineExternal->moduleFileSystem->free_type_library->GetFont(nObj.ReadString("fontName"));

	float3 col = nObj.ReadVector3("textColor");
	textColor[0] = col.x;
	textColor[1] = col.y;
	textColor[2] = col.z;
}