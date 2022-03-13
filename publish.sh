#!/bin/bash

sudo mkdir -p /var/dotnet
sudo mkdir -p /var/dotnet/sys1
sudo mkdir -p /var/dotnet/sys2

sudo dotnet publish -o /var/dotnet/sys1
sudo cp Config/appsettings.sys1.json /var/dotnet/sys1

sudo systemctl stop bpms1.service
sudo systemctl enable bpms1.service
sudo systemctl start bpms1.service

sudo dotnet publish -o /var/dotnet/sys2
sudo cp Config/appsettings.sys2.json /var/dotnet/sys2

sudo systemctl stop bpms2.service
sudo systemctl enable bpms2.service
sudo systemctl start bpms2.service

sleep 3
sudo systemctl status bpms1.service
sudo systemctl status bpms2.service
