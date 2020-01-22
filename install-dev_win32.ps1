New-Item -Path "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP" -Force
Remove-Item -Path "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP\IndustryLP.dll" -Force
Copy-Item -Path "IndustryLP\bin\Debug\IndustryLP.dll" -Destination "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP\" -Force