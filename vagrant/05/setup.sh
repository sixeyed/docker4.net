#!/bin/bash

# install Docker
curl -fsSL https://get.docker.com | sh

# use Docker without sudo
sudo usermod -aG docker vagrant
