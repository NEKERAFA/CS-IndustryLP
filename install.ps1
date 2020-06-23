try {
    if (!(Test-Path "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP")) {
        Write-Host "Creating IndustryLP Folder..." -ForegroundColor Green
        New-Item -Path "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP\runtimes\win-x64\native" -ItemType Directory | Out-Null
    }

    Write-Host "Copying clingo deprencencies..." -ForegroundColor Green
    Copy-Item "ClingoSharp\clingo\build\win\bin\Release\clingo.dll" "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP\runtimes\win-x64\native" -Force

    Write-Host "Copying logic program..." -ForegroundColor Green
    Copy-Item -Path "IndustryLP\DomainDefinition\logic_program" -Destination "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP" -Force -Recurse

    Write-Host "Copying IndustryLP Mod..." -ForegroundColor Green
    Copy-Item -Path @("IndustryLP\bin\Debug\ClingoSharp.*.dll", "IndustryLP\bin\Debug\ClingoSharp.dll", "IndustryLP\bin\Debug\IndustryLP.dll") -Destination "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\IndustryLP" -Force

    Write-Host "Done" -ForegroundColor Green
} catch {
    Write-Host "Cannot install IndustryLP: $PSItem" -ForegroundColor Red
}