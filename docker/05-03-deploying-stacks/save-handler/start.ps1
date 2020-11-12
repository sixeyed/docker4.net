function EnsureConfigFile {
    param([string] $path)

    if (Test-Path $path) {    
        Write-Output "STARTUP: Config file exists at $path"
    }
    else {
        $source = "$($path).default"            
        New-Item -Path $path `
                 -ItemType SymbolicLink `
                 -Value $source
        Write-Output "STARTUP: Linked default config file to $path"
    }
}

Write-Output 'STARTUP: Setting up config files'
EnsureConfigFile "$env:APP_ROOT\config\connectionStrings.config"

Write-Output 'STARTUP: Running dependency check'
.\Utilities.MessageQueueCheck.exe 

if ($LastExitCode -eq 0) {
    Write-Output 'STARTUP: Running message handler'
    .\SignUp.MessageHandlers.SaveProspect.exe
}