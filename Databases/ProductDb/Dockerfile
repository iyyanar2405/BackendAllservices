FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=YourPassword123!

EXPOSE 1433

COPY Tables/*.sql /docker-entrypoint-initdb.d/
COPY Scripts/*.sql /docker-entrypoint-initdb.d/
