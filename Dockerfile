# Working image built from guide:
# https://medium.com/front-end-weekly/net-core-web-api-with-docker-compose-postgresql-and-ef-core-21f47351224f
FROM microsoft/dotnet:2.2-sdk AS build
LABEL maintainer="e@dwelsh.uk"

WORKDIR /app
COPY ./src/Kaffee/Kaffee.csproj ./
RUN dotnet restore Kaffee.csproj

COPY ./src/Kaffee ./
RUN dotnet publish Kaffee.csproj -c Release -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Kaffee.dll"]