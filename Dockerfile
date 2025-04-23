# Build Stage One - Select a subset of files in repository so that we can dotnet restore
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS restore-env
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset
WORKDIR /restore
COPY ./Applications /restore/Applications
COPY ./Common /restore/Common
RUN dotnet subset restore Applications/MyApp/MyApp.csproj \
  --root-directory /restore --output restore_subset/

# Build Stage Two - Execute the restore (which will be cached if no changes detected above) and build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build-env
WORKDIR /src
COPY --from=restore-env /restore/restore_subset .
RUN dotnet restore /src/Applications/MyApp/MyApp.csproj
COPY ./Applications Applications
COPY ./Common Common
RUN dotnet publish Applications/MyApp/MyApp.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine

# Add globalization support and configure for Workstation garbage collection
# See https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md#alpine-images
# and https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector
ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_gcServer=0 \ 
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache \
    icu-data-full \
    icu-libs \
    tzdata

# Run the service
WORKDIR /app
COPY --from=build-env /src/out .
ENTRYPOINT ["/app/MyApp"]
