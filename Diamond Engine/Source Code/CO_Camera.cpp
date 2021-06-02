#include "CO_Camera.h"
#include"Globals.h"
#include"DEJsonSupport.h"
#include"ImGui/imgui.h"

#include"MathGeoLib/include/Math/float3.h"
#include"MathGeoLib/include/Geometry/Plane.h"

#include"MO_Renderer3D.h"
#include"MO_Scene.h"

#include"GameObject.h"
#include"CO_Transform.h"
#include"OpenGL.h"
#include"MO_Window.h"
#include "MO_ResourceManager.h"
#include "IM_PostProcessImporter.h"
#include"MathGeoLib/include/Geometry/AABB.h"

C_Camera::C_Camera() : Component(nullptr),
fov(60.0f),
cullingState(true),
msaaSamples(4),
orthoSize(0.0f),
windowWidth(0),
windowHeight(0),
drawSkybox(true),
msaaFBO(1920, 1080, 4,false),
resolvedFBO(1920, 1080, DEPTH_BUFFER_TYPE::DEPTH_TEXTURE, false),
postProcessProfile(nullptr),
isHDR(false)

{
	name = "Camera";
	camFrustrum.type = FrustumType::PerspectiveFrustum;
	camFrustrum.nearPlaneDistance = 0.1f;
	camFrustrum.farPlaneDistance = 500.f;
	camFrustrum.front = float3::unitZ;
	camFrustrum.up = float3::unitY;

	camFrustrum.verticalFov = 60.0f * DEGTORAD;
	camFrustrum.horizontalFov = 2.0f * atanf(tanf(camFrustrum.verticalFov / 2.0f) * 1.7f);

	camFrustrum.pos = float3::zero;
}

C_Camera::C_Camera(GameObject* _gm) : Component(_gm), fov(60.0f), cullingState(true),
msaaSamples(4), orthoSize(0.0f), drawSkybox(true),
msaaFBO(1920, 1080, 4,false),
resolvedFBO(1920, 1080, DEPTH_BUFFER_TYPE::DEPTH_TEXTURE,false),
postProcessProfile(nullptr),
isHDR(false)
{
	name = "Camera";
	camFrustrum.type = FrustumType::PerspectiveFrustum;
	camFrustrum.nearPlaneDistance = 1;
	camFrustrum.farPlaneDistance = 100.f;
	camFrustrum.front = gameObject->transform->GetForward();
	camFrustrum.up = gameObject->transform->GetUp();

	camFrustrum.verticalFov = 60.0f * DEGTORAD;
	camFrustrum.horizontalFov = 2.0f * atanf(tanf(camFrustrum.verticalFov / 2.0f) * 1.7f);

	camFrustrum.pos = gameObject->transform->position;
}

C_Camera::~C_Camera()
{
	msaaFBO.ClearBuffer();
	resolvedFBO.ClearBuffer();

	if (EngineExternal && EngineExternal->moduleRenderer3D->GetGameRenderTarget() == this)
		EngineExternal->moduleRenderer3D->SetGameRenderTarget(nullptr);

	if (EngineExternal && EngineExternal->moduleRenderer3D->activeRenderCamera == this)
		EngineExternal->moduleRenderer3D->activeRenderCamera = nullptr;

	if (postProcessProfile != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(postProcessProfile->GetUID());
		postProcessProfile = nullptr;
	}
}

