## TODO - needs to be added to GitHub job
## (the jobs build & push the variants)

$tag='docker4dotnet/whoami'

docker manifest create -a `
 $tag `
 "$($tag):linux-amd64" `
 "$($tag):windows-amd64" 
 
docker manifest push $tag