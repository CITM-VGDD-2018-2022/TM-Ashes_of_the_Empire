#include "IM_ModelImporter.h"
#include"GameObject.h"

#include "Assimp/include/cimport.h"
#include "Assimp/include/scene.h"
#include "Assimp/include/postprocess.h"
#include "Assimp/include/cfileio.h"

#include"MO_Scene.h"
#include"MO_ResourceManager.h"

#include"IM_FileSystem.h"
#include"IM_MeshLoader.h"
#include "IM_AnimationImporter.h"

#include"RE_Mesh.h"
#include"RE_Texture.h"
#include "RE_Animation.h"
#include"DEResource.h"

void ModelImporter::Import(char* buffer, int bSize, Resource* res)
{
	const aiScene* scene = aiImportFileFromMemory(buffer, bSize, aiProcessPreset_TargetRealtime_MaxQuality, nullptr);

	if (scene != nullptr && scene->HasMeshes())
	{
		std::vector<ResourceMesh*> meshesOnModel;
		std::vector<ResourceTexture*> texturesOnModel;
		std::vector<ResourceAnimation*> animationsOnModel;

		//This should not be here
		if (scene->HasMaterials())
		{
			//Needs to be moved to another place
			std::string generalPath(res->GetAssetPath());
			generalPath = generalPath.substr(0, generalPath.find_last_of("/\\") + 1);
			for (size_t k = 0; k < scene->mNumMaterials; k++)
			{
				aiMaterial* material = scene->mMaterials[k];
				uint numTextures = material->GetTextureCount(aiTextureType_DIFFUSE);

				if (numTextures > 0)
				{
					aiString path;
					material->GetTexture(aiTextureType_DIFFUSE, 0, &path);

					std::string localPath = generalPath;
					localPath = localPath.substr(0, localPath.find_last_of('/') + 1);
					localPath += FileSystem::NormalizePath(path.C_Str());

					std::string libraryPath = EngineExternal->moduleResources->GetMetaPath(localPath.c_str());
					uint UID = EngineExternal->moduleResources->GetMetaUID(libraryPath.c_str());
					libraryPath = EngineExternal->moduleResources->LibraryFromMeta(libraryPath.c_str());

					ResourceTexture* texture = dynamic_cast<ResourceTexture*>(EngineExternal->moduleResources->RequestResource(UID, libraryPath.c_str()));

					texturesOnModel.push_back(texture);
					path.Clear();
				}
				else
				{
					texturesOnModel.push_back(nullptr); //Empty texture
				}
			}
		}

		//Load mesh uid from meta file
		std::vector<uint> meshUIDs;
		if (FileSystem::Exists(EngineExternal->moduleResources->GetMetaPath(res->GetAssetPath()).c_str()))
			GetMeshesFromMeta(res->GetAssetPath(), meshUIDs);

		if (scene->HasMeshes())
		{
			for (unsigned int i = 0; i < scene->mNumMeshes; i++)
			{
				if(meshUIDs.size() != 0)
					meshesOnModel.push_back(MeshLoader::LoadMesh(scene->mMeshes[i], meshUIDs[i]));
				else
					meshesOnModel.push_back(MeshLoader::LoadMesh(scene->mMeshes[i]));
			}
		}

		SaveMeshesToMeta(res->GetAssetPath(), meshesOnModel);
		EngineExternal->moduleResources->UpdateMeshesDisplay();

		std::vector<uint> animationsUIDs;
		if (FileSystem::Exists(EngineExternal->moduleResources->GetMetaPath(res->GetAssetPath()).c_str()))
			GetAnimationsFromMeta(res->GetAssetPath(), animationsUIDs);

		if (scene->HasAnimations())
		{
			for (unsigned int i = 0; i < scene->mNumAnimations; i++)
			{
				if (animationsUIDs.size() != 0)
					animationsOnModel.push_back(AnimationLoader::ImportAnimation(scene->mAnimations[i], animationsUIDs[i]));
				else
					animationsOnModel.push_back(AnimationLoader::ImportAnimation(scene->mAnimations[i]));
			}
		}

		SaveAnimationsToMeta(res->GetAssetPath(), animationsOnModel);
		EngineExternal->moduleResources->UpdateAnimationsDisplay();

		//Save custom format model
		GameObject* root = new GameObject("First model GO", nullptr);

		std::string name = "";
		FileSystem::GetFileName(res->GetAssetPath(), name, false);

		MeshLoader::NodeToGameObject(scene->mMeshes, texturesOnModel, meshesOnModel, scene->mRootNode, root, name.c_str());

		SaveModelCustom(root->children[0], res->GetLibraryPath());
		for (size_t i = 0; i < meshesOnModel.size(); i++)
		{
			meshesOnModel[i]->UnloadFromMemory();
		}
		
		delete root;
		root = nullptr;

		for (size_t i = 0; i < meshesOnModel.size(); i++) {
			meshesOnModel[i] = nullptr;
		}
		meshesOnModel.clear();

		for (size_t i = 0; i < texturesOnModel.size(); i++) {
			texturesOnModel[i] = nullptr;
		}
		texturesOnModel.clear();

		for (size_t i = 0; i < animationsOnModel.size(); i++) 
		{
			EngineExternal->moduleResources->UnloadResource(animationsOnModel[i]->GetUID());
		}
		animationsOnModel.clear();
		meshUIDs.clear();
		animationsUIDs.clear();

		aiReleaseImport(scene);
		scene = nullptr;
	}
	else
		LOG(LogType::L_ERROR, "Error loading scene"/*, scene->name*/);
}

