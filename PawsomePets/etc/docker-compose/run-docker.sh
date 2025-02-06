#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p 574255a3-afe8-47fa-b933-d2cb5859ec96 -t
    fi
    cd ../
fi

docker-compose up -d
