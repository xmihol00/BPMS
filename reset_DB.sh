#!/bin/bash

mssql-cli -S 127.0.0.1 -U sa -P Aa123456 -i ./DB_reset.sql
