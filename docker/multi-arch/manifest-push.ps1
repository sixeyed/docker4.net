## TODO - needs to be added to GitHub job
## (the jobs build & push the variants)

$tags = @('docker4dotnet/whoami', 'docker4dotnet/fluentd') 

foreach ($tag in $tags) {
    docker manifest rm $tag

    docker manifest create `
    $tag `
    "$($tag):linux-amd64" `
    "$($tag):windows-amd64" 
    
    docker manifest push $tag
}