#ifndef STANDALONE
bool C_Camera::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::Separator();
		bool newHDR = isHDR;
		if (ImGui::Checkbox("HDR camera", &newHDR))
		{
			ChangeHDR(newHDR);
		}

		//ImGui::Text("FB %i, TB %i, RBO %i", framebuffer, texColorBuffer, rbo);

		ImGui::DragFloat("Near Distance: ", &camFrustrum.nearPlaneDistance, 0.1f, 0.01f, camFrustrum.farPlaneDistance);
		ImGui::DragFloat("Far Distance: ", &camFrustrum.farPlaneDistance, 0.1f, camFrustrum.nearPlaneDistance, 10000.f);

		ImGui::Separator();

		if (camFrustrum.type == FrustumType::PerspectiveFrustum)
		{
			ImGui::Text("Vertical FOV: %f", camFrustrum.verticalFov);
			ImGui::Text("Horizontal FOV: %f", camFrustrum.horizontalFov);
			ImGui::Separator();
			if (ImGui::DragFloat("FOV: ", &fov, 0.1f, 1.0f, 180.f))
			{
				SetVerticalFOV(fov);
			}
		}
		else
		{
			if (ImGui::DragFloat("Size: ", &orthoSize, 0.01f, 0.01f, 100.0f))
			{
				//camFrustrum.verticalFov = fov * DEGTORAD;
				//camFrustrum.orthographicWidth = 1920 / orthoSize;
				//camFrustrum.orthographicHeight = 1080 / orthoSize;
			}
		}



		if (ImGui::BeginCombo("Frustrum Type", (camFrustrum.type == FrustumType::PerspectiveFrustum) ? "Prespective" : "Orthographic"))
		{
			if (ImGui::Selectable("Perspective"))
			{
				camFrustrum.type = FrustumType::PerspectiveFrustum;
				camFrustrum.verticalFov = 60.0f * DEGTORAD;
				camFrustrum.horizontalFov = 2.0f * atanf(tanf(camFrustrum.verticalFov / 2.0f) * 1.7f);
			}

			if (ImGui::Selectable("Orthographic"))
			{
				camFrustrum.type = FrustumType::OrthographicFrustum;
				SetOrthSize(orthoSize);
			}

			ImGui::EndCombo();
		}

		ImGui::Text("Camera Culling: "); ImGui::SameLine();
		ImGui::Checkbox("##cameraCulling", &cullingState);

		ImGui::Text("Draw Skybox: "); ImGui::SameLine();
		ImGui::Checkbox("##drawSkybox", &drawSkybox);

		ImGui::Text("MSAA Samples: "); ImGui::SameLine();
		if (ImGui::SliderInt("##msaasamp", &msaaSamples, 1, 8))
		{
			msaaFBO.ReGenerateBuffer(msaaFBO.texBufferSize.x, msaaFBO.texBufferSize.y, msaaSamples);
			resolvedFBO.ReGenerateBuffer(resolvedFBO.texBufferSize.x, resolvedFBO.texBufferSize.y);
		}

		if (ImGui::Button("Set as Game Camera"))
		{
			EngineExternal->moduleRenderer3D->SetGameRenderTarget(this);
		}

		ImGui::Separator();
		ImGui::Spacing();
		ImGui::Text("Drop here to change profile");
		ImGui::Spacing();

		if (ImGui::BeginDragDropTarget())
		{
			if (const ImGuiPayload * payload = ImGui::AcceptDragDropPayload("_PPROCESS"))
			{
				std::string* assetsPath = (std::string*)payload->Data;

				ResourcePostProcess* newProfile = dynamic_cast<ResourcePostProcess*>(EngineExternal->moduleResources->RequestFromAssets(assetsPath->c_str()));

				SetPostProcessProfile(newProfile);
			}
			ImGui::EndDragDropTarget();
		}

		if (postProcessProfile == nullptr)
		{
			DrawCreationWindow();
			if (ImGui::Button("Create new Profile ##Post Processing Profile"))
			{
				ImGui::OpenPopup("Create new Profile##CamProfile", ImGuiPopupFlags_NoOpenOverExistingPopup);
			}
		}
		else
		{
			postProcessProfile->DrawEditor("##Post Processing Profile");
			ImGui::SameLine();
			if (ImGui::Button("Erase Profile##CamProfile"))
			{
				SetPostProcessProfile(nullptr);
			}
		}

		








		return true;
	}
	return false;
}
#endif // !STANDALONE

void C_Camera::PostUpdate()
{

	//Maybe dont update every frame?
	camFrustrum.pos = gameObject->transform->globalTransform.TranslatePart();
	camFrustrum.front = gameObject->transform->GetForward();
	camFrustrum.up = gameObject->transform->GetUp();

#ifndef STANDALONE
	float3 points[8];
	camFrustrum.GetCornerPoints(points);

	ModuleRenderer3D::DrawBox(points, float3(0.180f, 1.f, 0.937f));
#endif // !STANDALONE

}


