FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["LoginApi.csproj", "."]
RUN dotnet restore "LoginApi.csproj"

COPY . .
RUN dotnet publish "LoginApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_gcServer=0 \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache icu-data-full icu-libs tzdata

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LoginApi.dll"]
