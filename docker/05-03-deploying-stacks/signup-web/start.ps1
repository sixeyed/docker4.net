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
EnsureConfigFile "$env:APP_ROOT\config\log4net.config"
EnsureConfigFile "$env:APP_ROOT\config\connectionStrings.config"

Write-Output 'STARTUP: Running LogMonitor and ServiceMonitor'
C:\LogMonitor.exe C:\ServiceMonitor.exe w3svc