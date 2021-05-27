#pragma once
#include "Component.h"

class ResourceTexture;
class C_AudioSource;

#define TEXTURE_BLEND_DURATION 0.12

enum class BUTTONSTATE 
{
	BUTTON_NULL,
	BUTTONPRESSED,
	BUTTONHOVERED,
	BUTTONUNHOVERED
};

class C_Button : public Component 
{
public:
	C_Button(GameObject* gameObject);
	~C_Button() override;

	void Update() override;

	void ExecuteButton();
	void ReleaseButton();

	void ChangeTexture(BUTTONSTATE new_num_sprite, BUTTONSTATE previousState = BUTTONSTATE::BUTTON_NULL);

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

	inline BUTTONSTATE GetActualState() {
		return num_sprite_used;
	}

	void ChangeSprite(BUTTONSTATE num_sprite, ResourceTexture* sprite);

	void PlayHoveredSFX();
	void PlayPressedSFX();
#ifndef STANDALONE
	void ChangeScript(const char* script_name);
	bool OnEditor() override;

private:
	bool sprites_freezed;
#endif // !STANDALONE

public:
	bool is_selected;

private:
	ResourceTexture* sprite_button_pressed;
	ResourceTexture* sprite_button_hovered;
	ResourceTexture* sprite_button_unhovered;
	BUTTONSTATE num_sprite_used;
	std::string script_name;
	C_AudioSource* thisAudSource;

	std::string hoverSFX;
	std::string pressedSFX;

	float textureBlendTimer = 0.0f;
};