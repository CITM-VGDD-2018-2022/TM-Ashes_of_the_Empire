#ifndef __CO_LOW_POLY_WATER_H__
#define __CO_LOW_POLY_WATER_H__

#include "Component.h"

class C_ProceduralMesh : public Component
{
public:
	C_ProceduralMesh(GameObject* gameObject);
	~C_ProceduralMesh() override;

	void Update() override;

#ifndef STANDALONE
	bool OnEditor() override;
#endif // !STANDALONE

	static inline TYPE GetType() { return TYPE::PROCEDURAL_MESH; }; //This will allow us to get the type from a template

	void SaveData(JSON_Object* nObj) override;
	void LoadData(DEConfig& nObj) override;

private:
	void RecalculateMesh();
	void StoreGridScuare(int col, int row, std::vector<float>& buffer);
private:

	//VBO and draw shit

	//Counter of rows and columns
	unsigned int rows = 0u;
	unsigned int columns = 0u;
};

#endif // !__CO_LOW_POLY_WATER_H__