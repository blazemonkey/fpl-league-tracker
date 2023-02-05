FROM mcr.microsoft.com/mssql/server:2022-latest
WORKDIR /Scripts
COPY FPLV2.Database/Scripts /Scripts/
COPY FPLV2.Database/DockerInitDatabase.sh DockerInitDatabase.sh
ENV ACCEPT_EULA Y
ENV SA_PASSWORD Password123
CMD ["sh", "-c", "/opt/mssql/bin/sqlservr & sh DockerInitDatabase.sh $SA_PASSWORD FPLV2"]