void C_Camera::SaveData(JSON_Object* nObj)
{
	Component::SaveData(nObj);

	DEJson::WriteInt(nObj, "fType", camFrustrum.type);

	if (camFrustrum.type == FrustumType::OrthographicFrustum)
		DEJson::WriteFloat(nObj, "fSize", orthoSize);

	DEJson::WriteFloat(nObj, "nearPlaneDist", camFrustrum.nearPlaneDistance);
	DEJson::WriteFloat(nObj, "farPlaneDist", camFrustrum.farPlaneDistance);

	DEJson::WriteFloat(nObj, "vFOV", camFrustrum.verticalFov);
	DEJson::WriteFloat(nObj, "hFOV", camFrustrum.horizontalFov);
	DEJson::WriteBool(nObj, "culling", cullingState);

	DEJson::WriteBool(nObj, "HDR", isHDR);

	DEJson::WriteBool(nObj, "drawSkybox", drawSkybox);

	if (postProcessProfile != nullptr)
	{
		DEJson::WriteInt(nObj, "ProfileUID", postProcessProfile->GetUID());
	}
}

void C_Camera::LoadData(DEConfig& nObj)
{
	Component::LoadData(nObj);

	camFrustrum.type = (FrustumType)nObj.ReadInt("fType");

	if (camFrustrum.type == FrustumType::OrthographicFrustum)
		orthoSize = nObj.ReadFloat("fSize");

	camFrustrum.nearPlaneDistance = nObj.ReadFloat("nearPlaneDist");
	camFrustrum.farPlaneDistance = nObj.ReadFloat("farPlaneDist");

	camFrustrum.verticalFov = nObj.ReadFloat("vFOV");
	camFrustrum.horizontalFov = nObj.ReadFloat("hFOV");
	cullingState = nObj.ReadBool("culling");

	ChangeHDR(nObj.ReadBool("HDR"));

	drawSkybox = nObj.ReadBool("drawSkybox");

	EngineExternal->moduleScene->SetGameCamera(this);

	if (nObj.ReadInt("ProfileUID") != 0)
	{
		SetPostProcessProfile(dynamic_cast<ResourcePostProcess*>(EngineExternal->moduleResources->RequestResource(nObj.ReadInt("ProfileUID"), Resource::Type::POSTPROCESS)));
	}
}

void C_Camera::StartDraw()
{
	EngineExternal->moduleRenderer3D->activeRenderCamera = this;

#ifdef STANDALONE
	if (camFrustrum.type == FrustumType::PerspectiveFrustum)
		SetAspectRatio(this->windowWidth / this->windowHeight);
	else {
		camFrustrum.orthographicWidth = this->windowWidth / orthoSize;
		camFrustrum.orthographicHeight = this->windowHeight / orthoSize;
	}
#endif // !STANDALONE


	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);

	glStencilOp(GL_KEEP, GL_KEEP, GL_REPLACE);

	PushCameraMatrix();

	//glBindFramebuffer(GL_FRAMEBUFFER, msaaFBO.GetFrameBuffer());
	msaaFBO.BindFrameBuffer();
	glClearColor(EngineExternal->moduleRenderer3D->clearColor.x, EngineExternal->moduleRenderer3D->clearColor.y, EngineExternal->moduleRenderer3D->clearColor.z, EngineExternal->moduleRenderer3D->clearColor.w);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

	//glEnable(GL_DEPTH_TEST);

	//glLoadIdentity();

	//glMatrixMode(GL_MODELVIEW);
	//glLoadMatrixf(camFrustrum.ViewMatrix().ptr());
}

void C_Camera::EndDraw()
{
	msaaFBO.ResolveToFBO(resolvedFBO);

	glDisable(GL_DEPTH_TEST);
	EngineExternal->moduleRenderer3D->activeRenderCamera = nullptr;
	glClearColor(0.0f,0.0f,0.0f,1.0f);

}

void C_Camera::ReGenerateBuffer(int w, int h)
{
	windowWidth = w;
	windowHeight = h;

	SetAspectRatio((float)w / (float)h);

	msaaFBO.ReGenerateBuffer(w, h, msaaSamples);
	resolvedFBO.ReGenerateBuffer(w, h);
}

