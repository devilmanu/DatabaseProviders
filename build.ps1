# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

exec { & dotnet restore DatabaseProviders.sln }

$suffix = "-ci-local"
$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = "$($suffix)-$($commitHash)"

Write-Output "build: Version suffix is $buildSuffix"

exec { & dotnet build DatabaseProviders.sln -c Release --version-suffix=$buildSuffix -v q /nologo }
	
Write-Output "Running unit tests"

Write-Output "Starting docker containers"

exec { & docker-compose -f docker-compose.test.yml up -d }


Write-Output "Running tests"
try {
Push-Location -Path .\GettingStarted.AspNetCore.Test
        exec { & dotnet test }
} finally {
        Pop-Location
}

Write-Output "Running Unit tests SqlServer"
try {
$env:Data__Store='SqlServer'
$env:ConnectionString__Pokemons="Server=.;Database=RazorPagesMovieContext-bc;Trusted_Connection=True;MultipleActiveResultSets=true;Provider=sql"
Push-Location -Path .\GettingStarted.AspNetCore.Test
        exec { & dotnet test }
} finally {
        Pop-Location
}

Write-Output "Running Unit tests Oracle"
$env:Data__Store='Oracle'
$env:ConnectionString__Pokemons="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xe)));User Id=system;Password=oracle;Provider=oracle"
try {
Push-Location -Path .\GettingStarted.AspNetCore.Test
        exec { & dotnet test }
} finally {
        Pop-Location
}


Write-Output "Finalizing docker containers"
exec { & docker-compose -f docker-compose.test.yml down }
