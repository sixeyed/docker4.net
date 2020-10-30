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

Write-Verbose "Initializing SignUp database"

# attach or create database: 
$mdfPath = "$($env:DATA_FOLDER)\SignUp.mdf"
$ldfPath = "$($env:DATA_FOLDER)\SignUp.ldf"
$sqlcmd = 'CREATE DATABASE SignUp'

if ((Test-Path $mdfPath) -eq $true) {
    $sqlcmd = "$sqlcmd ON (FILENAME = N'$mdfPath')"
    $sqlcmd = "$sqlcmd LOG ON (FILENAME = N'$ldfPath')"    
    $sqlcmd = "$sqlcmd FOR ATTACH"
    Write-Verbose "Attaching existing data files from path: $env:DATA_FOLDER"
}
else {
    mkdir -p $env:DATA_FOLDER
    $sqlcmd = "$sqlcmd ON (NAME = SignUp_dat, FILENAME = N'$mdfPath')"
    $sqlcmd = "$sqlcmd LOG ON (NAME = SignUp_log, FILENAME = N'$ldfPath')"
    Write-Verbose "Creating database with data file path: $env:DATA_FOLDER"
}

Write-Verbose "Invoke-Sqlcmd -Query $($sqlcmd) -ServerInstance '.\SQLEXPRESS'"
Invoke-Sqlcmd -Query $sqlcmd -ServerInstance ".\SQLEXPRESS"

Write-Verbose "Running LogMonitor and ServiceMonitor"
C:\LogMonitor.exe C:\ServiceMonitor.exe MSSQL`$SQLEXPRESS