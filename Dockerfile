#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN echo 'deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main' >> /etc/apt/sources.list
RUN apt-get update \
&& apt-get install -y google-chrome-stable --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DocumentManager.csproj", "DocumentManager/"]
RUN dotnet restore "DocumentManager/DocumentManager.csproj"

WORKDIR "/src/DocumentManager"
COPY . .
RUN dotnet build "DocumentManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentManager.csproj" -c Release -o /app/publish /p:UseAppHost=false



FROM base AS final
WORKDIR /app
COPY "cert.pfx" "/app/cert.pfx"
COPY "cert.key" "/app/cert.key"
COPY "cert.crt" "/app/cert.crt"
COPY ["docs/mainPageHtml.txt", "/app/docs/"]
COPY ["docs/secondPageHtml.txt", "/app/docs/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocumentManager.dll"]
