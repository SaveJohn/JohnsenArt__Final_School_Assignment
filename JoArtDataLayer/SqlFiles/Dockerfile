FROM mysql:8.0.35

# -e MYSQL_ROOT_PASSWORD=gokstad
ENV MYSQL_ROOT_PASSWORD=gokstad

COPY JoArtDB-dump.sql /docker-entrypoint-initdb.d/
COPY JoArtDb-UserPriveleges.sql /docker-entrypoint-initdb.d/

# IKKE NØDVENDIG Å HA MED, men best practice
EXPOSE 3306