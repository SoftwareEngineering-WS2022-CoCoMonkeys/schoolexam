FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SchoolExam.Web/SchoolExam.Web.csproj", "SchoolExam.Web/"]
RUN dotnet restore "SchoolExam.Web/SchoolExam.Web.csproj"
COPY . /.

WORKDIR "/src/SchoolExam.Web"
RUN dotnet build "SchoolExam.Web.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "SchoolExam.Web.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchoolExam.Web.dll"]
