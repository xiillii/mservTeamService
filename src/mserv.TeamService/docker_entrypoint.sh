#!/bin/bash
cd /pipeline/source/app/publish
dotnet mserv.TeamService.dll --server.urls=http://0.0.0.0:${PORT-"5000"}
