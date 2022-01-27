#!/bin/bash

if [ "$1" = "redmine" ]
then
    #opera --remote "https://redmine.sperling.cz/projects/csi-telo/issues" 2>&1 >/dev/null &
    echo "unsupported"
elif [ "$1" = "gitlab" ]
then
    #opera --remote "https://gitlab.com/milanbx/csi-telocvik/-/merge_requests" 2>&1 >/dev/null &
    echo "unsupported"
elif [ "$1" = "issue" ]
then
    #opera --remote "https://redmine.sperling.cz/issues/$2" 2>&1 >/dev/null &
    echo "unsupported" 
elif [ "$1" = "add_migration" ]
then
    export PATH="$PATH:$HOME/.dotnet/tools/"
    export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true;
    dotnet ef migrations add $2 --project BPMS_DAL/BPMS_DAL.csproj --startup-project BPMS/BPMS.csproj
elif [ "$1" = "remove_migration" ]
then
    rm -rf /home/david/projs/bpms/BPMS_DAL/Migrations/
fi
