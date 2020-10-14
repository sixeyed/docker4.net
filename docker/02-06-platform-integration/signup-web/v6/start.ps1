
Write-Output 'Starting w3svc'
Start-Process C:\ServiceMonitor.exe -ArgumentList w3svc -NoNewWindow
    
Write-Output 'Making HTTP GET call'
Invoke-WebRequest http://localhost/app -UseBasicParsing | Out-Null

Write-Output 'Tailing log file'
Get-Content -path 'c:\logs\SignUp.log'
Get-Content -path 'c:\logs\SignUp.log' -Tail 1 -Wait