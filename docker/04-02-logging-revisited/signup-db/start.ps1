param(
    [Parameter(Mandatory=$true)]
    [string] $sa_password,
    
    [Parameter(Mandatory=$true)]
    [string] $sa_password_path
)

Write-Verbose "Starting SQL Server"
start-service MSSQL`$SQLEXPRESS

# set the SA password
if ($sa_password_path -and (Test-Path $sa_password_path)) {
    $password = Get-Content -Raw $sa_password_path
    if ($password) {
        $sa_password = $password
        Write-Verbose "Using SA password from secret file: $sa_password_path"
    }
    else {
        Write-Verbose "WARN: Using default SA password, no password in secret file: $sa_password_path"
    }
}
else {
    Write-Verbose "WARN: Using default SA password, secret file not found at: $sa_password_path"
}

if ($sa_password) {
	Write-Verbose 'Changing SA login credentials'
    $sqlcmd = "ALTER LOGIN sa with password='$sa_password'; ALTER LOGIN sa ENABLE;"
    Invoke-SqlCmd -Query $sqlcmd -ServerInstance ".\SQLEXPRESS" 
}
else {
    Write-Verbose 'FATAL: SA password not supplied in sa_password or sa_password_path'
    return 1
}

Write-Verbose "Running LogMonitor and ServiceMonitor"
C:\LogMonitor.exe C:\ServiceMonitor.exe MSSQL`$SQLEXPRESS