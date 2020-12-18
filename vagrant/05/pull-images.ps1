
start-service docker 

$images = 
'docker4dotnet/whoami',
'docker4dotnet/signup-db:05-03',
'docker4dotnet/signup-web:05-03',
'docker4dotnet/homepage:05-03',
'docker4dotnet/save-handler:05-03'

foreach ($image in $images) {
    docker image pull $image
}