
Write-Output 'Starting w3svc'
Start-Service W3SVC
    
Write-Output 'Making HTTP GET call'
Invoke-WebRequest http://localhost/app -UseBasicParsing | Out-Null

Write-Output 'Tailing log file'
Get-Content -path 'c:\logs\SignUp.log' -Tail 1 -Wait