ARG DOTNET_VERSION
ARG SDK_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
WORKDIR /src
COPY ["DataProtectionDemo.csproj", "."]
RUN dotnet restore "./DataProtectionDemo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DataProtectionDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataProtectionDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataProtectionDemo.dll"]
