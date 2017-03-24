@call dotnet --info
@set project=..\GracefullShutdown\
call dotnet restore %project%
call dotnet build -c Release %project%