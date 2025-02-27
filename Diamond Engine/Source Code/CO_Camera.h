#pragma once
#include "Component.h"
#include"DE_FrameBuffer.h"
#include "DE_Advanced_FrameBuffer.h"

#include"MathGeoLib/include/Geometry/Frustum.h"
class ResourcePostProcess;

class C_Camera : public Component
{
public:
	C_Camera();
	C_Camera(GameObject* _gm);
	virtual ~C_Camera();

#ifndef STANDALONE
	bool OnEditor() override;
#endif // !STANDALONE

	void PostUpdate() override;

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

	float4x4 ViewMatrixOpenGL() const;
	float4x4 ProjectionMatrixOpenGL() const;

	void SetAspectRatio(float aspectRatio);

	void SetAsGameCamera();

	void StartDraw();
	void EndDraw();

	void ReGenerateBuffer(int w, int h);

	void PushCameraMatrix();

	void SetPostProcessProfile(ResourcePostProcess* newProfile);
	void DrawCreationWindow();

	inline bool GetIsHDR() { return isHDR; }
	void ChangeHDR(bool isHDR);
	//DE_FrameBuffer resolvedFBO;
	//DE_FrameBuffer msaaFBO;

	DE_Advanced_FrameBuffer resolvedFBO;
	DE_Advanced_FrameBuffer msaaFBO;
	void SetCameraToPerspective();
	void SetCameraToOrthographic();
	void SetVerticalFOV(float verticalFOV);
	void SetHorizontalFOV(float horizontalFOV);

public:


	Frustum camFrustrum;
	float fov;
	float orthoSize;
	bool cullingState, drawSkybox;

	float windowWidth;
	float windowHeight;
	ResourcePostProcess* postProcessProfile;

//Movement logic
public: 
	void LookAt(const float3& Spot);
	static void LookAt(Frustum& frust, const float3& Spot);

	void Move(const float3& Movement);
	float3 GetPosition();
	void SetOrthSize(float size);
	float GetOrthSize();

	bool IsInsideFrustum(AABB& globalAABB);

private:
	bool OrthoCulling(AABB& globalAABB);
	bool PrespectiveCulling(AABB& globalAABB);

	int msaaSamples;
	bool isHDR;
	float verticalFOV;
};