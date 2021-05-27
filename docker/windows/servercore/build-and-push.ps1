
# script to build a multi-arch Server Core image using the latest LTSC/SAC
# this will use the matching OS where Docker is running and supports process isolation
# needs to be built manually on Windows 10 as GHA doesn't have hyper-v isolation

$image='docker4dotnet/servercore'
$versions='ltsc2019','1903','1909','2004','20H2'

foreach($version in $versions) {

    docker image build --isolation=hyperv --pull --build-arg WINVER="$version" `
      -t "$($image):$($version)" .

    docker image push "$($image):$($version)"
}

docker manifest create --amend $image `
 "$($image):$($versions[0])" `
 "$($image):$($versions[1])" `
 "$($image):$($versions[2])" `
 "$($image):$($versions[3])" `
 "$($image):$($versions[4])"
 
docker manifest push $image
