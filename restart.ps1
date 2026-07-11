$ErrorActionPreference = "Stop"

$serverRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$localSettings = Join-Path $serverRoot "server.local.ps1"
$serverExecutable = Join-Path $serverRoot "ragemp-server.exe"

Set-Location -LiteralPath $serverRoot

if (Test-Path -LiteralPath $localSettings) {
    . $localSettings
}

if (-not (Test-Path -LiteralPath $serverExecutable)) {
    throw "ragemp-server.exe wurde im Serverordner nicht gefunden."
}

while ($true) {
    $process = Start-Process `
        -FilePath $serverExecutable `
        -WorkingDirectory $serverRoot `
        -NoNewWindow `
        -PassThru

    $process.WaitForExit()
    Write-Host (
        "RageMP wurde mit Exit-Code {0} beendet. Neustart in 5 Sekunden." `
        -f $process.ExitCode
    )
    Start-Sleep -Seconds 5
}
