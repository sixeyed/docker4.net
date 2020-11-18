#!/bin/bash

export VERSION='19.03.12'

# install Docker
curl -fsSL https://get.docker.com | sh

# use Docker without sudo
sudo usermod -aG docker vagrant
