FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SchoolExam.Web/SchoolExam.Web.csproj", "SchoolExam.Web/"]
RUN dotnet restore "SchoolExam.Web/SchoolExam.Web.csproj"
COPY . /.

WORKDIR "/src/SchoolExam.Web"
RUN dotnet build "SchoolExam.Web.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "SchoolExam.Web.csproj" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet SchoolExam.Web.dll
