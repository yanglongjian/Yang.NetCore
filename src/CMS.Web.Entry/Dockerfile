#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CMS.Web.Entry/CMS.Web.Entry.csproj", "CMS.Web.Entry/"]
COPY ["Yang.TencentCloud.Application/Yang.TencentCloud.Application.csproj", "Yang.TencentCloud.Application/"]
COPY ["Yang.Core/Yang.Core.csproj", "Yang.Core/"]
COPY ["Yang.Admin.Application/Yang.Admin.Application.csproj", "Yang.Admin.Application/"]
COPY ["Yang.Admin.Domain/Yang.Admin.Domain.csproj", "Yang.Admin.Domain/"]
COPY ["Yang.Gather.Application/Yang.Gather.Application.csproj", "Yang.Gather.Application/"]
COPY ["Yang.Gather.Domain/Yang.Gather.Domain.csproj", "Yang.Gather.Domain/"]
COPY ["Yang.Cms.Domain/Yang.Cms.Domain.csproj", "Yang.Cms.Domain/"]
COPY ["Yang.Sms.Application/Yang.Sms.Application.csproj", "Yang.Sms.Application/"]
COPY ["Yang.Cms.Application/Yang.Cms.Application.csproj", "Yang.Cms.Application/"]
COPY ["Yang.SiteMap/Yang.SiteMap.csproj", "Yang.SiteMap/"]
COPY ["Yang.Web.Pack/Yang.Web.Pack.csproj", "Yang.Web.Pack/"]
RUN dotnet restore "CMS.Web.Entry/CMS.Web.Entry.csproj"
COPY . .
WORKDIR "/src/CMS.Web.Entry"
RUN dotnet build "CMS.Web.Entry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.Web.Entry.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.Web.Entry.dll"]
