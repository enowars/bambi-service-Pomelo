FROM mcr.microsoft.com/dotnet/sdk:5.0 AS backend-build
COPY Pomelo/Pomelo.csproj Pomelo/Pomelo.csproj
RUN dotnet restore Pomelo/Pomelo.csproj
COPY Pomelo Pomelo
RUN dotnet publish Pomelo/Pomelo.csproj -c Release -o /app

FROM node:16 AS frontend-build
RUN npm i -g vue-type-check
COPY Frontend/package.json Frontend/package.json
COPY Frontend/package-lock.json Frontend/package-lock.json
WORKDIR /Frontend
RUN npm install
COPY /Frontend .
RUN vtc --workspace . --srcDir ./src
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=frontend-build /Frontend/dist wwwroot/
COPY --from=backend-build /app .
ENTRYPOINT dotnet Pomelo.dll
