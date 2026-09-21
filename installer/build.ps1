# Genera el instalador Guardarropa-Setup-<version>.exe en installer\output\
# Uso: powershell -File installer\build.ps1

$ErrorActionPreference = "Stop"
$env:PATH = "C:\Program Files\dotnet;$env:PATH"

$raiz = Split-Path -Parent $PSScriptRoot
$csproj = Join-Path $raiz "src\Guardarropa\Guardarropa.csproj"
$publishDir = Join-Path $raiz "publish\win-x64"
$iscc = "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"

Write-Host "Publicando Guardarropa (self-contained win-x64)..." -ForegroundColor Cyan
dotnet publish $csproj -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $publishDir

if (-not (Test-Path $iscc)) {
    throw "No se encuentra ISCC.exe en '$iscc'. Instala Inno Setup 6 (winget install JRSoftware.InnoSetup)."
}

Write-Host "Compilando el instalador con Inno Setup..." -ForegroundColor Cyan
& $iscc "$PSScriptRoot\Guardarropa.iss"

Write-Host "Instalador generado en installer\output\" -ForegroundColor Green
