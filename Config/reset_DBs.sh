#!/bin/bash

mssql-cli -S 127.0.0.1 -U sa -P Aa123456 -i ./DBs_reset.sql
