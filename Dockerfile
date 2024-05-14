FROM mcr.microsoft.com/dotnet/sdk:8.0.101 AS build
WORKDIR /

ARG FULLVERSION
ARG SEMVERSION

# copy csproj and restore as distinct layers
RUN mkdir /MaxPower
COPY ./MaxPower/*.csproj /MaxPower/
WORKDIR /MaxPower/

RUN dotnet restore

# copy and build app and libraries
WORKDIR /
COPY ./MaxPower /MaxPower/
WORKDIR /MaxPower
RUN dotnet publish -c Release -r linux-arm64 -o out -p:PublishSingleFile=true --self-contained true /p:AssemblyVersion=$SEMVERSION /p:Version=$SEMVERSION

# RUNTIME CONTAINER
FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-jammy-arm64v8 AS runtime
EXPOSE 80
WORKDIR /app
COPY --from=build /MaxPower/out ./
RUN rm appsettings.Development.json
ENTRYPOINT ["./MaxPower", ""]