[CmdletBinding()]
param([string]$DotnetPath)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$windowsRoot = Join-Path $repo 'windows'
$dist = [IO.Path]::GetFullPath((Join-Path $repo 'dist-windows'))
function Invoke-Dotnet {
    param([string[]]$CommandArgs)
    & $script:dotnet @CommandArgs
    if ($LASTEXITCODE -ne 0) { throw "dotnet $($CommandArgs[0]) failed ($LASTEXITCODE)" }
}
$candidates = @($DotnetPath, (Join-Path $env:LOCALAPPDATA 'KimiPocket/dotnet/dotnet.exe'))
$systemDotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($systemDotnet) { $candidates += $systemDotnet.Source }
$dotnet = $null
foreach ($candidate in $candidates) {
    if ($candidate -and (Test-Path -LiteralPath $candidate)) {
        $sdks = & $candidate --list-sdks
        if ($LASTEXITCODE -eq 0 -and ($sdks -match '^10\.0\.')) { $dotnet = $candidate; break }
    }
}
if (-not $dotnet) { throw 'Install the .NET 10 SDK or pass -DotnetPath <dotnet.exe>.' }
[xml]$project = Get-Content -LiteralPath (Join-Path $windowsRoot 'src/KimiPocket.App/KimiPocket.App.csproj') -Raw
$version = [string]$project.Project.PropertyGroup.Version
if ($version -notmatch '^\d+\.\d+\.\d+([-.][a-zA-Z0-9.]+)?$') { throw 'Invalid application version.' }
$buildRoot = [IO.Path]::GetFullPath((Join-Path $dist ('build-' + [guid]::NewGuid().ToString('N'))))
if (-not $buildRoot.StartsWith($dist + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) { throw 'Unsafe output path.' }
$publish = Join-Path $buildRoot 'Kimi-Pocket'
$results = Join-Path $buildRoot 'test-results'
New-Item -ItemType Directory -Path $buildRoot -Force | Out-Null
Push-Location $windowsRoot
try {
    Invoke-Dotnet @('restore','KimiPocket.sln','--configfile','NuGet.Config')
    Invoke-Dotnet @('format','KimiPocket.sln','--verify-no-changes','--no-restore','--verbosity','quiet')
    Invoke-Dotnet @('build','KimiPocket.sln','--configuration','Release','--no-restore','-warnaserror','--nologo')
    Invoke-Dotnet @('test','KimiPocket.sln','--configuration','Release','--no-build','--no-restore','--logger','trx','--results-directory',$results,'--nologo')
    $testCount = 0
    foreach ($file in Get-ChildItem -LiteralPath $results -Filter '*.trx') {
        [xml]$trx = Get-Content -LiteralPath $file.FullName -Raw
        $testCount += [int]$trx.TestRun.ResultSummary.Counters.total
        if ([int]$trx.TestRun.ResultSummary.Counters.failed -ne 0) { throw 'Tests failed.' }
    }
    if ($testCount -lt 1) { throw 'No tests executed.' }
    Invoke-Dotnet @('publish','src/KimiPocket.App/KimiPocket.App.csproj','--configuration','Release','--runtime','win-x64','--self-contained','true','-p:PublishTrimmed=false','-p:PublishSingleFile=false','-p:DebugType=None','--output',$publish,'--nologo')
    foreach ($required in @('KimiPocket.exe','KimiPocket.dll','coreclr.dll','hostfxr.dll','PresentationFramework.dll','Assets/Tray/light.ico')) {
        if (-not (Test-Path -LiteralPath (Join-Path $publish $required))) { throw "Missing package file: $required" }
    }
    Copy-Item -LiteralPath (Join-Path $windowsRoot 'README.md') -Destination (Join-Path $publish 'README.md')
    Copy-Item -LiteralPath (Join-Path $repo 'LICENSE') -Destination (Join-Path $publish 'LICENSE')
    $archive = Join-Path $dist "Kimi-Pocket-windows-x64-$version.zip"
    Compress-Archive -LiteralPath $publish -DestinationPath $archive -Force
    $hash = (Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash
    "$hash  $([IO.Path]::GetFileName($archive))" | Set-Content -LiteralPath ($archive + '.sha256') -Encoding ascii
    [pscustomobject]@{ version=$version; archive=$archive; executable=(Join-Path $publish 'KimiPocket.exe'); tests=$testCount; sha256=$hash; created=(Get-Date).ToString('o') } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $dist 'latest-build.json') -Encoding utf8
    Write-Host "Built $archive ($testCount tests passed)"
    Write-Host "SHA256 $hash"
} finally { Pop-Location }
