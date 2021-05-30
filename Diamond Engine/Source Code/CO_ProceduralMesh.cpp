#include "CO_ProceduralMesh.h"

#include "ImGui/imgui.h"


const int VERTICES_PER_SQUARE = 3 * 2;
const int VERTEX_SIZE_BYTES = 8 + 4; //2 floats (pos) + 4 bytes

C_ProceduralMesh::C_ProceduralMesh(GameObject* gameObject) : Component(gameObject),
	rows(0u),
	columns(0u)
{
	name = "Procedural mesh";
}


C_ProceduralMesh::~C_ProceduralMesh()
{
	rows = 0u;
	columns = 0u;
}


void C_ProceduralMesh::Update()
{

}


#ifndef STANDALONE
bool C_ProceduralMesh::OnEditor()
{
	if (Component::OnEditor() == true)
	{
		ImGui::Separator();

		int r = rows;
		int c = columns;

		ImGui::InputInt("Mesh rows", &r);
		ImGui::InputInt("Mesh columns", &c);

		if (rows != (unsigned int)r || columns != (unsigned int)c)
		{
			rows = r;
			columns = c;
			RecalculateMesh();
		}
	}
	return true;
}
#endif // !STANDALONE


void C_ProceduralMesh::SaveData(JSON_Object* nObj)
{

}


void C_ProceduralMesh::LoadData(DEConfig& nObj)
{

}


void C_ProceduralMesh::RecalculateMesh()
{
	int gridCount = rows * columns;
	int totalVertexCount = gridCount * gridCount * VERTICES_PER_SQUARE;

	
}