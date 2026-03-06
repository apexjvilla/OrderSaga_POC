# Order SAGA Microservice POC
---

## Overview

---

## Build commands

**Start project**
```cmd
docker compose up --build
```

**Sttop project**
```cmd
docker compose down
```

## Database credentials

- **SERVER: **   
    localhost,1433
- **Username: **   
    sa
- **Password: **
    2026@Pass.Word

    Validate database with:
    ```cmd
    docker exec -it saga-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "2026@Pass.Word" -C -Q "SELECT name FROM sys.databases"
    ```

    ## Urls

    - [OrderService](http://localhost:5001)
    - [Rabbit MQ Dashboard](http://localhost:15672)
    - [Mongo DB Express](http://localhost:8081)