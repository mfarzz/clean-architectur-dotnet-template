$ErrorActionPreference = 'Stop'

function Invoke-DotNet {
    & dotnet @args
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet failed with exit code $LASTEXITCODE : $args"
    }
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$runRoot = Join-Path $repoRoot ('.nuget/template-tests/' + [guid]::NewGuid().ToString('N'))
$hive = Join-Path $runRoot 'hive'
$packageOutput = Join-Path $runRoot 'package'
$packageProject = Join-Path $repoRoot 'packaging/Mfarzz.CleanArchitecture.Template.csproj'

Invoke-DotNet pack $packageProject -c Release -o $packageOutput --nologo
$package = Get-ChildItem -LiteralPath $packageOutput -Filter '*.nupkg' | Select-Object -First 1
Invoke-DotNet new install $package.FullName --debug:custom-hive $hive --force

$cases = @(
    @{ Name = 'DefaultShop'; Options = @(); Provider = 'PostgreSQL'; ConnectionPrefix = 'Host='; Service = 'postgres' },
    @{ Name = 'ECommerce'; Options = @('-D', 'sqlserver'); Provider = 'SqlServer'; ConnectionPrefix = 'Server='; Service = 'sqlserver' },
    @{ Name = 'SqlShop'; Options = @('--db', 'sqlserver'); Provider = 'SqlServer'; ConnectionPrefix = 'Server='; Service = 'sqlserver' }
)

foreach ($case in $cases) {
    $output = Join-Path $runRoot $case.Name
    $templateOptions = $case.Options
    Invoke-DotNet new cleanapi -n $case.Name -o $output @templateOptions --no-update-check --debug:custom-hive $hive

    $settings = Get-Content (Join-Path $output "src/$($case.Name).WebApi/appsettings.json") -Raw | ConvertFrom-Json
    if ($settings.DatabaseProvider -ne $case.Provider -or
        -not $settings.ConnectionStrings.DefaultConnection.StartsWith($case.ConnectionPrefix)) {
        throw "Incorrect database configuration for $($case.Name)"
    }

    $compose = Get-Content (Join-Path $output 'docker-compose.yml') -Raw
    if ($compose -notmatch "(?m)^  $($case.Service):\s*$") {
        throw "Incorrect Docker service for $($case.Name)"
    }

    # Compile Application independently to catch missing direct references.
    Invoke-DotNet build (Join-Path $output "src/$($case.Name).Application/$($case.Name).Application.csproj") --nologo
    Invoke-DotNet test (Join-Path $output "$($case.Name).slnx") --nologo
}

Write-Host 'Template verification passed for default PostgreSQL, -D sqlserver, and --db sqlserver.'
