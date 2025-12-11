FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5135
EXPOSE 5135


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["RoomBooking.Web/RoomBooking.Web.csproj", "RoomBooking.Web/"]
COPY ["RoomBooking.Application/RoomBooking.Application.csproj", "RoomBooking.Application/"]
COPY ["RoomBooking.Infrastructure/RoomBooking.Infrastructure.csproj", "RoomBooking.Infrastructure/"]

RUN dotnet restore "RoomBooking.Web/RoomBooking.Web.csproj"

COPY . .

WORKDIR "/src/RoomBooking.Web"
RUN dotnet publish "RoomBooking.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RoomBooking.Web.dll"]
