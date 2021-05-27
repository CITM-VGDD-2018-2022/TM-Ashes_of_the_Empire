#ifndef __CO_IMAGE2D_H__
#define __CO_IMAGE2D_H__

#include "Component.h"
#include "DEResource.h"

class ResourceTexture;
class ResourceMaterial;

class C_Image2D : public Component
{
public:
	C_Image2D(GameObject* gameObject);
	~C_Image2D() override;

#ifndef STANDALONE
	bool OnEditor() override;
#endif // !STANDALONE

	void RenderImage(float* transform, ResourceMaterial* material, unsigned int VAO);

	ResourceTexture* GetTexture() const;
	void SetTexture(ResourceTexture* tex);
	void SetTexture(int UID, const char* library_path);
	void SetTexture(int UID, Resource::Type _type);

	void SetBlendTexture(ResourceTexture* bTexture);

	void SetFadeValue(float fadeValue);


	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

public:
	float blendValue;

private:
	ResourceTexture* texture = nullptr;
	ResourceTexture* blendTexture = nullptr;

	float fadeValue = 1.0f;
};

#endif // !__CO_IMAGE2D_H__
