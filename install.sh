xbuild IndustryLP.sln
mkdir -p -v "$HOME/.local/share/Colossal Order/Cities_Skylines/Addons/Mods/IndustryLP"
rm -f -v "$HOME/.local/share/Colossal Order/Cities_Skylines/Addons/Mods/IndustryLP/IndustryLP.dll"
cp -f -v "IndustryLP/bin/Debug/IndustryLP.dll" "$HOME/.local/share/Colossal Order/Cities_Skylines/Addons/Mods/IndustryLP/"