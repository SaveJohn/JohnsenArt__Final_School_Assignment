FROM mysql:8.0.33

ENV MYSQL_ROOT_PASSWORD=gokstad

COPY joartdb_dump.sql /docker-entrypoint-initdb.d/
COPY JoArtDb-UserPriveleges.sql /docker-entrypoint-initdb.d/


EXPOSE 3306