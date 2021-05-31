#include "CO_ProceduralMesh.h"

#include "ImGui/imgui.h"


const int VERTICES_PER_SQUARE = 3 * 2;
const int VERTEX_SIZE_BYTES = 2 * sizeof(float) + 4 * sizeof(float); //2 floats (pos) + 4 indicators
const int VERTEX_ATRIBUTE_DATA = 6;

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

	std::vector<float> buffer;
	buffer.reserve(totalVertexCount * VERTEX_ATRIBUTE_DATA);

	for (int i = 0; i < rows; ++i)
	{
		for (int j = 0; j < columns; ++j)
		{
			StoreGridScuare(j, i, buffer);
		}
	}
}


void C_ProceduralMesh::StoreGridScuare(int col, int row, std::vector<float>& buffer)
{
	std::vector<float[2]> cornerPositions;
	float aux[2];

	aux[0] = col;
	aux[1] = row;
	cornerPositions.push_back(aux);

	aux[0] = col;
	aux[1] = row + 1;
	cornerPositions.push_back(aux);

	aux[0] = col + 1;
	aux[1] = row;
	cornerPositions.push_back(aux);

	aux[0] = col + 1;
	aux[1] = row + 1;
	cornerPositions.push_back(aux);
}