#include "Application.h"
#include "DEResource.h"
#include "IM_FileSystem.h"
#include "MO_ResourceManager.h"
#include "RE_Animation.h"
#include "IM_AnimationImporter.h"

ResourceAnimation::ResourceAnimation(unsigned int _uid) : Resource(_uid, Resource::Type::ANIMATION), animationName("No Name"),duration(0),ticksPerSecond(0), loop(true), time(0.0f)
{
}

ResourceAnimation::~ResourceAnimation()
{}

bool ResourceAnimation::LoadToMemory()
{
	LOG(LogType::L_WARNING, "Animation loaded to memory");

	LoadCustomFormat(GetLibraryPath());

	return true;
}

bool ResourceAnimation::UnloadFromMemory()
{
	channels.clear();

	return false;
}

uint ResourceAnimation::SaveCustomFormat(ResourceAnimation* animation, char** buffer)
{
	uint size =
		32 //name size 
		+ sizeof(float)  //duration
		+ sizeof(float)  //ticks per second
		+ sizeof(bool)   //loop
		+ sizeof(uint);  //channels size

	//Channels size 
	std::map<std::string, Channel>::const_iterator it;
	for (it = animation->channels.begin(); it != animation->channels.end(); ++it)
		size += AnimationLoader::GetChannelsSize(it->second);

	//Allocate buffer size
	*buffer = new char[size];
	char* cursor = *buffer;

	//Name
	memcpy(cursor, &animation->animationName, 32);
	cursor += sizeof(animationName);

	//Duration
	memcpy(cursor, &animation->duration, sizeof(float));
	cursor += sizeof(float);

	//Ticks per second
	memcpy(cursor, &animation->ticksPerSecond, sizeof(float));
	cursor += sizeof(float);

	//loop
	memcpy(cursor, &animation->loop, sizeof(bool)); 
	cursor += sizeof(bool);

	//Channels number
	uint channelsSize = animation->channels.size();
	memcpy(cursor, &channelsSize, sizeof(uint));
	cursor += sizeof(uint);

	//Save each channel data
	for (it = animation->channels.begin(); it != animation->channels.end(); ++it)
		AnimationLoader::SaveChannels(it->second, &cursor);

	return size;
}

void ResourceAnimation::LoadCustomFormat(const char* path)
{
	char* buffer = nullptr;

	uint size = FileSystem::LoadToBuffer(path, &buffer);

	const char* cursor = buffer;
	uint bytes;

	//Name
	memcpy(&animationName, cursor, 32);
	cursor += sizeof(animationName);

	//Fills Duration
	memcpy(&duration, cursor, sizeof(float));
	cursor += sizeof(float);

	//Fills Ticks per second
	memcpy(&ticksPerSecond, cursor, sizeof(float));
	cursor += sizeof(float);
	
	//loop
	memcpy(&loop, cursor, sizeof(bool)); //Comment this to load animations the old way
	cursor += sizeof(bool);

	//Fills Channels number
	uint channelsSize = 0;
	memcpy(&channelsSize, cursor, sizeof(uint));
	cursor += sizeof(uint);

	//Fills channels
	for (uint i = 0; i < channelsSize; ++i)
	{
		Channel Channelat;
		AnimationLoader::LoadChannels(Channelat, &cursor);
		channels[Channelat.boneName.c_str()] = Channelat;
	}

	RELEASE_ARRAY(buffer);
}

Channel::~Channel()
{
	boneName.clear();
	positionKeys.clear();
	rotationKeys.clear();
	scaleKeys.clear();
}

std::map<double, float3>::const_iterator Channel::GetPrevPosKey(double currentKey) const
{
	std::map<double, float3>::const_iterator ret = positionKeys.lower_bound(currentKey);
	if (ret != positionKeys.begin())
		ret--;

	return ret;
}

std::map<double, float3>::const_iterator Channel::GetNextPosKey(double currentKey) const
{
	std::map<double, float3>::const_iterator ret = positionKeys.find(currentKey);
	if (ret == positionKeys.end()) {
		ret = positionKeys.upper_bound(currentKey);

		if (ret == positionKeys.end())
			ret--;
	}
	return ret;
}

std::map<double, Quat>::const_iterator Channel::GetPrevRotKey(double currentKey) const
{
	std::map<double, Quat>::const_iterator ret = rotationKeys.lower_bound(currentKey);
	if (ret != rotationKeys.begin())
		ret--;
	return ret;
}

std::map<double, Quat>::const_iterator Channel::GetNextRotKey(double currentKey) const
{
	std::map<double, Quat>::const_iterator ret = rotationKeys.find(currentKey);
	if (ret == rotationKeys.end()) {
		ret = rotationKeys.upper_bound(currentKey);

		if (ret == rotationKeys.end())
			ret--;
	}
	return ret;
}

std::map<double, float3>::const_iterator Channel::GetPrevScaleKey(double currentKey) const
{
	std::map<double, float3>::const_iterator ret = scaleKeys.lower_bound(currentKey);
	if (ret != scaleKeys.begin())
		ret--;
	return ret;
}

std::map<double, float3>::const_iterator Channel::GetNextScaleKey(double currentKey) const
{
	std::map<double, float3>::const_iterator ret = scaleKeys.find(currentKey);
	if (ret == scaleKeys.end()) {
		ret = scaleKeys.upper_bound(currentKey);

		if (ret == scaleKeys.end())
			ret--;
	}

	return ret;
}
