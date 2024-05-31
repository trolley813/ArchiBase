### ArchiBase 

ArchiBase is an engine for architectural database websites.

#### Usage with docker-compose
1. Generate a self-signed HTTPS certificate (for testing), or use a trusted certificate (for production) if you have one. Example of generation with OpenSSL:

```bash
# Create a PEM certificate (OpenSSL will ask for country, organization etc. Input anything you want. 3650 days ≈ 10 years)
openssl req -newkey rsa:2048 -new -nodes -x509 -days 3650 -keyout key.pem -out cert.pem
# Export to PFX file needed for .NET Core. Remember the export password!
openssl pkcs12 -export -in cert.pem -inkey key.pem -out pkcs12.pfx
```

2. Create the `docker-compose.yaml` with the following content and place it in some empty folder:
```yaml
services:
  archibase:
    image: ghcr.io/trolley813/archibase:0.1.1 # or later version if available
    environment:
      - ASPNETCORE_Kestrel__Certificates__Default__Password=<YOUR EXPORT PASSWORD HERE>
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/pkcs12.pfx 
      - ASPNETCORE_ConnectionStrings__ArchitectureDatabase=Host=10.94.0.2;Username=postgres;Password=example;Database=archibase
      - ASPNETCORE_SiteName=SovietArch
      - ASPNETCORE_CopyrightAuthors=SovietArch Team
      - ASPNETCORE_CopyrightYears=2015-2024
    volumes:
      - ./https:/https/:rw
    networks:
      abnet:
        ipv4_address: 10.94.0.1
    depends_on:
      db:
        condition: service_healthy
        restart: true
    deploy:
      resources:
        limits:
          cpus: "0.5"
          memory: 256M
        
  db:
    image: ghcr.io/cloudnative-pg/postgis:16-3.4-44
    environment:
      POSTGRES_PASSWORD: example
    volumes:
      - 'dbdata:/var/lib/postgresql/data:Z'
    networks:
      abnet:
        ipv4_address: 10.94.0.2
    healthcheck:
      test: ["CMD-SHELL", "pg_isready", "-d", "archibase"]
      interval: 30s
      timeout: 60s
      retries: 5
      start_period: 80s 
    deploy:
      resources:
        limits:
          cpus: "1.0"
          memory: 1024M

  adminer:
    image: ghcr.io/shyim/adminerevo:latest
    networks:
      abnet:
        ipv4_address: 10.94.0.3
    deploy:
      resources:
        limits:
          cpus: "0.5"
          memory: 256M
        
networks:
  abnet:
    driver: bridge
    ipam:
     config:
       - subnet: 10.94.0.0/16
         gateway: 10.94.0.254
         
volumes:
  dbdata:
```

3. Make the `https` subfolder and place your generated `pkcs12.pfx` certificate there.

4. On Linux, you'll probably have to set the permissions to the https folder and the certificate:
```bash
# Lower privileges (e.g. 755 or 644) may also work
sudo chown -R root:root https/
sudo chmod -R 777 https/
```
5. Run the containers
```bash
docker-compose up -d
```

After that, you will be able to use:
- the engine with HTTPS at https://10.94.0.1:443 (or just https://10.94.0.1, since the port 443 is assumed when using HTTPS) and https://10.94.0.1:8081;
- the engine with plain HTTP at http://10.94.0.1:80 (or just http://10.94.0.1, since the port 80 is assumed when using HTTP) and http://10.94.0.1:8080;
- PostgreSQL database (with PostGIS enabled, but unused at the moment) at [10.94.0.2:5432](10.94.0.2:5432) (username is `postgres` and password is `example`);
- Adminerevo database management tool at http://10.94.0.3:8081 (optional, you can use other software such as DBeaver to get to the database).

---
⚠️ Note: This will create a temporary admin with username `admin` and password `1Password!`, intended to give necessary rights to the regular admin user. ***When using on production, make sure that this account is disabled after the regular admin is created.***

---

#### License
ArchiBase is licensed under GNU Affero Public License version 3.0:

    ArchiBase in an engine for architectural database websites.
    Copyright (C) 2024  ArchiBase Team

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.