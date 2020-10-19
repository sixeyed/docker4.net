
# install pre-reqs
Install-WindowsFeature -Name Containers
Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force

# install Docker
Install-Module -Name DockerMsftProvider -Repository PSGallery -Force
Install-Package -Name docker -ProviderName DockerMsftProvider -Force