void C_Camera::PushCameraMatrix()
{
	glLoadIdentity();
	glMatrixMode(GL_PROJECTION);
	glLoadMatrixf((GLfloat*)ProjectionMatrixOpenGL().v);

	glMatrixMode(GL_MODELVIEW);
	glLoadMatrixf((GLfloat*)ViewMatrixOpenGL().v);
}

void C_Camera::SetPostProcessProfile(ResourcePostProcess* newProfile)
{
	if (postProcessProfile != nullptr)
	{
		EngineExternal->moduleResources->UnloadResource(postProcessProfile->GetUID());
	}
	postProcessProfile = newProfile;
}

void C_Camera::DrawCreationWindow()
{
	if (ImGui::BeginPopup("Create new Profile##CamProfile", ImGuiWindowFlags_AlwaysAutoResize))
	{

		static char name[50] = "\0";

		ImGui::Text("Profile Name:"); ImGui::SameLine();

		std::string id("##");
		id += ".pprocess";

		ImGui::InputText(id.c_str(), name, sizeof(char) * 50);
		if (ImGui::Button("Create##CamProfile"))
		{
			std::string path = "Assets/PostProcessingProfiles/";
			path += name;

			if (path.find('.') == path.npos)
				path += ".pprocess";

			//TODO: Check if the extension is correct, to avoid a .cs.glsl file
			if (path.find(".pprocess") != path.npos)
			{

				SetPostProcessProfile(PostProcessImporter::CreateBaseProfileFile(path.c_str()));

				name[0] = '\0';
			}

			ImGui::CloseCurrentPopup();
		}

		ImGui::EndPopup();
	}
}

void C_Camera::ChangeHDR(bool isHDR)
{
	if (this->isHDR != isHDR)
	{
		this->isHDR = isHDR;
		msaaFBO.ReGenerateBuffer(msaaFBO.texBufferSize.x, msaaFBO.texBufferSize.y, msaaFBO.GetMSAA(), this->isHDR);
		resolvedFBO.ReGenerateBuffer(resolvedFBO.texBufferSize.x, resolvedFBO.texBufferSize.y, resolvedFBO.GetMSAA(), this->isHDR);
	}
}

void C_Camera::SetCameraToPerspective()
{
	if (camFrustrum.type == FrustumType::PerspectiveFrustum)
		return;

	camFrustrum.type = FrustumType::PerspectiveFrustum;

	SetVerticalFOV(fov);
}

void C_Camera::SetCameraToOrthographic()
{
	if (camFrustrum.type == FrustumType::OrthographicFrustum)
		return;

	camFrustrum.type = FrustumType::OrthographicFrustum;
}

void C_Camera::SetVerticalFOV(float _verticalFOV)
{
	fov = _verticalFOV;
	camFrustrum.verticalFov = _verticalFOV * DEGTORAD;
	camFrustrum.horizontalFov = 2.0f * atanf(tanf(camFrustrum.verticalFov / 2.0f) * 1.7f);
}

void C_Camera::SetHorizontalFOV(float horizontalFOV)
{
	camFrustrum.horizontalFov = horizontalFOV * DEGTORAD;
	camFrustrum.verticalFov = 2.0f * atanf(tanf(camFrustrum.horizontalFov / 2.0f) * 1.7f);
	fov = camFrustrum.verticalFov;
}

void C_Camera::LookAt(const float3& Spot)
{
	/*Reference = Spot;*/
	camFrustrum.front = (Spot - camFrustrum.pos).Normalized();
	float3 X = float3(0, 1, 0).Cross(camFrustrum.front).Normalized();
	camFrustrum.up = camFrustrum.front.Cross(X);
}

void C_Camera::LookAt(Frustum& camFrustrum, const float3& Spot)
{
	/*Reference = Spot;*/
	camFrustrum.front = (Spot - camFrustrum.pos).Normalized();
	float3 X = float3(0, 1, 0).Cross(camFrustrum.front).Normalized();
	camFrustrum.up = camFrustrum.front.Cross(X);
}

void C_Camera::Move(const float3& Movement)
{
	camFrustrum.pos += Movement;
}

float3 C_Camera::GetPosition()
{
	return camFrustrum.pos;
}