void ModelImporter::SaveModelCustom(GameObject* root, const char* nameWithExtension)
{
	JSON_Value* file = json_value_init_object();
	JSON_Object* root_object = json_value_get_object(file);

	JSON_Value* goArray = json_value_init_array();
	root->SaveToJson(json_value_get_array(goArray));
	json_object_set_value(root_object, "Model Objects", goArray);

	//Save file
	json_serialize_to_file_pretty(file, nameWithExtension);

	//Free memory
	json_value_free(file);
}

void ModelImporter::LoadModelCustom(const char* nameWithExtension)
{
	JSON_Value* scene = json_parse_file(nameWithExtension);

	if (scene == NULL)
		return;

	JSON_Object* sceneObj = json_value_get_object(scene);
	JSON_Array* sceneGO = json_object_get_array(sceneObj, "Model Objects");

	JSON_Object* goJsonObj = json_array_get_object(sceneGO, 0);
	GameObject* parent = EngineExternal->moduleScene->root;

	for (size_t i = 0; i < json_array_get_count(sceneGO); i++)
	{
		goJsonObj = json_array_get_object(sceneGO, i);
		GameObject* originalParent = parent;

		while (parent != nullptr && json_object_get_number(goJsonObj, "ParentUID") != parent->UID)
			parent = parent->parent;

		if (parent == nullptr)
			parent = originalParent;

		parent = new GameObject(json_object_get_string(goJsonObj, "name"), parent, json_object_get_number(goJsonObj, "UID"));
		parent->LoadFromJson(goJsonObj);
	}
	
	//Free memory
	json_value_free(scene);
}

void ModelImporter::SaveMeshesToMeta(const char* assetFile, std::vector<ResourceMesh*>& meshes)
{
	JSON_Value* metaFile = json_parse_file(EngineExternal->moduleResources->GetMetaPath(assetFile).c_str());

	if (metaFile == NULL)
		return;

	JSON_Object* sceneObj = json_value_get_object(metaFile);

	JSON_Value* arrayVal = json_value_init_array();
	JSON_Array* meshArray = json_value_get_array(arrayVal);

	for (size_t i = 0; i < meshes.size(); i++)
	{
		json_array_append_number(meshArray, meshes[i]->GetUID());
	}
	json_object_set_value(sceneObj, "Meshes Inside", arrayVal);

	json_serialize_to_file_pretty(metaFile, EngineExternal->moduleResources->GetMetaPath(assetFile).c_str());

	//Free memory
	json_value_free(metaFile);
}

void ModelImporter::GetMeshesFromMeta(const char* assetFile, std::vector<uint>& uids)
{
	JSON_Value* metaFile = json_parse_file(EngineExternal->moduleResources->GetMetaPath(assetFile).c_str());

	if (metaFile == NULL)
		return;

	JSON_Object* sceneObj = json_value_get_object(metaFile);

	JSON_Array* meshArray = json_object_get_array(sceneObj, "Meshes Inside");

	for (size_t i = 0; i < json_array_get_count(meshArray); i++)
	{
		uids.push_back(json_array_get_number(meshArray, i));
	}

	//Free memory
	json_value_free(metaFile);
}

void ModelImporter::SaveAnimationsToMeta(const char* assetFile, std::vector<ResourceAnimation*>& animations)
{
	JSON_Value* metaFile = json_parse_file(EngineExternal->moduleResources->GetMetaPath(assetFile).c_str());

	if (metaFile == NULL)
		return;

	JSON_Object* sceneObj = json_value_get_object(metaFile);

	JSON_Value* arrayVal = json_value_init_array();
	JSON_Array* meshArray = json_value_get_array(arrayVal);

	for (size_t i = 0; i < animations.size(); i++)
	{
		json_array_append_number(meshArray, animations[i]->GetUID());
	}
	json_object_set_value(sceneObj, "Animations Inside", arrayVal);

	json_serialize_to_file_pretty(metaFile, EngineExternal->moduleResources->GetMetaPath(assetFile).c_str());

	//Free memory
	json_value_free(metaFile);
}

void ModelImporter::GetAnimationsFromMeta(const char* assetFile, std::vector<uint>& uids)
{
	JSON_Value* metaFile = json_parse_file(EngineExternal->moduleResources->GetMetaPath(assetFile).c_str());

	if (metaFile == NULL)
		return;

	JSON_Object* sceneObj = json_value_get_object(metaFile);

	JSON_Array* animationsArray = json_object_get_array(sceneObj, "Animations Inside");

	for (size_t i = 0; i < json_array_get_count(animationsArray); i++)
	{
		double animationNumber = json_array_get_number(animationsArray, i);

		if (animationNumber > 0)
			uids.push_back(animationNumber);
	}

	//Free memory
	json_value_free(metaFile);
}