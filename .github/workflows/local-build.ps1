$ErrorActionPreference = "Stop"

# =========================
# CONFIG
# =========================
$solution = "FluentFlyout.sln"
$wapProject = "FluentFlyoutMSIX\FluentFlyoutMSIX.wapproj"
$platform = "x64"

# =========================
# GET VERSION
# =========================
[xml]$manifest = Get-Content "FluentFlyoutMSIX\Package.appxmanifest"
$fullVersion = $manifest.Package.Identity.Version
$version = ($fullVersion -split '\.')[0..2] -join '.'

Write-Host "Version: $version"

# =========================
# CONFIGURATIONS
# =========================
$configurations = @("GitHub Release") # add "Release" if needed

# =========================
# BUILD LOOP
# =========================
foreach ($config in $configurations) {

    Write-Host "=== Building: $config ==="

    $safeConfig = $config -replace '\s+', ''

    # output folder (MSIX intermediate)
    $appxDir = Join-Path $PWD ".AppxPackages"
    Remove-Item $appxDir -Recurse -Force -ErrorAction SilentlyContinue

    # installer output
    $installerOut = Join-Path $PWD ".InstallerOutput"
    Remove-Item $installerOut -Recurse -Force -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Force -Path $installerOut | Out-Null

    # =========================
    # RESTORE
    # =========================
    dotnet restore $solution

    # =========================
    # BUILD + PACKAGE
    # =========================
    $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe"

    & $msbuild $wapProject `
        /p:Configuration=$config `
        /p:Platform=$platform `
        /p:UapAppxPackageBuildMode=StoreUpload `
        /p:AppxPackageDir="$appxDir\" `
        /p:AppxPackageSigningEnabled=false

    Write-Host $(Get-Date -Format G)
    
    if($LASTEXITCODE -ne 0){
        Write-Host "`nFAILED"
        exit $LASTEXITCODE
    }

    # =========================
    # FIND OUTPUT
    # =========================
    $testFolder = Get-ChildItem $appxDir -Directory |
        Where-Object { $_.Name -like "*_Test*" } |
        Select-Object -First 1

    if (-not $testFolder) {
        throw "MSIX output folder not found"
    }

    Move-Item $testFolder.FullName "$installerOut\SystemFiles"

    Copy-Item ".github\build-files\FluentFlyout_Installer.bat" $installerOut
    Copy-Item ".github\build-files\FluentFlyout_Updater.bat" $installerOut
    Copy-Item ".github\build-files\README.txt" $installerOut

    # =========================
    # FINAL OUTPUT (Downloads-style)
    # =========================
    $dest = "$env:USERPROFILE\Downloads\FluentFlyout__$safeConfig"#${version}_$safeConfig"
    New-Item -ItemType Directory -Force -Path $dest | Out-Null

    Copy-Item -Recurse -Force "$installerOut\*" $dest

    Write-Host "`nDONE [$dest]"
}