void C_Camera::SetOrthSize(float size)
{
	orthoSize = size;
}

float C_Camera::GetOrthSize()
{
	return orthoSize;
}

float4x4 C_Camera::ViewMatrixOpenGL() const
{
	math::float4x4 mat = camFrustrum.ViewMatrix();
	return mat.Transposed();
}

float4x4 C_Camera::ProjectionMatrixOpenGL() const
{
	return camFrustrum.ProjectionMatrix().Transposed();
}

void C_Camera::SetAspectRatio(float aspectRatio)
{
	camFrustrum.horizontalFov = 2.f * atanf(tanf(camFrustrum.verticalFov * 0.5f) * aspectRatio);
}

void C_Camera::SetAsGameCamera()
{
	EngineExternal->moduleRenderer3D->SetGameRenderTarget(this);
}

bool C_Camera::IsInsideFrustum(AABB& globalAABB)
{
	return (this->camFrustrum.type == FrustumType::PerspectiveFrustum) ? PrespectiveCulling(globalAABB) : OrthoCulling(globalAABB);
}
bool C_Camera::OrthoCulling(AABB& globalAABB)
{
	float3 obbPoints[8];


	float3 orthoNormals[6];
	orthoNormals[0] = -camFrustrum.front;
	orthoNormals[1] = camFrustrum.front;
	orthoNormals[2] = -camFrustrum.front.Cross(camFrustrum.up);
	orthoNormals[3] = camFrustrum.front.Cross(camFrustrum.up);
	orthoNormals[4] = camFrustrum.up;
	orthoNormals[5] = -camFrustrum.up;

	float3 planePoints[6];
	planePoints[0] = camFrustrum.pos;
	planePoints[1] = camFrustrum.pos + (camFrustrum.front * camFrustrum.farPlaneDistance);
	planePoints[2] = (camFrustrum.pos + (camFrustrum.front * (camFrustrum.farPlaneDistance / 2))) + (orthoNormals[2] * (camFrustrum.orthographicWidth / 2));
	planePoints[3] = (camFrustrum.pos + (camFrustrum.front * (camFrustrum.farPlaneDistance / 2))) + (orthoNormals[3] * (camFrustrum.orthographicWidth / 2));
	planePoints[4] = (camFrustrum.pos + (camFrustrum.front * (camFrustrum.farPlaneDistance / 2))) + (orthoNormals[4] * (camFrustrum.orthographicHeight / 2));
	planePoints[5] = (camFrustrum.pos + (camFrustrum.front * (camFrustrum.farPlaneDistance / 2))) + (orthoNormals[5] * (camFrustrum.orthographicHeight / 2));

	int totalIn = 0;
	globalAABB.GetCornerPoints(obbPoints);

	for (size_t i = 2; i < 6; i++)
	{
		int inCount = 8;
		int iPtIn = 1;

		for (size_t k = 0; k < 8; k++)
		{
			//Is "IsOnPositiveSide" slow?
			//if (camFrustrumumPlanes[i].IsOnPositiveSide(obbPoints[k]))
			if (orthoNormals[i].Dot(obbPoints[k]) - Dot(planePoints[i], orthoNormals[i]) >= 0.f)
			{
				iPtIn = 0;
				--inCount;
			}
			if (inCount == 0)
				return false;

			totalIn += iPtIn;
		}
	}

	if (totalIn == 6)
		return true;

	return true;
}

bool C_Camera::PrespectiveCulling(AABB& globalAABB)
{
	float3 obbPoints[8];
	Plane camFrustrumumPlanes[6];

	int totalIn = 0;

	globalAABB.GetCornerPoints(obbPoints);
	camFrustrum.GetPlanes(camFrustrumumPlanes);

	for (size_t i = 0; i < 6; i++)
	{
		int inCount = 8;
		int iPtIn = 1;

		for (size_t k = 0; k < 8; k++)
		{
			//Is "IsOnPositiveSide" slow?
			if (camFrustrumumPlanes[i].IsOnPositiveSide(obbPoints[k]))
			{
				iPtIn = 0;
				--inCount;
			}
			if (inCount == 0)
				return false;

			totalIn += iPtIn;
		}
	}

	if (totalIn == 6)
		return true;

	return true;
}