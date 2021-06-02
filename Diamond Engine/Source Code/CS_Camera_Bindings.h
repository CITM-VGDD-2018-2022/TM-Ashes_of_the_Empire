#pragma once
#include "Application.h"
#include "CO_Camera.h"
#include "MO_MonoManager.h"
#include "GameObject.h"


void SetOrthSize(MonoObject* go, float size) {
	if (go == NULL) return;

	GameObject* GO = EngineExternal->moduleMono->GameObject_From_CSGO(go);

	C_Camera* cam = dynamic_cast<C_Camera*>(GO->GetComponent(Component::TYPE::CAMERA));
	if (cam != nullptr || cam->camFrustrum.type!=FrustumType::OrthographicFrustum)
	{
		cam->SetOrthSize(size);
	}
	else
	{
		LOG(LogType::L_WARNING, "Couldn't get the camera or it's not a orthogonal camera");
	}
}

float GetOrthSize(MonoObject* go) {
	if (go == nullptr)
		return NULL;

	GameObject* GO = EngineExternal->moduleMono->GameObject_From_CSGO(go);

	C_Camera* cam = dynamic_cast<C_Camera*>(GO->GetComponent(Component::TYPE::CAMERA));
	if (cam != nullptr && cam->camFrustrum.type != FrustumType::PerspectiveFrustum)
	{
		return cam->GetOrthSize();
	}
	else
	{
		LOG(LogType::L_WARNING, "Couldn't get the orth size. Component was null pointer or it's not a orth camera");
		return NULL;
	}
}

void CS_SetCameraOrthographic(MonoObject* go)
{
	if (go == nullptr)
		return;

	GameObject* GO = EngineExternal->moduleMono->GameObject_From_CSGO(go);

	C_Camera* cam = dynamic_cast<C_Camera*>(GO->GetComponent(Component::TYPE::CAMERA));

	if (cam != nullptr)
	{
		cam->SetCameraToOrthographic();
	}
}

void CS_SetCameraPerspective(MonoObject* go)
{
	if (go == nullptr)
		return;

	GameObject* GO = EngineExternal->moduleMono->GameObject_From_CSGO(go);

	C_Camera* cam = dynamic_cast<C_Camera*>(GO->GetComponent(Component::TYPE::CAMERA));

	if (cam != nullptr)
	{
		cam->SetCameraToPerspective();
	}
}

void CS_SetCameraVerticalFOV(MonoObject* go, float verticalFOV)
{
	if (go == nullptr)
		return;

	GameObject* GO = EngineExternal->moduleMono->GameObject_From_CSGO(go);

	C_Camera* cam = dynamic_cast<C_Camera*>(GO->GetComponent(Component::TYPE::CAMERA));

	if (cam != nullptr)
	{
		cam->SetVerticalFOV(verticalFOV);
	}
}

void SetAsGameCamera(MonoObject* go)
{
	if (go == nullptr)
		return;

	GameObject* GO = EngineExternal->moduleMono->GameObject_From_CSGO(go);

	C_Camera* cam = dynamic_cast<C_Camera*>(GO->GetComponent(Component::TYPE::CAMERA));

	if (cam != nullptr)
	{
		cam->SetAsGameCamera();
	}
}